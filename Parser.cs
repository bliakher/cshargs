using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenReader = CShargs.ListReader<string>;

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

        private static readonly Dictionary<Type, ParserMetadata> typeMetadata_ = new();
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
        /// If parsing fails, <see cref="ParsingException" /> is thrown
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
                string rawArg = tokens_.Read();
                bool plain = delimited;

                if (!delimited) {
                    if (rawArg == DelimiterSymbol) {

                        delimited = plain = true;

                    } else if (LongOptionSymbol == ShortOptionSymbol) {
                        if (!tryParseLong(rawArg) && !tryParseShort(rawArg)) {
                            throw new UnknownOptionException(rawArg);
                        }

                    } else if (rawArg.StartsWith(LongOptionSymbol)) {
                        if (!tryParseLong(rawArg)) {
                            throw new UnknownOptionException(rawArg);
                        }

                    } else if (rawArg.StartsWith(ShortOptionSymbol)) {
                        if (rawArg.Length - ShortOptionSymbol.Length > 1) {
                            // more options aggregated OR value follows

                            // first attempt to interpret string as flag aggregation
                            bool aggregated = false;
                            if (!OptionFlags.HasFlag(OptionFlags.ForbidAggregated)) {

                                // when first char is flag option, we can be sure that it is an aggregation
                                // therefore it can throw so we get more detailed error messages
                                if (isFirstCharShortFlagOption(rawArg)) {
                                    aggregated = true;
                                    parseAggregated(rawArg);
                                }
                            }

                            // if that fails, interpret as key-value option
                            if (!aggregated && !tryParseShort(rawArg)) {
                                throw new UnknownOptionException(rawArg);
                            }

                        } else if (!tryParseShort(rawArg)) { // only one short option
                            throw new UnknownOptionException(rawArg);
                        }
                    }
                }

                if (plain) {
                    plainArgs_.Add(tokens_.Read());
                }

                Debug.Assert(tokens_.Position > position, "No tokens have been consumed.");
            }

            metadata_.CheckRules(parsedOptions_);

        }

        private bool isFirstCharShortFlagOption(string rawArg)
        {
            char first = rawArg[ShortOptionSymbol.Length];
            return metadata_.OptionsByShort.TryGetValue(first.ToString(), out var option) && isTargetFlagOption(option);
        }

        private bool isTargetFlagOption(OptionMetadata option)
        {
            var targetOptionType = (option is AliasOption aliasOp
                    ? aliasOp.Targets.First()
                    : option).GetType();
            return targetOptionType == typeof(FlagOption);
        }

        private bool tryParseLong(string rawArg)
        {
            bool forbidEquals = OptionFlags.HasFlag(OptionFlags.ForbidLongEquals);
            bool forbidSpace = OptionFlags.HasFlag(OptionFlags.ForbidLongSpace);
            bool caseInsensitive = OptionFlags.HasFlag(OptionFlags.LongCaseInsensitive);

            int start = LongOptionSymbol.Length;
            int eqIdx = rawArg.IndexOf(EqualsSymbol);
            string name, value = null;
            if (rawArg.Contains(EqualsSymbol) && !forbidEquals) {
                name = rawArg.Substring(start, eqIdx - start);
                value = rawArg.Substring(eqIdx + EqualsSymbol.Length);
            } else {
                name = rawArg.Substring(start);
                value = null;
            }

            if (value == null && forbidSpace) {
                throw new MissingOptionValueException(rawArg);
            }

            if (caseInsensitive) {
                name = name.ToUpper();
            }

            return tryParse(name, value, metadata_.OptionsByLong);
        }

        private bool tryParseShort(string rawArg)
        {
            // here assume that rawArg is not an aggregation

            bool forbidEquals = OptionFlags.HasFlag(OptionFlags.ForbidShortEquals);
            bool forbidNospace = OptionFlags.HasFlag(OptionFlags.ForbidShortNoSpace);
            bool caseInsensitive = OptionFlags.HasFlag(OptionFlags.ShortCaseInsensitive);
            bool forbidSpace = OptionFlags.HasFlag(OptionFlags.ForbidShortSpace);

            int start = ShortOptionSymbol.Length;
            int eqIdx = rawArg.IndexOf(EqualsSymbol);
            string name = null, value = null;
            if (rawArg.Length - ShortOptionSymbol.Length == 1) {
                name = rawArg.Substring(start);
                value = null;

            } else if (rawArg.Contains(EqualsSymbol) && !forbidEquals) {
                name = rawArg.Substring(start, eqIdx - start);
                value = rawArg.Substring(eqIdx + EqualsSymbol.Length);

            } else if (!forbidNospace) {
                name = rawArg.Substring(start, 1);
                value = rawArg.Substring(start + 1);
            }

            if (value == null && forbidSpace) {
                throw new MissingOptionValueException(rawArg);
            }

            if (name != null) {
                if (caseInsensitive) {
                    name = name.ToUpper();
                }

                return tryParse(name, value, metadata_.OptionsByShort);
            }

            return false;
        }


        private bool tryParse(string argName, string value, IDictionary<string, OptionMetadata> lookup)
        {
            if (lookup.TryGetValue(argName, out var option)) {
                ParseOption(option, value);
                return true;
            }
            return false;
        }

        private void parseAggregated(string rawArg)
        {
            var lookup = metadata_.OptionsByShort;

            // remove option symbol from the start
            var names = rawArg.Substring(ShortOptionSymbol.Length, rawArg.Length - ShortOptionSymbol.Length);

            foreach (char name in names) {
                if (lookup.TryGetValue(name.ToString(), out var option)) {

                    if (!isTargetFlagOption(option)) {
                        throw new OptionAggregationException("Only flags can be aggregated");
                    }

                    ParseOption(option, null);
                } else {
                    throw new UnknownOptionException(name.ToString());
                }
            }
        }

        internal void ParseOption(OptionMetadata option, string value)
        {
            if (option.GetType() != typeof(FlagOption) && parsedOptions_.Contains(option)) {
                throw new DuplicateOptionException(tokens_.Peek(-1));
            }

            option.Parse(this, value, tokens_);
            parsedOptions_.Add(option);
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
