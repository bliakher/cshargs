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
            string alias = null,
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
            string alias = null,
            string useWith = null,
            string help = null)
        { }

        public ValueOptionAttribute(
            string name,
            string alias = null,
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
          string[] aliasOf)
        { }

        public AliasOptionAttribute(
          string name,
          string aliasOf) : this(name, new string[] { aliasOf })
        { }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    class CustomOptionAttribute : Attribute
    {
        public CustomOptionAttribute(
            string name,
            bool required,
            Action<string> callback,
            string alias = null,
            string useWith = null,
            string help = null)
        { }
    }

}