using System;

namespace CShargs.Examples
{

    [ParserSettings1(ParserConst.LONG_USE_EQUAL | ParserConst.ALLOW_MERGE)]
    class TimeArguments : Parser {

        [FlagOption("p", alias: "portability", help: "Use the portable output format.")]
        public bool Portable { get; set; }
        
        [FlagOption("a", alias: "append", help: "(Used together with -o.) Do not overwrite but append.")]
        public bool Append { get; set; }
        
        [FlagOption("v", alias: "verbose", help: "Give very verbose output about all the program knows about.")]
        public bool Verbose { get; set; }
        
        [FlagOption("V", alias:"verison", help:"Print version information on standard output, then exit successfully.")]
        public bool Version { get; set; }

        [ValueOption("f", alias: "format", required: false, help: "Specify output format, possibly overriding the format specified in the environment variable TIME.")]
        public string Format { get; set; }
        
        [ValueOption("o", alias:"output", required: false, help: "Do not send the results to stderr, but overwrite the specified file.")]
        public string OutputFile { get; set; }

    }

    static class ParserConst
    {
        // now: space is default, equal sign can be set in settings
        //      separately for short and long options
        
        public const int SHORT_USE_EQUAL = 1;
        public const int LONG_USE_EQUAL = 2;
        public const int ALLOW_MERGE = 4;
    }
    
    class ParserSettings1Attribute : Attribute
    {

        public ParserSettings1Attribute(int settingsFilter)
        {
            
        }
    }


}