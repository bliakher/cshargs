using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace CShargs {
    interface IOptionAttribute {
        string Name { get; }
        char ShortName { get; }
        bool Required { get; }
        string UseWith { get; }
        string HelpText { get; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FlagOptionAttribute : Attribute, IOptionAttribute {
        public FlagOptionAttribute(
            string name,
            char shortName = '\0',
            string useWith = null,
            string help = null) {

            Name = name;
            ShortName = shortName;
            UseWith = useWith;
        }

        public string Name { get; private init; }
        public char ShortName { get; private init; }
        public bool Required => false;
        public string HelpText { get; private init; }
        public string UseWith { get; private init; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ValueOptionAttribute : Attribute, IOptionAttribute {

        public ValueOptionAttribute(
            string name,
            bool required,
            char shortName = '\0',
            string useWith = null,
            string help = null) {

            Name = name;
            Required = required;
            ShortName = shortName;
            UseWith = useWith;
            HelpText = help;
        }

        public string Name { get; private init; }
        public char ShortName { get; private init; }
        public bool Required { get; private init; }
        public string HelpText { get; private init; }
        public string UseWith { get; private init; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class VerbOptionAttribute : Attribute, IOptionAttribute {
        public VerbOptionAttribute(
            string name,
            string help = null) {

            Name = name;
            HelpText = help;
        }

        public string Name { get; private init; }
        public char ShortName => '\0';
        public bool Required => false;
        public string HelpText { get; private init; }
        public string UseWith => null;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AliasOptionAttribute : Attribute {
        public AliasOptionAttribute(
            string name,
            params string[] aliasOf) { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CustomOptionAttribute : Attribute, IOptionAttribute {
        public CustomOptionAttribute(
            string name,
            bool required,
            char shortName = '\0',
            string useWith = null,
            string help = null) {

            Name = name;
            Required = required;
            ShortName = shortName;
            UseWith = useWith;
        }


        public string Name { get; private init; }
        public char ShortName { get; private init; }
        public bool Required { get; private init; }
        public string HelpText { get; private init; }
        public string UseWith { get; private init; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class OptionGroupAttribute : Attribute {
        public OptionGroupAttribute(
            bool required,
            params string[] optionGroup) {
        }
    }

}