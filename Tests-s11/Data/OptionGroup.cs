using CShargs;

namespace Tests.Data
{
    [OptionGroup(required: true, nameof(Words), nameof(Lines))]
    class CountGroupArguments : Parser
    {
        [FlagOption("words", shortName: 'w')]
        public bool Words { get; set; }

        [FlagOption("lines", shortName: 'l')]
        public bool Lines { get; set; }

        [FlagOption("verbose", shortName: 'v', help: "Give very verbose output about all the program knows about.")]
        public bool Verbose { get; set; }
    }

    [OptionGroup(required: true, nameof(InputFile), nameof(OutputFile))]
    class InvalidConfigurationGroupArguments : Parser
    {
        [ValueOption("input", shortName: 'i', required: true, help: "Do not read from stdin, but from the specified file.")]
        public string InputFile { get; set; }

        [ValueOption("output", shortName: 'o', help: "Do not send the results to stderr, but overwrite the specified file.")]
        public string OutputFile { get; set; }
    }


    [OptionGroup(false, nameof(ToFile), nameof(ToStdErr), UseWith = nameof(Print))]
    class GroupDependenciesArguments : Parser
    {

        [FlagOption("print", shortName: 'p', help: "Print out progress")]
        public bool Print { get; set; }

        [ValueOption("tofile")]
        public string ToFile { get; set; }

        [FlagOption("tostderr")]
        public bool ToStdErr { get; set; }
    }
}
