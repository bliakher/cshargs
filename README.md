# README
# 🌊🦈 CShargs argument parser

(Hopefully) easy to use declarative argument parser for C#.

Define a class representing the arguments object. 🌊🦈 will populate this object with data from the command line.

**Table of contents:**

[[_TOC_]]



***

# Build

Before build you will need to install
- dotnet cli
- make
- for generating documentation:
  - doxygen
  - graphviz
  
Run `make build` to build the library.

Run `make test` to run tests.

Run `make docs` to generate reference documentation.


# Usage


- Create a class that represents arguments of your command (must be child of Parser class)
- Properties or methods in the class correspond to command options. Annotate them with attributes to specify kinds of options.
- Parameters of an option are specified in attribute constructor.
- To parse arguments, create an instance of this object and call the Parse() function, passing command line arguments to it.

```c#
class MyCommandArguments : CShargs.Parser {
    // ... options go here
}

static void Main(string[] argv) {
    var arguments = new MyCommandArguments();
    arguments.Parse(argv);
}
```
- Parsed options and their parameters will be filled in the fields of the class instance.
- Plain arguments can be found in PlainArgs property as a list of strings.

## Error handling

When parsing fails, appropriate `ParsingException` is thrown. It contains autogenerated error message. See `Exceptions.cs` for details.
If you implement custom option/type, you can throw `FormatException` to signal to the parser that the parsing has failed.

If your parser configuration contains a conflict, `ConfigurationException` is thrown at parser initialisation time.

## General remarks
- To use option attributes, the member of your class must be a property.
- There can be only one option attribute for one property.


# Features

## Flag option ( --flag / -f )
Flag option attribute is used for options without parameters. The type of the property must be bool.
- For option name, use the first constructor argument `name`
- For short alias, use the named argument `shortName`
- For option that can be used only with some other option use the named argument `useWith` (see [Option dependencies]())
- For help description, use the named argument `help`
```c#
    [FlagOption("silent", shortName: 's', help: "No output will be produced to stdout.")]
    bool Silent { get; set; }
    // set to true when `-s` or `--silent` is used
```


## Value option ( --key=Value / -k Value / -kValue )
Value option attribute is used for options with parameters. Type of the option property must be either:
  - one of the C# primitives (`int`, `string`, `bool`, `short`, `long`, etc.)
  - an enum. Entries are case sensitive, if LongCaseInsensitive setting is not set.
  - any user defined type `T`, which provides static method `T Parse(string str);`
```c#
    [ValueOption("number-of-cats", shortName: 'c', required: false)]
    int Cats { get; set; } = 12;
    // set with --number-of-cats=123 / --number-of-cats 123 / -c 123 / -c123
```
Value option has `required` argument. Use this to tell the parser to automatically raise exception when the argument is missing.

- If `required` not specified, it is inferred from the used type. (`Nullable<T>` &Rightarrow; not required)
- If `required: false`, the default value of the property is used.
- If `required: true`, missing option throws an exception.
- For alias, use the named argument `alias`
- For option that can be used only with some other option use the named argument `useWith` (see [Option dependencies]())
- For help description, use the named argument `help`




## Verb option ( `git push` )
Verb option attribute is used for subcommands. In this case, type of the option property must be a child of the `Parser` class.
```c#
// example: git push --force

class GitArguments : Parser {
    [VerbOption("push")]
    GitPushArguments Push { get; set; }
}

class GitPushArguments : Parser {
    [FlagOption("force")]
    bool Force { get; set; }
}
```

## Custom option

If you need an option that needs some context during its own parsing, or you need to interpret the raw arguments in sligtly different way, you can use the `CustomOption` attribute on a method. 
The signature of the method should be `void MyMethod(string value)`.

If the custom parameter has a value attached to it (like this: `--param=value`), it is given via the `string value` parameter of the method. Otherwise the parameter is `null`.

