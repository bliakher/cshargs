using CShargs;

namespace Tests.Data
{
    class FlagArguments : Parser
    {
        [FlagOption("verbose", shortName: 'v', help: "Give very verbose output about all the program knows about.")]
        public bool Verbose { get; set; }
    }
    class FixedCountPlainArguments : Parser
    {
        [FlagOption("verbose", shortName: 'v', help: "Give very verbose output about all the program knows about.")]
        public bool Verbose { get; set; }

        protected override int PlainArgsRequired => 3;
    }

    class SkippedArguments : Parser
    {
        public SkippedArguments(int argumentsToSkip)
        {
            Skip = argumentsToSkip;
        }
        [FlagOption("verbose", shortName: 'v', help: "Give very verbose output about all the program knows about.")]
        public bool Verbose { get; set; }
    }

    [AliasOption("R", nameof(Recursive))]
    [AliasOption("a", nameof(Recursive), nameof(Force))]
    class AliasFlagArguments : Parser
    {
        [FlagOption("recursive", shortName: 'r')]
        public bool Recursive { get; set; }

        [FlagOption("force", shortName: 'f')]
        public bool Force { get; set; }
    }
}
