using CShargs;

namespace Tests.Data
{
    class OptionDependenciesArguments : Parser
    {
        [FlagOption("print", shortName: 'p', help: "Print out progress")]
        public bool Print { get; set; }

        [FlagOption("verbose", shortName: 'v', help: "Print more details", useWith: nameof(Print))]
        public bool Verbose { get; set; }
    }

    class RequiredDependenciesArguments : Parser
    {

        [FlagOption("print", shortName: 'p', help: "Print out progress")]
        public bool Print { get; set; }

        [ValueOption("out", required: true, shortName: 'o', help: "Print output", useWith: nameof(Print))]
        public string Output { get; set; }
    }

    [OptionGroup(true, nameof(ToFile), nameof(ToStdOut), nameof(ToStdErr), useWith = nameof(Print))]
    class RequiredGroupDependenciesArguments : Parser
    {

        [FlagOption("print", shortName: 'p', help: "Print out progress")]
        public bool Print { get; set; }

        [ValueOption("tofile")]
        public string ToFile { get; set; }
        
        [FlagOption("tostdout")]
        public bool ToStdOut { get; set; }

        [FlagOption("tostderr")]
        public bool ToStdErr { get; set; }
    }
}
