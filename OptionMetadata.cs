using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TokenReader = CShargs.ListReader<string>;

namespace CShargs
{
    internal abstract class OptionMetadata
    {
        protected readonly ParserMetadata parserMeta_;
        protected IOptionAttribute attribute_ { get; init; }
        protected MemberInfo member_ { get; init; }
        public string LongName => attribute_.LongName;
        public bool Required => attribute_.Required;
        public OptionMetadata UseWith { get; set; }
        public string UseWithName => attribute_.UseWith;
        public char ShortName => attribute_.ShortName;

        public OptionMetadata(ParserMetadata parserMeta, MemberInfo member, IOptionAttribute attribute)
        {
            ThrowIf.ArgumentNull(nameof(parserMeta), parserMeta);
            ThrowIf.ArgumentNull(nameof(attribute), attribute);

            member_ = member;
            attribute_ = attribute;
            parserMeta_ = parserMeta;
        }


        public abstract void Parse(Parser parser, string value, TokenReader tokens);
        protected abstract void SetValue(object instance, object value);

        public virtual string GetRawName(bool preferShort = true)
        {
            if (preferShort && attribute_.ShortName != '\0') {
                return parserMeta_.Config.ShortOptionSymbol + attribute_.ShortName;
            }

            return parserMeta_.Config.LongOptionSymbol + LongName;
        }

        public IEnumerable<IRule> ExtractRules()
        {
            var rules = new List<IRule>();
            if (Required) {
                rules.Add(new RequiredRule(this));
            }
            if (UseWith != null) {
                rules.Add(new DependencyRule(this));
            }
            return rules;
        }

        public override string ToString()
        {
            return GetRawName();
        }
    }

    internal sealed class FlagOption : OptionMetadata
    {
        public PropertyInfo Property => (PropertyInfo)member_;

        public FlagOption(ParserMetadata parserMeta, PropertyInfo prop, FlagOptionAttribute attribute)
            : base(parserMeta, prop, attribute) { }

        public override void Parse(Parser userParser, string value, TokenReader reader)
        {
            SetValue(userParser, true);
        }

        protected override void SetValue(object instance, object value)
            => Property.SetValue(instance, value);
    }

    internal sealed class ValueOption : OptionMetadata
    {
        public PropertyInfo Property => (PropertyInfo)member_;
        private Delegate staticParse_ = null;
        private new ValueOptionAttribute attribute_ => (ValueOptionAttribute)base.attribute_;

        public string MetaVar => attribute_.MetaVar ?? Property.Name.ToUpper();

        public ValueOption(ParserMetadata parserMeta, PropertyInfo prop, ValueOptionAttribute attribute)
            : base(parserMeta, prop, attribute) { }

        public override void Parse(Parser parser, string valueStr, TokenReader tokens)
        {
            // TODO: check parser options
            if (valueStr == null) {
                if (tokens.EndOfList) {
                    throw new MissingOptionValueException(tokens.Peek(-1));
                }
                valueStr = tokens.Read();
            }

            object value;
            if (Property.PropertyType == typeof(string)) {
                value = valueStr;
            } else {
                // ex: int.Parse(...)
                value = InvokeStaticParseMethod(valueStr);
            }
            SetValue(parser, value);
        }

        protected override void SetValue(object userObject, object value)
            => Property.SetValue(userObject, value);

        public override string GetRawName(bool preferShort = true)
        {
            if (preferShort && attribute_.ShortName != '\0') {
                string name = base.GetRawName(true);

                if (!parserMeta_.Config.OptionFlags.HasFlag(OptionFlags.ForbidShortEquals)) {
                    name += parserMeta_.Config.EqualsSymbol + MetaVar;
                } else if (!parserMeta_.Config.OptionFlags.HasFlag(OptionFlags.ForbidShortSpace)) {
                    name += ' ' + MetaVar;
                } else if (!parserMeta_.Config.OptionFlags.HasFlag(OptionFlags.ForbidShortNoSpace)) {
                    name += MetaVar;
                }
                return name;
            } else {
                string name = base.GetRawName(false);

                if (!parserMeta_.Config.OptionFlags.HasFlag(OptionFlags.ForbidLongEquals)) {
                    name += parserMeta_.Config.EqualsSymbol + MetaVar;
                } else if (!parserMeta_.Config.OptionFlags.HasFlag(OptionFlags.ForbidLongSpace)) {
                    name += ' ' + MetaVar;
                }

                return name;
            }
        }

