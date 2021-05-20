using CShargs;

namespace Tests.Data
{
    class CommandArguments : Parser
    {
        [VerbOption("push")]
        public SubcommandFlagArguments Push { get; set; }

        [VerbOption("process")]
        public SubcommandValueOptionArguments Process { get; set; }
    }

    class SubcommandFlagArguments : Parser
    {
        [FlagOption("force")]
        public bool Force { get; set; }
    }

    class SubcommandValueOptionArguments : Parser
    {
        [ValueOption("output", shortName: 'o')]
        public string OutputFile { get; set; }
    }
}
