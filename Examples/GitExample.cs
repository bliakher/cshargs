using System;

namespace CShargs.Examples
{
    //[AliasOption("v", equivalentTo="Ut")]
    class GitArguments : Parser {

        [FlagOption("version")]
        bool showVersion;

        [VerbOption("push")]
        GitPushArguments pushArguments;
    }

    class GitPushArguments : Parser {

    }

}