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
    /// <summary>
    /// Base class for user parser.
    /// 
    /// To use automatic argument parsing:
    /// - Create a custom class which extends this Parser.
    /// - Annotate class properties with according <see cref="IOptionAttribute"/> attributes.
    /// - Now you can call <see cref="Parser.Parse"/> to parse arguments automatically to your class.
    ///
    /// To generate help text for your class, use the <see cref="Parser.GenerateHelp"/> method.
    /// </summary>
    public abstract class Parser
    {
        protected Parser()
        {
            metadata_ = getMetadata();
        }

        internal string ShortOptionSymbol => metadata_.Config.ShortOptionSymbol;
        internal string LongOptionSymbol => metadata_.Config.LongOptionSymbol;
        internal string DelimiterSymbol => metadata_.Config.DelimiterSymbol;
        internal string EqualsSymbol => metadata_.Config.EqualsSymbol;
        internal OptionFlags OptionFlags => metadata_.Config.OptionFlags;

        /// <summary>
        /// Count of <see cref="PlainArgs"/> will be checked against this at the end of the parsing.
        /// If not matched <see cref="PlainArgsCountException"/> is thrown.
        /// </summary>
        protected virtual int PlainArgsRequired {
            get => PlainArgs.Count;
        }

        private int skip;
        /// <summary>
        /// The amount of arguments that the parser will skip.
        /// </summary>
        protected int Skip {
            get => skip;
            set {
                if (value >= 0) {
                    skip = value;
                } else throw new ArgumentOutOfRangeException();
            }
        }

        private static readonly Dictionary<Type, ParserMetadata> typeMetadata_ = new();
        private readonly HashSet<OptionMetadata> parsedOptions_ = new();

        private string[] rawArgs_;
        private List<string> plainArgs_ = new();
        private TokenReader tokens_;
        private ParserMetadata metadata_;

        /// <summary>
        /// Indicates if parser instance was used -> holds parsed arguments
        /// </summary>
        public bool Parsed { get; private set; }

        /// <summary>
        /// List of all plain arguments
        /// </summary>
        public IReadOnlyList<string> PlainArgs {
            get => plainArgs_.AsReadOnly();
        }

        /// <summary>
        /// View on the raw arguments array starting at the currently parsed argument.
        /// </summary>
        protected Span<string> Arguments {
            get => new Span<string>(rawArgs_, tokens_.Position, rawArgs_.Length - tokens_.Position);
        }


        /// <summary>
        /// Do the actual parsing.
        /// 
        /// Parse given arguments and populate the user-defined options according to their values.      
        /// If parsing fails, <see cref="ParsingException" /> is thrown.
        /// </summary>
        /// <param name="args">List of command line arguments to parse</param>
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
                    continue;
                }

                int position = tokens_.Position;
                string rawArg = tokens_.Read();
                bool plain = delimited;

                if (!delimited) {
                    if (rawArg == DelimiterSymbol) {

                        delimited = true;

                    } else if (LongOptionSymbol == ShortOptionSymbol && rawArg.StartsWith(LongOptionSymbol)) {
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
                    } else {
                        bool isVerb = tryParseVerb(rawArg);
                        plain = !isVerb; // if not verb than plain
                    }
                }

                if (plain) {
                    plainArgs_.Add(rawArg);
                }

                Debug.Assert(tokens_.Position > position, "No tokens have been consumed.");
            }
            checkPlainArgsCount();
            metadata_.CheckRules(parsedOptions_);

        }

        private void checkPlainArgsCount()
        {
            if (PlainArgsRequired != PlainArgs.Count) {
                throw new PlainArgsCountException(
                    $"{PlainArgsRequired} plain arguments required but {PlainArgs.Count} given.");
            }
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

            if (caseInsensitive) {
                name = name.ToUpper();
            }

            return tryParse(name, value, metadata_.OptionsByLong, forbidSpace);
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

            if (name != null) {
                if (caseInsensitive) {
                    name = name.ToUpper();
                }

                return tryParse(name, value, metadata_.OptionsByShort, forbidSpace);
            }

            return false;
        }


        private bool tryParse(string argName, string value, IDictionary<string, OptionMetadata> lookup, bool forbidSpace)
        {
            if (lookup.TryGetValue(argName, out var option)) {

                if (value == null && option is ValueOption && forbidSpace) {
                    throw new MissingOptionValueException(tokens_.Peek(-1));
                }

                ParseOption(option, value);
                return true;
            }
            return false;
        }

        private bool tryParseVerb(string rawArg)
        {
            bool caseInsensitive = OptionFlags.HasFlag(OptionFlags.VerbCaseInsensitive);
            if (caseInsensitive) {
                rawArg = rawArg.ToUpper();
            }

            return tryParse(rawArg, null, metadata_.VerbOptions, false);
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

            try {
                option.Parse(this, value, tokens_);
            } catch (FormatException ex) {
                throw new ValueOptionFormatException(tokens_.Peek(-1), ex);
            }

            parsedOptions_.Add(option);
        }

        private ParserMetadata getMetadata()
        {
            var type = this.GetType();
            if (!typeMetadata_.ContainsKey(type)) {
                var metadata = new ParserMetadata(type);
                metadata.LoadAtrributes();
                return typeMetadata_[type] = metadata;
            }
            return typeMetadata_[type];
        }

        /// <summary>
        /// Generates help text for command
        /// </summary>
        /// <param name="shortHelp">Short help includes 1 line with command and options, omits help texts of options. Default short.</param>
        /// <returns>Help text as string</returns>
        public string GenerateHelp(bool shortHelp = true)
        {
            StringWriter sw = new();
            GenerateHelp(sw, shortHelp);
            return sw.ToString();
        }
        /// <summary>
        /// Generates help text for command and writes it to given TextWriter
        /// </summary>
        /// <param name="output">Output TextWriter</param>
        /// <param name="shortHelp">Short help includes 1 line with command and options, omits help texts of options. Default short.</param>
        public void GenerateHelp(TextWriter output, bool shortHelp = true)
        {
            if (shortHelp) {
                output.Write($"{metadata_.Config.CommandName}");
                var groups = metadata_.GetGroups();
                var groupOptions = groups.SelectMany(group => group.Options).ToHashSet();

                // first print non group options
                foreach (var option in metadata_.OptionsByProperty.Values) {
                    if (!groupOptions.Contains(option)) {
                        if (option.Required) {
                            output.Write($" {option.GetRawNameWithMetavar()}");
                        } else {
                            output.Write($" [{option.GetRawNameWithMetavar()}]");
                        }
                    }
                }

                // then print groups
                foreach (var group in groups) {
                    output.Write(group.Required ? " (" : " [");
                    output.Write(group);
                    output.Write(group.Required ? ")" : "]");
                }

                output.WriteLine();
            } else {
                int maxShortName = metadata_.OptionsByProperty.Values
                    .Select(o => o.ShortName != '\0' ? o.GetRawName(out _, true).Length : 0).Max();
                int maxLongName = metadata_.OptionsByProperty.Values
                    .Select(o => o.LongName != null ? o.GetRawNameWithMetavar(out _, false).Length : 0).Max();

                var allOptions = metadata_.OptionsByProperty.Values.OrderBy(o => o.GetName(out _, false));
                foreach (var option in allOptions) {
                    string shortName = option.ShortName != '\0' ? ShortOptionSymbol + option.ShortName : "";
                    string longName = option.LongName != null ? option.GetRawNameWithMetavar(out _, false) : "";

                    output.Write("    ");
                    output.Write(shortName.PadLeft(maxShortName));

                    output.Write(shortName != "" ? ", " : "  ");
                    output.Write(longName.PadRight(maxLongName));
                    output.Write(" ");

                    List<string> rawHelpText = new();
                    rawHelpText.Add(option.HelpText);
                    if (option.Required && option.UseWith == null) {
                        rawHelpText.Add("This option is required.");
                    } else if (!option.Required && option.UseWith != null) {
                        rawHelpText.Add($"Use only with {option.UseWith.GetRawName()}.");
                    } else if (option.Required && option.UseWith != null) {
                        rawHelpText.Add($"When {option} used, this must be also specified.");
                    }

                    int maxLineLength = 40, i = 0;
                    string newlinePad = "\n" + new string(' ', 6 + maxLongName + maxShortName);
                    var words = rawHelpText
                        .SelectMany(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                        .SelectMany(s => {
                            var split = s.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < split.Length - 1; i++)
                                split[i] += "\n";
                            return split;
                        });

                    foreach (string word in words) {
                        string space = i == 0 ? "" : " ";

                        if (i != 0 && i + word.Trim(' ', '\n').Length - 1 + space.Length > maxLineLength) {
                            i = 0;
                            output.Write(newlinePad);
                        }
                        output.Write(space + word);

                        if (!word.Contains('\n')) {
                            i += word.Length + space.Length;
                        } else {
                            i = 0;
                        }
                    }

                    output.WriteLine();
                }
            }
        }
    }
}
