# CShargs `🌊🦈` argument parser
- Easy to use declarative argument parser.
- Define a class representing the arguments object. 🌊🦈 will populate this object with data from the command line.

**Table of contents:**
[ToC]



***

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


# Features

## Flag option ( `--flag` / `-f` )
- Flag option is used for options without parameters
- For option name, use the first constructor argument `name`
- For alias, use the named argument `alias`
- For help description, use the named argument `help`
```c#
    [FlagOption("silent", alias: "s", help: "No output will be produced to stdout.")]
    bool Silent { get; set; }
    // set to true when `-s` or `--silent` is used
```


## Value option ( `--key=Value` / `-k Value` / `-kValue` )
Value option attribute is used for options with parameters, eg.
```c#
    [ValueOption("number-of-cats", alias: "c", required: false)]
    int Cats { get; set; } = 12;
    // set with --number-of-cats=123 / --number-of-cats 123 / -c 123 / -c123
```
Value option has `required` argument. Use this to tell the parser to automatically raise exception when the argument is missing.

- If `required` not specified, it is inferred from the nullability of the used type. (nullable &equiv; required)
- If `required: false`, the default value of the property is used.
- If `required: true`, missing option throws an exception.

Types of option properties must be either:
  - one of the C# primitives (`int`, `string`, `bool`, `short`, `long`, etc.)
  - an enum.
  - any user defined type `T`, which provides static method `T Parse(string str);`



## Verb option ( `git push` )
- Verb option attribute is used for subcommands
```c#
class GitArguments : Parser {
    [VerbOption("push")]
    PushArguments Push { get; set; }
    // ie. git push --force
}

class GitPushArguments : Parser {
    [FlagOption("force")]
    bool Force { get; set; }
    // ie. git push --force
}
```

## Settings
- You can set additional rules with parameters given to Parser constructor
- optionSettings - managing syntax of options eg.
  - aggregating short options ie. -abc / -a -b -c
  - syntax of option parameters ie. -c 123 / -c123 / -c=123
  - case sensitivity
- choosing symbols to denote short and long options and delimiter
- all constants are in the class OptionSettings

## Alias options
Each option allows you to define one mandatory name and one optional alias (presumably short and long names).

In addition to that you can set more aliases using `AliasOptionAttribute`, like this:
```c#
[AliasOption("R", nameof(Recursive))]
[AliasOption("a", nameof(Recursive), nameof(Force))]
class MyArguments : Parser {
    [FlagOption("r", alias:"recursive")]
    bool Recursive { get; set; }
    
    [FlagOption("f", alias:"force")]
    bool Force { get; set; }
    
    // option -a is now equivalent to -rf and -Rf
}
```
You have to use property names of the aliased options. If you wont use them, you will get an exception on static initialization.

Keep in mind, that the parser class can be annotated with multiple `AliasOption` attributes. Also, you can create aliases for multiple options. 

## Option groups

If you want to force the user to select one of a list of options, use `OptionGroup` attribute. Again, use property names to reference other options.

```c#
[OptionGroup(nameof(Recursive), nameof(Force))]
class CountArguments : Parser {

    [FlagOption("w", alias:"words")]
    bool Words { get; set; }
    
    [FlagOption("l", alias:"lines")]
    bool Lines { get; set; }
    
    // the user must now decide, whether they want to count words or lines
}
```

## Callbacks

If you need The parser base class provides you with virtual methods, which you can override to execute your code on various events.

| Method name      | Called... |
|------------------|-----------|
| `OnFinish()`     | when the parser has finished without an error. |
| `OnError(ref bool suppress)` | when the parser is going to throw an exception. Handling exceptions is expensive, so you can set `suppress=true` to handle the error even before any exception is thrown |
