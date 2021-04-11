using System;

namespace CShargs.Examples
{

    class GitArguments : Parser {

        [FlagOption("version", shortName: 'v', help: "Display version.")] 
        private bool showVersion { get; set; }

        [VerbOption("push", help: "Push changes to origin.")]
        private GitPushArguments pushArguments { get; set; }
        
        // git push --force
    }

    class GitPushArguments : Parser {
        [FlagOption("force", shortName:'f', help:"Force push.")]
        private bool Force { get; set; }
    }

}