        private object InvokeStaticParseMethod(string value)
        {
            if (staticParse_ == null) {
                if (member_.MemberType != MemberTypes.Property) {
                    throw new InvalidOperationException("Member must be a property");
                }

                var propType = Property.PropertyType;

                IEnumerable<MethodInfo> methods;

                if (Property.PropertyType.IsEnum) {

                    try {
                        return Enum.Parse(Property.PropertyType, value, parserMeta_.Config.OptionFlags.HasFlag(OptionFlags.EnumCaseInsensitive));
                    } catch (ArgumentException ex) {
                        throw new FormatException(ex.Message);
                    }

                } else {
                    var allMethods = propType.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    var parseMethods = allMethods.Where(m => m.Name == "Parse");
                    var typedMethods = parseMethods.Where(m => m.ReturnType == propType);
                    methods = typedMethods.Where(m => {
                        var param = m.GetParameters().ToArray();
                        return param.Length == 1 && param[0].ParameterType == typeof(string);
                    });
                }

                if (!methods.Any()) {
                    throw new ConfigurationException(
                        $"Option {LongName}: {propType} must have public static method Parse(string) which returns {propType}");
                }
                var ParseMethod = methods.First();

                var delegateType = Expression.GetDelegateType(typeof(string), propType);
                staticParse_ = Delegate.CreateDelegate(delegateType, ParseMethod);
            }

            try {
                return staticParse_.DynamicInvoke(value);
            } catch (TargetInvocationException ex) {
                throw ex.InnerException;
            }
        }
    }

    internal class CustomOption : OptionMetadata
    {
        public MethodInfo Method => (MethodInfo)member_;
        public CustomOption(ParserMetadata parserMeta, MethodInfo method, CustomOptionAttribute attribute)
            : base(parserMeta, method, attribute) { }

        public override void Parse(Parser parser, string value, TokenReader tokens)
            => SetValue(parser, tokens.Peek(-1));

        protected override void SetValue(object instance, object value)
        {
            try {
                Method.Invoke(instance, new[] { value });
            } catch (TargetInvocationException e) {
                throw e.InnerException;
            }
        }
    }

    internal class VerbOption : OptionMetadata
    {
        public PropertyInfo Property => (PropertyInfo)member_;
        public VerbOption(ParserMetadata parserMeta, PropertyInfo prop, VerbOptionAttribute attribute)
            : base(parserMeta, prop, attribute) { }

        public override void Parse(Parser parser, string value, TokenReader tokens)
        {
            var subparser = (Parser)Activator.CreateInstance(Property.PropertyType);

            string[] rest = tokens.ReadToEnd().ToArray();
            subparser.Parse(rest);
            SetValue(parser, subparser);
        }

        protected override void SetValue(object instance, object value)
            => Property.SetValue(instance, value);
    }


    internal class AliasOption : OptionMetadata
    {
        public IEnumerable<OptionMetadata> Targets { get; private set; }
        public AliasOption(ParserMetadata parserMeta, AliasOptionAttribute attribute, IEnumerable<OptionMetadata> targets)
            : base(parserMeta, null, attribute)
        {
            Targets = targets.ToArray();
        }

        public override void Parse(Parser parser, string value, TokenReader tokens)
        {
            foreach (var target in Targets) {
                parser.ParseOption(target, null);
            }
        }

        protected override void SetValue(object instance, object value)
            => throw new InvalidOperationException();
    }
}