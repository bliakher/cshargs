using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

using TokenReader = CShargs.ListReader<string>;

namespace CShargs
{
    /// <summary>
    /// Interface that generalizes all OptionAttribute types
    /// </summary>
    internal interface IOptionAttribute
    {
        /// <summary>
        /// Long name of option without option symbol ie. "number-of-cats" for option --number-of-cats
        /// </summary>
        string LongName { get; }
        /// <summary>
        /// Short name of option without option symbol ie. 'c' for option -c
        /// </summary>
        char ShortName { get; }
        /// <summary>
        /// If option is required
        /// </summary>
        bool Required { get; }
        /// <summary>
        /// Dependency of option - option can be used only if dependency is present
        /// Name of property in parser class which is associated with dependency
        /// </summary>
        string UseWith { get; }
        /// <summary>
        /// Help text of option
        /// </summary>
        string HelpText { get; }
        bool CanConcat { get; }

        internal OptionMetadata CreateMetadata(ParserMetadata parserMeta, MemberInfo member);
    }

    /// \addtogroup Options
    /// <summary>
    /// Flag option attribute is used for options without parameters. The type of the target property must be bool.
    /// Target is property inside the parser class
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class FlagOptionAttribute : Attribute, IOptionAttribute
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
        /// <summary>
        /// Flag option cannot be required
        /// </summary>
        public bool Required => false;
        public string HelpText { get; private init; }
        public string UseWith { get; private init; }
        public bool CanConcat => true;

        OptionMetadata IOptionAttribute.CreateMetadata(ParserMetadata parserMeta, MemberInfo member)
        {
            return new FlagOption(parserMeta, (PropertyInfo)member, this);
        }
    }

    /// <summary>
    /// Value option attribute is used for options with parameters.
    /// Target is property inside the parser class
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ValueOptionAttribute : Attribute, IOptionAttribute
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

    /// <summary>
    /// Verb option attribute is used for subcommands.
    /// Target is property inside the parser class of type Parser
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class VerbOptionAttribute : Attribute, IOptionAttribute
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
        /// <summary>
        /// Verb option doesn't have a long name
        /// </summary>
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

    /// <summary>
    /// For options that need some context during parsing
    /// Target is method in parser class with signature void MyMethod(string value)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CustomOptionAttribute : Attribute, IOptionAttribute
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

    /// <summary>
    /// For defining aliases of options
    /// Target is the parser class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class AliasOptionAttribute : Attribute, IOptionAttribute
    {
        public AliasOptionAttribute(
            string alias,
            params string[] targets)
        {
            Alias = alias;
            Targets = targets;
            ThrowIf.ArgumentNull("Alias", Alias, "Alias name can't be null." );
        }
        /// <summary>
        /// Alias name, can be short or long
        /// </summary>
        public string Alias { get; private init; }
        /// <summary>
        /// names of propeties in parser that are targets of alias
        /// </summary>
        public IReadOnlyList<string> Targets { get; private init; }
        /// <summary>
        /// If alias is long, returns Alias -> alias is a long option
        /// </summary>
        public string LongName => Alias.Length != 1 ? Alias : null;
        /// <summary>
        /// If alias is short, returns Alias -> alias is a short option
        /// </summary>
        public char ShortName => Alias.Length == 1 ? Alias[0] : '\0';
        /// <summary>
        /// Required always false - required of target option is used
        /// </summary>
        public bool Required => false;
        /// <summary>
        /// Dependency always null - dependency of target option is used
        /// </summary>
        public string UseWith => null;
        /// <summary>
        /// Help text always null - help text of target option is used
        /// </summary>
        public string HelpText => null;
        public bool CanConcat => true;

        OptionMetadata IOptionAttribute.CreateMetadata(ParserMetadata parserMeta, MemberInfo member)
        {
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// For defining exclusive groups of options.
    /// The <see cref="Parser"/> class is the target.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class OptionGroupAttribute : Attribute
    {
        /// <summary>
        /// Names of properties in parser that are part of this group.
        /// </summary>
        public string[] OptionGroup { get; }
        /// <summary>
        /// If required, one of group options must be used
        /// Options inside an option group cannot be required -> ConfigurationException
        /// </summary>
        public bool Required { get; }
        /// <summary>
        /// Dependency of whole option group
        /// Options inside an option group cannot have dependencies -> ConfigurationException
        /// </summary>
        public string UseWith { get; init; }

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