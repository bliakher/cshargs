using System;
using System.Collections.Generic;
using System.IO;

namespace CShargs
{

    abstract class Parser
    {
        public void Parse(string[] args)
        {
            int cursor = 0;

            while (cursor < args.Length) {

                if (skip > 0) {
                    skip--;
                    cursor++;
                    continue;
                }

                // do parsing
                cursor++;
            }
        }

        public string GenerateHelp()
        {
            StringWriter sw = new();
            GenerateHelp(sw);
            return sw.ToString();
        }
        public void GenerateHelp(TextWriter output) { }

        public IReadOnlyList<string> PlainArgs { get; set; }

        protected virtual int PlainArgsRequired => PlainArgs.Count;

        /// <summary>
        /// The parser will skip next n arguments.
        /// </summary>
        /// <param name="n"></param>
        protected int Skip {
            get => skip;
            set {
                if (value >= 0) {
                    skip = value;
                } else throw new ArgumentOutOfRangeException();
            }
        }

        private int skip;

        /// <summary>
        /// Called by the parser when an unknown parameter is encountered.
        /// User handles this state, and returns whether the handling was sucessfull.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>If false returned, the parser will go to error state.</returns>
        protected virtual bool OnUnknownParameter(string param)
        {
            return false;
        }
    }

    class FlagOptionAttribute : Attribute
    {
        public FlagOptionAttribute(
            string name,
            string alias = null,
            string help = null)
        { }
    }

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

    class VerbOptionAttribute : Attribute
    {
        public VerbOptionAttribute(
            string name,
            string help = null
        )
        { }
    }

}