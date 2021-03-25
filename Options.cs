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
            string help = null)
        { }

        public ValueOptionAttribute(
            string name,
            string alias = null,
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
          string aliasOf)
        { }
    }

}