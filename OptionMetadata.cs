using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using TokenReader = ListReader<string>;

namespace CShargs
{
    internal class OptionMetadata
    {
        private MemberInfo member_ { get; init; }
        private IOptionAttribute attribute_ { get; init; }

        public string Name => attribute_.Name;
        public Type OptionType { get; private init; }
        public bool Required => attribute_.Required;
        public OptionMetadata UseWith { get; set; }
        public string UseWithName => attribute_.UseWith;

        public OptionMetadata(MemberInfo member, IOptionAttribute attribute)
        {
            ThrowIf.ArgumentNull(nameof(attribute), attribute);
            ThrowIf.ArgumentNull(nameof(member), member);

            OptionType = member.GetOwnType();
            member_ = member;
        }

        private MethodInfo staticParse_ = null;
        public object InvokeStaticParseMethod(object instance, string value)
        {
            if (staticParse_ == null) {
                if (member_.MemberType != MemberTypes.Property) {
                    throw new InvalidOperationException("Member must be a property");
                }
                var methods = from method in OptionType.GetMethods(BindingFlags.Static)
                              where method.Name == "Parse"
                              where method.ReturnType == OptionType
                              let param = method.GetParameters()
                              where param.Length == 1 && param[0].ParameterType == typeof(string)
                              select method;
                if (!methods.Any()) {
                    throw new ConfigurationException($"Option {Name}: {OptionType} must have public static method Parse(string) which returns {OptionType}");
                }
                staticParse_ = methods.First();
            }

            return staticParse_.Invoke(instance, new object[] { value });
        }

        public void Parse(object instance, TokenReader tokens) {
            object value = attribute_.Parse(instance, this, tokens);
            SetValue(instance, value);
        }

        public void SetValue(object instance, object value)
        {
            attribute_.SetValue(instance, member_, value);
        }
    }
}