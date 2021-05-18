using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenReader = ListReader<string>;

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

        private TokenReader tokens_;

        /// <summary>
        /// Do the actual parsing.
        ///
        /// If parsing fails, <see cref="" />
        /// </summary>
        public void Parse(string[] args) {

            tokens_ = new(args);

        }

        public string GenerateHelp()
        {
            StringWriter sw = new();
            GenerateHelp(sw);
            return sw.ToString();
        }
        public void GenerateHelp(TextWriter output) { }

        public IReadOnlyList<string> PlainArgs => plainArgs_.AsReadOnly();
        private List<string> plainArgs_;


        /// <summary>
        /// Count of <see cref="PlainArgs"/> will be checked against this at the end of the parsing.
        /// </summary>
        protected virtual int PlainArgsRequired => PlainArgs.Count;

        private int skip;
        /// <summary>
        /// The parser will skip next n arguments.
        /// </summary>
        protected int Skip {
            get => skip;
            set {
                if (value >= 0) {
                    skip = value;
                } else throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// View on the raw arguments array starting at the currently parsed argument
        /// </summary>
        protected ArraySegment<string> Arguments => throw new NotImplementedException();


    }
}
