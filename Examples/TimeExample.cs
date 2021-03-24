using System;

namespace CShargs.Examples
{
    //[AliasOption("v", equivalentTo="Ut")]
    class TimeArguments : Parser {

        [ValueOption("f", alias: "format", help: "Specify output format, possibly overriding the format specified in the environment variable TIME.")]
        public string Format { get; set; }

    }


}