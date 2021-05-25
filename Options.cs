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

    /// 
    /// \example TimeExample.cs
    /// This is an example implementation of argument parser for GNU `time`.
    /// 
    /// \example GitExample.cs
    /// This is an example of a tiny subset of the `git` command.
    /// It demonstrates the usage of CShargs.VerbOptionAttribute
    /// 
    /// \example NumactlExample.cs
    /// This is an example implementation of the `numactl` command.
    /// 

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
    /// 
    /// </summary>
    /// 
    /// ## Example
    /// \dontinclude TimeExample.cs
    /// Following flag option
    /// \skipline verbose
    /// \skipline *
    /// Could be defined like this
    /// \skip "verbose"
    /// \until }
    /// 
    /// For full example see \ref TimeExample.cs
    ///  
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
    /// 
    /// ## Example
    /// \dontinclude TimeExample.cs
    /// Following value option
    /// \skipline format
    /// \skipline *
    /// Could be defined like this
    /// \skip "format"
    /// \until }
    /// 
    /// For full example see \ref TimeExample.cs
    /// 
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
    /// 
    /// ## Example
    /// \dontinclude GitExample.cs
    /// \skip "push"
    /// \until }
    /// 
    /// For full example see \ref GitExample.cs
    /// 
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
    /// This attribute is used to annotate methods of a custom parser.
    /// 
    /// If you need an option that needs some context during its own parsing,
    /// or you need to interpret the raw arguments in slightly different way, use custom option.
    /// </summary>
    /// 
    /// The annotated method must be of type CShargs.CustomOptionAttribute.Parser
    /// 
    /// ## Example
    /// \dontinclude CustomExample.cs
    /// \skip //
    /// \until }
    /// \until }
    /// 
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

        /// <summary>
        /// Delegate of a custom option method.
        /// </summary>
        /// <param name="rawValue">The raw parameter from command line.</param>
        public delegate void Parser(string rawValue);

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
    /// This attribute is used to annotate the class of a custom parser.
    /// 
    /// Aliases are just other ways how to write an existing option.
    /// Aliases can target multiple flag options at once, or one option of any type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class AliasOptionAttribute : Attribute, IOptionAttribute
    {
        /// <summary>
        /// Create an alias option attached to the specified option(s).
        /// </summary>
        /// <param name="alias">Alias name, can be short or long.</param>
        /// <param name="targets">Names of propeties in parser that are targets of this alias.</param>
        public AliasOptionAttribute(
            string alias,
            params string[] targets)
        {
            Alias = alias;
            Targets = targets;
            ThrowIf.ArgumentNull("Alias", Alias, "Alias name can't be null.");
        }

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
    /// This attribute is used to annotate the class of a custom parser.
    /// 
    /// You can use option groups to mark certain options to be mutually exclusive.
    /// </summary>
    /// 
    /// ## Example
    /// 
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class OptionGroupAttribute : Attribute
    {
        /// <summary>
        /// Create an option group associated to the given options.
        /// </summary>
        /// <param name="required">If required, one of group options must be used. Individual options inside an option group cannot be required.</param>
        /// <param name="options">Names of properties in the parser that are part of this group.</param>
        public OptionGroupAttribute(
            bool required,
            params string[] options
            )
        {
            this.Options = options;
            Required = required;
        }

        internal string[] Options { get; }
        internal bool Required { get; }

        /// <summary>
        /// Dependency of the whole option group -
        /// Options inside an option group cannot have dependencies.
        /// </summary>
        public string UseWith { get; init; }
    }

    /// @}

}