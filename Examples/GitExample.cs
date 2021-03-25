using System;

namespace CShargs.Examples
{

    class GitArguments : Parser {

        [FlagOption("version")] 
        private bool showVersion { get; set; }

        [VerbOption("push")]
        private GitPushArguments pushArguments { get; set; }
    }

    class GitPushArguments : Parser {

    }

}