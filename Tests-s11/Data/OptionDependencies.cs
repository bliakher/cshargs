using CShargs;

namespace Tests.Data
{
    class OptionDependenciesArguments : Parser
    {
        [FlagOption("print", shortName: 'p', help: "Print out progress")]
        bool Print { get; set; }

        [FlagOption("verbose", shortName: 'v', help: "Print more details", useWith: nameof(Print))]
        bool Verbose { get; set; }
    }
}
