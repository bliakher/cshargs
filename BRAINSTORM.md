# CShargs (🌊🦈) argument parser
- Easy to use declarative argument parser.
- Define a class representing the arguments object. 🌊🦈 will populate this object with data from the command line.

# Usage
- Create a class that represents arguments of your command (must be child of Parser class)
- Properties in the class correspond to command options. Annotate them with attributes to specify kinds of options.
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

## General remarks 
- To use option attributes, the member of your class must be a property.
- There can be only one option attribute for one property.
- Types of option properties must be either
  - one of the C# primitives (`int`, `string`, `bool`, `short`, `long`, etc.)
  - an enum.
  - any user defined type `T`, which provides static method
```c#
        public static T Parse(string str);
```

## Flag option ( `-f` )
- Flag option is used for options without parameters
- For option name, use the first constructor argument `name`
- For alias, use the named argument `alias`
- For help description, use the named argument `help`
```c#
    [FlagOption("silent", alias: "s", help: "No output will be produced to stdout.")]
    bool Silent { get; set; }
    // set to true when `-s` or `--silent` is used
```


## Value option ( `--key Value` / `--key=Value` / `-k Value` / `-kValue` )
- Value option attribute is used for options with parameters, eg
```c#
    [ValueOption("number-of-cats", alias: "c", required: false)]
    int Cats { get; set; } = 12;
    // set with --number-of-cats=123 / --number-of-cats 123 / -c 123 / -c123
```
- Value option has `required` argument. If the option is not used on the command line, one of the following will happen:
    - If `required` not specified, the option is not considered required iff its type is marked as nullable (ie `int?`).
    - If `required: false`, the default value of the property is used
    - If `required: true` and option is not present -> error

## Verb option ( `git push` )
- VerbOption attribute is used for subcommands
```c#
    [VerbOption("push")]
    PushArguments Push { get; set; }
    // ie. git push --force
...
class PushArguments : Parser {

}
```

## Settings
- You can set additional rules by annotating the class with `ParserSettings` attribute
- optionSettings - managing syntax of options eg.
  - aggregating short options ie. -abc / -a -b -c
  - syntax of option parameters ie. -c 123 / -c123 / -c=123
  - case sensitivity
- choosing symbols to denote short and long options and delimiter

## Aliases for options
- Each property option attribute allows you to define one mandatory name and one optional alias (presumably short and long names)
- In addition to that you can set more aliases using AliasOptionAttribute
- Class can be annotated with multiple option alias attributes
- You can also creating aliases for a group of options



## Considerations


## ToDo:
+ Parser class
+ Option attribute classes
+ ParserSettings attribute
+ global settings attributes - synonyms etc
+ write `time` command example


```c#
    [ValueOption("number-of-cats", alias:"c", required:false)]
    int Cats { get; set; }
// --number-of-cats      .... defaults to 0


    [ValueOption("number-of-cats", alias:"c", required:false)]
    int? Cats { get; set; } = 12;


```
