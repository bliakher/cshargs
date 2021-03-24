
# CShargs (🌊🦈) argument parser
- Easy to use declarative argument parser.
- Define a class representing the arguments object. 🌊🦈 will populate this object with data from the command line.

# Usage
- Create a class that represents arguments of your command (must be child of Parser class)
- Properties in the class corespond to command options. Annotate them with attributes to specify kinds of options.
- Parameters of an option are specified in attribute constructor.
- Then create an instance of this object and pass command line arguments to it when you want to parse them.

``` CSharp
class MyCommandArguments : CShargs.Parser {
    // ... options go here
}

static void Main(string[] argv) {
    var arguments = new MyCommandArguments();
    arguments.Parse(argv);
}
```

## Flag option ( `-f` )
- Flag opiton is used for options without parameters
- For option name, use the first constructor argument `name`
- For alias, use the named argument `alias`
- For help description, use the named argument `help`
``` CSharp
    [FlagOption("silent", alias: "s", help: "No output will be produced to stdout.")]
    bool Silent { get; set; }
    // set to true when `-s` or `--silent` is used
```
- You can set additional rules by annotating the class with `ParserSettings` attribute


## Value option ( `--key Value` / `--key=Value` / `-k Value` / `-kValue` )
- Value option attribute is used for options with parameters, eg
``` CSharp
    [ValueOption("number-of-cats", alias: "c", required: false)]
    int Cats { get; set; } = 12;
    // set with --number-of-cats=123 / --number-of-cats 123 / -c 123 / -c123
```
- Value option has `required` argument. If the option is not used on the command line, one of the following will happen:
    - If `required` not specified, the option is not considered required iff its type is marked as nullable (ie `int?`).
    - If `required: false`, the default value of the property is used
    - If `required: true` and option is not used -> error

## Verb option ( `git push` )
- VerbOption attribute is used for subcommands
``` CSharp
    [VerbOption("push")]
    PushArguments Push { get; set; }
    //

...
class PushArguments : Parser {

}
```

## Plain arguments


## Global rules
- You can set attributes of class for global rules regarding options:
    - Setting synonyms for options, creating aliases for a group of options
    - Setting a select group of options
    - ParserSettings - configuration of option parsing
        - using equal sign or space eg. --format=FORMAT or --format FORMAT
        - enabling option merging eg. -aSt instead of -a -S -t
- Types of option properties must be either
    - one of the C# primitives (`int`, `string`, `bool`, `short`, `long`, etc.)
    - an enum.
    - any user defined type `T`, which provides static method
```CSharp
        public static T Parse(string str);
```

## Parser class
+ Parse(string args) : void
+ GenerateHelp() : string
- PlainArgs - array of strings ?
+ GetPlainArgs() ?


## Considerations
- not present value options - value is null - all types nullable ? no
- nullable desiding optional x mandatory option ?
- default values ? yes, not nullable, but required:false

## ToDo:
- Parser class
- Option attriubute classes
- ParserSettings attribute
- global settings attributes - synonyms etc
- write `time` command example


``` CSharp
    [ValueOption("number-of-cats", alias:"c", required:false)]
    int Cats { get; set; }
// --number-of-cats      .... defaults to 0


    [ValueOption("number-of-cats", alias:"c", required:false)]
    int? Cats { get; set; } = 12;


```