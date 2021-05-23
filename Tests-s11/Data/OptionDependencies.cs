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

        [ValueOption("verbose", shortName: 'v', help: "Print more details", useWith: nameof(Print))]
        public bool Verbose { get; set; }
    }
}
