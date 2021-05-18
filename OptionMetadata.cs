using System;
using System.Reflection;

namespace CShargs
{
    internal class OptionMetadata
    {
        private readonly MemberInfo member_;
        private readonly IOptionAttribute attribute_;

        public Type OptionType { get; private init; }

        public OptionMetadata(MemberInfo member, IOptionAttribute attribute)
        {
            ThrowIf.ArgumentNull(nameof(attribute), attribute);
            ThrowIf.ArgumentNull(nameof(member), member);

            OptionType = member.GetOwnType();
            member_ = member;
        }

        public void SetValue(object instance, object value)
        {
            attribute_.SetValue(instance, member_, value);
        }
    }
}