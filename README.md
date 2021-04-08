# TODOs:

- README
    - chybí zmínka o CustomOptionAttribute
    - chybí Skip, PlainArgsRequired (s13)
    - popsat výjimky, hlavně jak řízeně spadnout
        - tj. sekce error handling
    - ujasnit Nullable optiony
+ u OptionGroup umožnit nepovinou grupu
+ předělat virtuální callbacky na eventy
+ přidat výjimky
- přesunout OptionGroup do future? (s13) ne


# `🌊🦈` CShargs argument parser

(Hopefully) easy to use declarative argument parser.

Define a class representing the arguments object. 🌊🦈 will populate this object with data from the command line.

**Table of contents:**

[[_TOC_]]



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
Flag option attribute is used for options without parameters. The type of the property must be bool.
- For option name, use the first constructor argument `name`
- For alias, use the named argument `alias`
- For option that can be used only with some other option use the named argument `useWith` which takes the option property name.
- For help description, use the named argument `help`
```c#
    [FlagOption("silent", alias: "s", help: "No output will be produced to stdout.")]
    bool Silent { get; set; }
    // set to true when `-s` or `--silent` is used
```


## Value option ( `--key=Value` / `-k Value` / `-kValue` )
Value option attribute is used for options with parameters, eg. Type of the option property must be either:
  - one of the C# primitives (`int`, `string`, `bool`, `short`, `long`, etc.)
  - an enum.
  - any user defined type `T`, which provides static method `T Parse(string str);`
```c#
    [ValueOption("number-of-cats", alias: "c", required: false)]
    int Cats { get; set; } = 12;
    // set with --number-of-cats=123 / --number-of-cats 123 / -c 123 / -c123
```
Value option has `required` argument. Use this to tell the parser to automatically raise exception when the argument is missing.

- If `required` not specified, it is inferred from the nullability of the used type. (nullable &equiv; required)
- If `required: false`, the default value of the property is used.
- If `required: true`, missing option throws an exception.
- For alias, use the named argument `alias`
- For option that can be used only with some other option use the named (see [Option dependencies]())
- For help description, use the named argument `help`




## Verb option ( `git push` )
Verb option attribute is used for subcommands. In this case, type of the option property must be a child of the `Parser` class.
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
[OptionGroup(nameof(Words), nameof(Lines))]
class CountArguments : Parser {

    [FlagOption("w", alias:"words")]
    bool Words { get; set; }

    [FlagOption("l", alias:"lines")]
    bool Lines { get; set; }

    // the user must now decide, whether they want to count words or lines
}
```


## Option dependencies

You can specify that some options are available only when other options are present. Use the `useWith` argument in your option attribute which takes the option property name.

```c#
class MyArguments : Parser {

    [FlagOption("p", alias:"print", help: "Print out progress")]
    bool Print { get; set; }

    [FlagOption("v", alias:"verbose", help: "Print more details", useWith: nameof(Print))]
    bool Verbose { get; set; }

    // if -v is used without -p parameter, an exception is thrown
}
```

## Callbacks

If you need The parser base class provides you with virtual methods, which you can override to execute your code on various events.

| Method name      | Called... |
|------------------|-----------|
| `OnFinish()`     | when the parser has finished without an error. |
| `OnUnknownParameter(string p)`     | when the parser has encountered an unknown parameter. Before the parser throws an exception, you can decide whether the syntax is really wrong Return `false` to indicate error. |


