using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShargs
{
    abstract class Parser
    {
        public Parser(
            OptionSettings optionSettings = OptionSettings.Default,
            string flagOptionSymbol = "-",
            string valueOptionSymbol = "--",
            string equalsSymbol = "="
            ) { }

        public Parser() { }
        public void Parse(string[] args) { }

        public string GenerateHelp()
        {
            StringWriter sw = new();
            GenerateHelp(sw);
            return sw.ToString();
        }
        public void GenerateHelp(TextWriter output) { }

        public IReadOnlyList<string> PlainArgs { get; set; }


        /// <summary>
        /// Count of <see cref="Parser.PlainArgs"/> will be checked against this at the end of the parsing.
        /// </summary>
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
}
