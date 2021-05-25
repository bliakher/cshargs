using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

using TokenReader = CShargs.ListReader<string>;

namespace CShargs
{
    /// <summary>
    /// Interface that generalizes all OptionAttribute types.
    /// </summary>
    internal interface IOptionAttribute
    {
        /*! \cond PRIVATE */

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

        internal OptionMetadata CreateMetadata(ParserMetadata parserMeta, MemberInfo member);

        /*! \endcond */
    }


    /// \addtogroup OptionAttributes
    /// @brief A list of attributes which are used to define options.
    /// 
    /// Annotate your extension of <see cref="Parser"/> with these attributes do define certain behavior for the parser.
    /// For details, see individual classes in this list.
    /// @{


    /// <summary>
    /// This attribute is used to annotate properties of a custom parser.
    /// 
    /// A flag option is a command line option without any value. It's either present or not.
    /// The flag option is recognised when the parser sees long/short
    /// symbol followed immediately by long/short name of this option.
    /// To define a flag option for your parser, annotate one of its bool properties with this attrubute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class FlagOptionAttribute : Attribute, IOptionAttribute
    {

        /// <summary>
        /// Define a flag option attached to the annotated property.
        /// </summary>
        /// <param name="longName">Long name of the option.</param>
        /// <param name="shortName">Short name of the option. Can be omitted.</param>
        /// <param name="useWith">Property name of other option, which is required for this option be used. Can be omitted.</param>
        /// <param name="help">Help text. Can be omitted.</param>
        public FlagOptionAttribute(
            string longName,
            char shortName = '\0',
            string useWith = null,
            string help = null)
        {
            ThrowIf.ArgumentNull(nameof(longName), longName);
            LongName = longName;
            ShortName = shortName;
            HelpText = help;
            UseWith = useWith;
        }

        internal string LongName { get; }
        internal char ShortName { get; }
        internal bool Required => false;
        internal string HelpText { get; }
        internal string UseWith { get; }

        string IOptionAttribute.LongName => LongName;
        char IOptionAttribute.ShortName => ShortName;
        bool IOptionAttribute.Required => Required;
        string IOptionAttribute.UseWith => UseWith;
        string IOptionAttribute.HelpText => HelpText;

        OptionMetadata IOptionAttribute.CreateMetadata(ParserMetadata parserMeta, MemberInfo member)
        {
            return new FlagOption(parserMeta, (PropertyInfo)member, this);
        }
    }

    /// <summary>
    /// This attribute is used to annotate properties of a custom parser.
    /// 
    /// A value option is a command line option in a "--key=value" style. The actual form of value options that
    /// are recognised by the parser differs based on the <see cref="ParserConfigAttribute"/> on the containing class.
    /// To define a value option for your parser, annotate one of its properties with this attrubute. Upon recognising
    /// the option on the command line, the parser will attempt to parse it by standard means, or by calling
    /// <code>public static T Parse(string)</code> on the propery type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ValueOptionAttribute : Attribute, IOptionAttribute
    {

        /// <summary>
        /// Define a value option attached to the annotated property.
        /// </summary>
        /// <param name="longName">Long name of the option.</param>
        /// <param name="required">If true, parser will require this option to be used. When useWith 
        /// not null, the option is required only if the useWith dependency was used.</param>
        /// <param name="shortName">Short name of the option. Can be omitted.</param>
        /// <param name="useWith">Property name of other option, which is required for this option be used. Can be omitted.</param>
        /// <param name="help">Help text. Can be omitted.</param>
        /// <param name="metaVar">Text that will be show in help as a placeholder for an actual value.</param>
        public ValueOptionAttribute(
            string longName,
            bool required = false,
            char shortName = '\0',
            string useWith = null,
            string help = null,
            string metaVar = null)
        {
            ThrowIf.ArgumentNull(nameof(longName), longName);
            LongName = longName;
            Required = required;
            ShortName = shortName;
            UseWith = useWith;
            HelpText = help;
            MetaVar = metaVar;
        }

        internal string LongName { get; }
        internal char ShortName { get; }
        internal bool Required { get; }
        internal string HelpText { get; }
        internal string UseWith { get; }
        internal string MetaVar { get; }

        string IOptionAttribute.LongName => LongName;
        char IOptionAttribute.ShortName => ShortName;
        bool IOptionAttribute.Required => Required;
        string IOptionAttribute.UseWith => UseWith;
        string IOptionAttribute.HelpText => HelpText;

        OptionMetadata IOptionAttribute.CreateMetadata(ParserMetadata parserMeta, MemberInfo member)
        {
            return new ValueOption(parserMeta, (PropertyInfo)member, this);
        }
    }

    /// <summary>
    /// This attribute is used to annotate properties of a custom parser.
    /// 
    /// A verb option works like a subcommand.
    /// When it's recognised on the command line, all the following arguments are passed to the subparser;
    /// To define a verb option for your parser, annotate one of its properties with this attribute.
    /// The type of the property <b>must</b> be a descendant of <see cref="Parser"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class VerbOptionAttribute : Attribute, IOptionAttribute
    {
        /// <summary>
        /// Define a verb option attached to the annotated property.
        /// </summary>
        /// <param name="name">Name of the option.</param>
        /// <param name="help">Help text. Can be omitted.</param>
        public VerbOptionAttribute(
            string name,
            string help = null)
        {
            ThrowIf.ArgumentNull(nameof(name), name);
            LongName = name;
            HelpText = help;
        }

        internal string LongName { get; }
        internal string HelpText { get; }

        string IOptionAttribute.LongName => LongName;
        char IOptionAttribute.ShortName => '\0';
        bool IOptionAttribute.Required => false;
        string IOptionAttribute.UseWith => null;
        string IOptionAttribute.HelpText => HelpText;


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

        internal string LongName { get; }
        internal char ShortName { get; }
        internal bool Required { get; }
        internal string HelpText { get; }
        internal string UseWith { get; }

        string IOptionAttribute.LongName => LongName;
        char IOptionAttribute.ShortName => ShortName;
        bool IOptionAttribute.Required => Required;
        string IOptionAttribute.UseWith => UseWith;
        string IOptionAttribute.HelpText => HelpText;

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
            ThrowIf.ArgumentNull("Alias", Alias, "Alias name can't be null.");
        }

        /// <summary>
        /// Alias name, can be short or long
        /// </summary>
        /// <summary>
        /// names of propeties in parser that are targets of alias
        /// </summary>
        /// <summary>
        /// If alias is long, returns Alias -> alias is a long option
        /// </summary>
        /// <summary>
        /// If alias is short, returns Alias -> alias is a short option
        /// </summary>
        /// <summary>
        /// Required always false - required of target option is used
        /// </summary>
        /// <summary>
        /// Dependency always null - dependency of target option is used
        /// </summary>
        /// <summary>
        /// Help text always null - help text of target option is used
        /// </summary>
        internal string Alias { get; private init; }
        internal IReadOnlyList<string> Targets { get; private init; }
        internal string LongName => Alias.Length != 1 ? Alias : null;
        internal char ShortName => Alias.Length == 1 ? Alias[0] : '\0';
        internal bool Required => false;
        internal string UseWith => null;
        internal string HelpText => null;
        string IOptionAttribute.LongName => LongName;
        char IOptionAttribute.ShortName => ShortName;
        bool IOptionAttribute.Required => Required;
        string IOptionAttribute.UseWith => UseWith;
        string IOptionAttribute.HelpText => HelpText;

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

    /// @}

}