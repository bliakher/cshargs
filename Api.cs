using System;
using System.IO;

namespace CShargs {

    class Parser {
        public void Parse(string[] args) {

        }

        public string GenerateHelp() {
            StringWriter sw = new();
            GenerateHelp(sw);
            return sw.ToString();
        }
        public void GenerateHelp(TextWriter output) {  }
    }

    class FlagOptionAttribute : Attribute {
        public FlagOptionAttribute(
            string name,
            string alias = null,
            string help = null) { }
    }

    class ValueOptionAttribute : Attribute {

        public ValueOptionAttribute(
            string name,
            bool required,
            string alias = null,
            string help = null) { }

        public ValueOptionAttribute(
            string name,
            string alias = null,
            string help = null) { }
    }

    class VerbOptionAttribute : Attribute {
        public VerbOptionAttribute(
            string name,
            string help = null
        ) { }
    }

}