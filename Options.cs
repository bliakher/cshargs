using System;
using System.Collections.Generic;
using System.IO;

namespace CShargs
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    class FlagOptionAttribute : Attribute
    {
        public FlagOptionAttribute(
            string name,
            char shortName = '\0',
            string useWith = null,
            string help = null)
        { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    class ValueOptionAttribute : Attribute
    {
        public ValueOptionAttribute(
            string name,
            bool required,
            char shortName = '\0',
            string useWith = null,
            string help = null)
        { }

        public ValueOptionAttribute(
            string name,
            char shortName = '\0',
            string useWith = null,
            string help = null)
        { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    class VerbOptionAttribute : Attribute
    {
        public VerbOptionAttribute(
            string name,
            string help = null)
        { }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    class AliasOptionAttribute : Attribute
    {
        public AliasOptionAttribute(
            string name,
            params string[] aliasOf)
        { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    class CustomOptionAttribute : Attribute
    {
        public CustomOptionAttribute(
            string name,
            bool required,
            char shortName = '\0',
            string useWith = null,
            string help = null)
        { }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    class OptionGroupAttribute : Attribute
    {
        public OptionGroupAttribute(
            bool required,
            params string[] optionGroup)
        { }
    }

}