using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

using TokenReader = CShargs.ListReader<string>;

namespace CShargs
{
    interface IOptionAttribute
    {
        string LongName { get; }
        char ShortName { get; }
        bool Required { get; }
        string UseWith { get; }
        string HelpText { get; }
        bool CanConcat { get; }

        internal OptionMetadata CreateMetadata(ParserMetadata parserMeta, MemberInfo member);
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FlagOptionAttribute : Attribute, IOptionAttribute
    {
        public FlagOptionAttribute(
            string name,
            char shortName = '\0',
            string useWith = null,
            string help = null)
        {
            ThrowIf.ArgumentNull(nameof(name), name);
            LongName = name;
            ShortName = shortName;
            HelpText = help;
            UseWith = useWith;
        }

        public string LongName { get; private init; }
        public char ShortName { get; private init; }
        public bool Required => false;
        public string HelpText { get; private init; }
        public string UseWith { get; private init; }
        public bool CanConcat => true;

        OptionMetadata IOptionAttribute.CreateMetadata(ParserMetadata parserMeta, MemberInfo member)
        {
            return new FlagOption(parserMeta, (PropertyInfo)member, this);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ValueOptionAttribute : Attribute, IOptionAttribute
    {
        public ValueOptionAttribute(
            string name,
            bool required = false,
            char shortName = '\0',
            string useWith = null,
            string help = null,
            string metaVar = null)
        {
            ThrowIf.ArgumentNull(nameof(name), name);
            LongName = name;
            Required = required;
            ShortName = shortName;
            UseWith = useWith;
            HelpText = help;
            MetaVar = metaVar;
        }

        public string LongName { get; private init; }
        public char ShortName { get; private init; }
        public bool Required { get; private init; }
        public string HelpText { get; private init; }
        public string UseWith { get; private init; }
        public bool CanConcat => false;
        public string MetaVar { get; }

        OptionMetadata IOptionAttribute.CreateMetadata(ParserMetadata parserMeta, MemberInfo member)
        {
            return new ValueOption(parserMeta, (PropertyInfo)member, this);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class VerbOptionAttribute : Attribute, IOptionAttribute
    {
        public VerbOptionAttribute(
            string name,
            string help = null)
        {
            ThrowIf.ArgumentNull(nameof(name), name);
            LongName = name;
            HelpText = help;
        }

        public string LongName { get; private init; }
        public char ShortName => '\0';
        public bool Required => false;
        public string UseWith => null;
        public string HelpText { get; private init; }
        public bool CanConcat => false;


        OptionMetadata IOptionAttribute.CreateMetadata(ParserMetadata parserMeta, MemberInfo member)
        {
            return new VerbOption(parserMeta, (PropertyInfo)member, this);
        }
    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CustomOptionAttribute : Attribute, IOptionAttribute
    {
        public CustomOptionAttribute(
            string name,
            bool required = false,
            char shortName = '\0',
            string useWith = null,
            string help = null)
        {
            ThrowIf.ArgumentNull(nameof(name), name);
            LongName = name;
            Required = required;
            ShortName = shortName;
            UseWith = useWith;
            HelpText = help;
        }

        public string LongName { get; private init; }
        public char ShortName { get; private init; }
        public bool Required { get; private init; }
        public string HelpText { get; private init; }
        public string UseWith { get; private init; }
        public bool CanConcat => false;

        OptionMetadata IOptionAttribute.CreateMetadata(ParserMetadata parserMeta, MemberInfo member)
        {
            return new CustomOption(parserMeta, (MethodInfo)member, this);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AliasOptionAttribute : Attribute, IOptionAttribute
    {
        public AliasOptionAttribute(
            string alias,
            params string[] targets)
        {
            Alias = alias;
            Targets = targets;
            ThrowIf.ArgumentNull("Alias", Alias, "Alias name can't be null." );
        }

        public string Alias { get; private init; }
        public IReadOnlyList<string> Targets { get; private init; }
        public string LongName => Alias.Length != 1 ? Alias : null;
        public char ShortName => Alias.Length == 1 ? Alias[0] : '\0';
        public bool Required => false;
        public string UseWith => null;
        public string HelpText => null;
        public bool CanConcat => true;

        OptionMetadata IOptionAttribute.CreateMetadata(ParserMetadata parserMeta, MemberInfo member)
        {
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class OptionGroupAttribute : Attribute
    {
        public string[] OptionGroup { get; }
        public bool Required { get; }
        public string useWith { get; init; }

        public OptionGroupAttribute(
            bool required,
            params string[] optionGroup
            )
        {
            OptionGroup = optionGroup;
            Required = required;
        }
    }

}