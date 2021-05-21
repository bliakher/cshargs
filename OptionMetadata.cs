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
            ThrowIf.ArgumentNull(nameof(attribute), attribute);

            member_ = member;
            attribute_ = attribute;
            parserMeta_ = parserMeta;
        }


        public abstract object Parse(TokenReader tokens);
        public abstract void SetValue(object instance, object value);

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
    }

    internal sealed class FlagOption : OptionMetadata
    {
        public PropertyInfo Property => (PropertyInfo)member_;

        public FlagOption(ParserMetadata parserMeta, PropertyInfo prop, FlagOptionAttribute attribute)
            : base(parserMeta, prop, attribute) { }

        public override object Parse(TokenReader reader)
        {
            return true;
        }

        public override void SetValue(object instance, object value)
            => Property.SetValue(instance, value);
    }

    internal sealed class ValueOption : OptionMetadata
    {
        public PropertyInfo Property => (PropertyInfo)member_;
        private Delegate staticParse_ = null;

        public ValueOption(ParserMetadata parserMeta, PropertyInfo prop, ValueOptionAttribute attribute)
            : base(parserMeta, prop, attribute) { }

        public override object Parse(TokenReader tokens)
        {
            Debug.Assert(tokens.Position > 0);
            string first = tokens.Peek(-1);
            string value;
            int index;
            // TODO: check parser options
            if ((index = first.IndexOf('=')) != -1) {
                value = first.Substring(index + 1);
            } else {
                value = tokens.Read();
            }

            // ex: int.Parse(value)
            return InvokeStaticParseMethod(value);
        }

        public override void SetValue(object instance, object value)
            => Property.SetValue(instance, value);

        private object InvokeStaticParseMethod(string value)
        {
            if (staticParse_ == null) {
                if (member_.MemberType != MemberTypes.Property) {
                    throw new InvalidOperationException("Member must be a property");
                }

                var propType = Property.PropertyType;
                var methods = from method in propType.GetMethods(BindingFlags.Static)
                              where method.Name == "Parse"
                              where method.ReturnType == propType
                              let param = method.GetParameters()
                              where param.Length == 1 && param[0].ParameterType == typeof(string)
                              select method;

                if (!methods.Any()) {
                    throw new ConfigurationException(
                        $"Option {LongName}: {propType} must have public static method Parse(string) which returns {propType}");
                }
                var ParseMethod = methods.First();

                var delegateType = Expression.GetDelegateType(typeof(string), propType);
                staticParse_ = Delegate.CreateDelegate(delegateType, ParseMethod);
            }

            return staticParse_.DynamicInvoke(value);
        }
    }

    internal class CustomOption : OptionMetadata
    {
        public MethodInfo Method => (MethodInfo)member_;
        public CustomOption(ParserMetadata parserMeta, MethodInfo method, CustomOptionAttribute attribute)
            : base(parserMeta, method, attribute) { }

        public override object Parse(TokenReader tokens)
            => tokens.Peek(-1);

        public override void SetValue(object instance, object value)
            => Method.Invoke(instance, new[] { value });
    }

    internal class VerbOption : OptionMetadata
    {
        public PropertyInfo Property => (PropertyInfo)member_;
        public VerbOption(ParserMetadata parserMeta, PropertyInfo prop, VerbOptionAttribute attribute)
            : base(parserMeta, prop, attribute) { }

        public override object Parse(TokenReader tokens)
        {
            var subparser = (Parser)Activator.CreateInstance(Property.PropertyType);

            string[] rest = tokens.ReadToEnd().ToArray();
            subparser.Parse(rest);
            return subparser;
        }

        public override void SetValue(object instance, object value)
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

        public override object Parse(TokenReader tokens)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object instance, object value)
        {
            throw new NotImplementedException();
        }
    }
}