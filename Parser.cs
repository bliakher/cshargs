using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenReader = CShargs.ListReader<string>;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("s14-api-testing")]
namespace CShargs
{
    public abstract class Parser : IParserConfig
    {
        public Parser()
        {
            metadata_ = getMetadata();
        }

        public string ShortOptionSymbol => metadata_.Config.ShortOptionSymbol;
        public string LongOptionSymbol => metadata_.Config.LongOptionSymbol;
        public string DelimiterSymbol => metadata_.Config.DelimiterSymbol;
        public string EqualsSymbol => metadata_.Config.EqualsSymbol;
        public OptionFlags OptionFlags => metadata_.Config.OptionFlags;

        private readonly static Dictionary<Type, ParserMetadata> typeMetadata_ = new();
        private readonly HashSet<OptionMetadata> parsedOptions_ = new();

        private string[] rawArgs_;
        private List<string> plainArgs_ = new();
        private TokenReader tokens_;
        private ParserMetadata metadata_;

        public bool Parsed { get; private set; }

        /// <summary>
        /// List of all plain arguments
        /// </summary>
        public IReadOnlyList<string> PlainArgs => plainArgs_.AsReadOnly();

        /// <summary>
        /// View on the raw arguments array starting at the currently parsed argument
        /// </summary>
        protected ArraySegment<string> Arguments => new ArraySegment<string>(rawArgs_, tokens_.Position, rawArgs_.Length - tokens_.Position);


        /// <summary>
        /// Do the actual parsing.
        ///
        /// If parsing fails, <see cref="" />
        /// </summary>
        public void Parse(string[] args)
        {
            ThrowIf.ArgumentNull(nameof(args), args);
            ThrowIf.InState(Parsed == true);

            rawArgs_ = args;
            tokens_ = new TokenReader(args);

            bool delimited = false;

            while (!tokens_.EndOfList) {
                if (skip > 0) {
                    skip--;
                    tokens_.Read();
                }

                int position = tokens_.Position;
                string rawArg = tokens_.Peek();
                bool plain = delimited;

                if (!delimited) {
                    if (rawArg == DelimiterSymbol) {

                        delimited = plain = true;

                    } else if (rawArg.StartsWith(LongOptionSymbol)) {

                        if (!tryParseLong(rawArg) && LongOptionSymbol == ShortOptionSymbol) {
                            TryParseShort(rawArg);
                        }

                    } else if (rawArg.StartsWith(ShortOptionSymbol)) {

                        TryParseShort(rawArg);
                    }
                }

                if (plain) {
                    plainArgs_.Add(tokens_.Read());
                }

                Debug.Assert(tokens_.Position > position, "No tokens have been consumed.");
            }

            metadata_.CheckRules(parsedOptions_);

        }

        private bool tryParseLong(string rawArg)
        {
            int eqIdx = 0;
            string name;
            if (Settings.HasFlag(OptionFlags.ForbidLongEquals) || (eqIdx = rawArg.IndexOf(EqualsSymbol)) == -1) {
                name = rawArg.Substring(LongOptionSymbol.Length);
            } else {
                name = rawArg.Substring(LongOptionSymbol.Length, rawArg.Length - LongOptionSymbol.Length - eqIdx);
            }

            if (metadata_.OptionsByLong.TryGetValue(name, out var option)) {

                option.ParseAndSet(this, tokens_);
                return true;
            } else {
                return false;
            }
        }

        private bool TryParseShort(string rawArg)
        {
            return false; //ToDo
        }

        private ParserMetadata getMetadata()
        {
            var type = this.GetType();
            if (!typeMetadata_.ContainsKey(type)) {
                var metadata = new ParserMetadata(type);
                metadata.LoadAtrributes();
                typeMetadata_[type] = metadata;
            }
            return typeMetadata_[type];
        }

        public string GenerateHelp()
        {
            StringWriter sw = new();
            GenerateHelp(sw);
            return sw.ToString();
        }
        public void GenerateHelp(TextWriter output) { }

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
    }
}