```c#
// example: --pos <x> <y> <z>
// note: here we assume that "=" in value options are disabled via OptionSettings.ForbidLongEquals
class VectorArguments : Parser {
    Vector3 position;
    [CustomOption("pos", true, help: "Position of the item.")]
    void ParsePosition(string value) {
        if (Arguments.Count <= 3) {
            throw new FormatException("Not enough arguments");
        }

        Skip = 3;

        position = new Vector3(
            int.Parse(Arguments[1]),
            int.Parse(Arguments[2]),
            int.Parse(Arguments[3])
        );
    }
}
```

## Settings

You can change the default parser configuration by annotating your parser class with `ParserConfigAttribute`.
Use arguments of attribute constructor:
- `commandName` (default 'command') name of your command 
- `shortOptionSymbol` (default '-') symbol with which short options are denoted 
- `longOptionSymbol` (default '--') symbol with which long options are denoted (length is greater than or equal to short symbol length)
- `delimiterSymbol` (default '--') symbol that separates plain arguments from options
- `equalsSymbol` (default '=') symbol used with value options: -n=5 --number-of-cats=5
- `optionFlags` variety of parser configurations from `OptionFlags` enum
  - default - case sensitive and all syntax variants allowed
    - aggregating short options ie. -abc / -a -b -c
    - syntax of option parameters ie. -c 123 / -c123 / -c=123
  - use flags to forbid unwanted syntax variants

```c#
[ParserConfig("time", OptionFlags.ForbidShortEquals | OptionFlags.ForbidLongSpace, shortOptionSymbol: "/", longOptionSymbol: "/")]
    class TimeArguments : Parser 
    {
       // ...
    }
```

## Alias options
Each option allows you to define one mandatory name and one optional short alias.

In addition to that you can set more aliases using `AliasOptionAttribute`, like this:
```c#
[AliasOption("R", nameof(Recursive))]
[AliasOption("a", nameof(Recursive), nameof(Force))]
class MyArguments : Parser {
    [FlagOption("recursive", shortName: 'r')]
    bool Recursive { get; set; }

    [FlagOption("force", shortName: 'f')]
    bool Force { get; set; }

    // option -a is now equivalent to -rf and -Rf
}
```
You have to use property names of the aliased options. If you wont use them, you will get an exception on parser initialization.
Keep in mind, that the parser class can be annotated with multiple `AliasOption` attributes. Also, you can create one alias for multiple flag options.

## Option groups

If you want to force the user to select one of a list of options, use `OptionGroup` attribute. Again, use property names to reference other options.

Use the `required` parameter of `OptionGroup` attribute to designate that one of the group arguments must be specified.
The group must not contain any required options, otherwise you'll get a `ConfigurationException`.

```c#
[OptionGroup(required: true, nameof(Words), nameof(Lines))]
class CountArguments : Parser {

    [FlagOption("words", shortName: 'w')]
    bool Words { get; set; }

    [FlagOption("lines", shortName: 'l')]
    bool Lines { get; set; }

    // the user must now decide, whether they want to count words or lines
}
```


## Option dependencies

You can specify that some options are available only when other options are present. 
Use the `useWith` argument in your option attribute which takes the option property name.

```c#
class MyArguments : Parser {

    [FlagOption("print", shortName: 'p', help: "Print out progress")]
    bool Print { get; set; }

    [FlagOption("verbose", shortName: 'v', help: "Print more details", useWith: nameof(Print))]
    bool Verbose { get; set; }

    // if -v is used without -p parameter, an exception is thrown
}
```

## Parser properties

- `int Skip` - Set this at any time during the parsing. The parser will then skip that number of parameters.
- `string[] PlainArgs` - At the end of parsing will contain all plain arguments (excluding `Skip`ped ones)
- `int PlainArgsRequired` - Override this to control the number of required plain arguments. When the parser is finished, it checks the `PlainArgsRequired` against `PlainArgs.Length`. If not equal, `PlainArgsCountException` is thrown. If not overriden, any number of plain arguments is accepted.
- `ArraySegment<string> Arguments` - View on the raw arguments array starting at the currently parsed argument.


