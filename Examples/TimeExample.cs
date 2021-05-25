using System;

namespace CShargs.Examples
{
    
/*
 * time [options] command [arguments...]
 *
 * GNU Options
 *     -f FORMAT, --format=FORMAT
 *            Specify output format, possibly overriding the format specified
 *            in the environment variable TIME.
 *     -p, --portability
 *            Use the portable output format.
 *     -o FILE, --output=FILE
 *            Do not send the results to stderr, but overwrite the specified file.
 *     -a, --append
 *            (Used together with -o.) Do not overwrite but append.
 *     -v, --verbose
 *            Give very verbose output about all the program knows about.
 * 
 * GNU Standard Options
 *     --help Print a usage message on standard output and exit successfully.
 *     -V, --version
 *            Print version information on standard output, then exit successfully.
 *     --     Terminate option list.
 *
 */
    
    // sets one alias -u for -p -v
    [AliasOption("u", nameof(Portable), nameof(Verbose))]
    // sets command name to 'time'
    // limits syntax of value options - only space for short options and equal sign for long eg. -f FORMAT, --format=FORMAT
    // all other settings are left at default
    [ParserConfig("time", OptionFlags.ForbidShortEquals | OptionFlags.ForbidLongSpace)]
    internal class TimeArguments : Parser 
    {
        // options without parameters - flags
        // their type must be bool - meaning: present x not present in args

        [FlagOption("portability", shortName: 'p' , help: "Use the portable output format.")]
        public bool Portable { get; set; }

        // useWith: -a can be used only if -o present
        [FlagOption("append", shortName:'a' , useWith: nameof(OutputFile), help: "(Used together with -o.) Do not overwrite but append.")]
        public bool Append { get; set; }

        [FlagOption("verbose", shortName: 'v' , help: "Give very verbose output about all the program knows about.")]
        public bool Verbose { get; set; }

        [FlagOption("version", shortName: 'V', help:"Print version information on standard output, then exit successfully.")]
        public bool Version { get; set; }

        [FlagOption("help", help: "Print a usage message on standard output and exit successfully.")]
        public bool Help {get; set;}

        // options with parameters - value options
        // type according to option parameter type

        [ValueOption("format", shortName:'f', required: false, help: "Specify output format, possibly overriding the format specified in the environment variable TIME.")]
        public string Format { get; set; }

        [ValueOption("output", shortName:'o', required: false, help: "Do not send the results to stderr, but overwrite the specified file.")]
        public string OutputFile { get; set; }

    }
}