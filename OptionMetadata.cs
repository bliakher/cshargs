using System;
using System.Reflection;

using TokenReader = ListReader<string>;

namespace CShargs
{
    internal class OptionMetadata
    {
        private MemberInfo member_ { get; init; }
        private IOptionAttribute attribute_ { get; init; }

        public Type OptionType { get; private init; }
        public bool Required => attribute_.Required;
        public OptionMetadata UseWith {get; private set;}
        public string UseWithName => attribute_.UseWith;

        public OptionMetadata(MemberInfo member, IOptionAttribute attribute)
        {
            ThrowIf.ArgumentNull(nameof(attribute), attribute);
            ThrowIf.ArgumentNull(nameof(member), member);

            OptionType = member.GetOwnType();
            member_ = member;
        }

        public MethodInfo GetParseMethod() {
            if (member_.MemberType != MemberTypes.Property) {
                throw new InvalidOperationException("Member must be a property");
            }

            // TODO: return OptionType.GetMethod("Parse")
        }

        public void SetValue(object instance, object value)
        {
            attribute_.SetValue(instance, member_, value);
        }
    }
}