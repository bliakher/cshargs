# Task #1: API Design

The general topic of the assignments is the design and implementation
of a library for parsing command-line arguments. It is a well-defined
problem which leaves a lot of room for different design decisions,
and allows different solutions that can all be considered good.

## Definitions

Short option
: A command-line argument that starts with a single dash,
  followed by a single character (e.g. "`-v`")

Long option
: A command-line argument that starts with two dashes,
  followed by one or more characters (e.g. "`--version`")

Option
: Short option or Long option

Option parameter
: A command-line argument that follows an option (if the option is defined
  to accept parameters by the user of your library)

Plain argument
: A command-line argument that is neither an option nor an option paramter.

Delimiter
: A command-line argument consisting of two dashes only, i.e. `--`.
  Any subsequent argument is considered to be a plain argument

For example, in the following command

```shell
cmd -v --version -s OLD --length=20 -- -my-file your-file
```

there are short options `v` and `s`, long options `version` and
`length`, the `OLD` and `20` represent parameters to `-s` and `--length`
respectively. The `-my-file` and `your-file` arguments after the `--` delimiter
are plain arguments.


## Functional requirements

Your library must allow the user to perform the following operations in a
convenient way:

- To specify what options the client program accepts, which of them
  are optional and which of them are mandatory, verify the actual
  arguments passed to the client program conform to this
  specification.
- To define synonyms (at the very least 1:1 between short and long
  options, but ideally in a more general way).
- To specify, whether an option may/may not/must accept parameters,
  verify that the actual arguments passed to the client program
  conform to this specification.
- To specify types of parameters, verify that the actual arguments
  passed to the client program conform to this specification. At the
  very least the library has to distinguish between string parameters,
  integral parameters (with either or both: lower and upper bound),
  boolean parameters, and string parameters with fixed domain
  (enumeration).
- To determine what options were passed to the client program together
  with their parameters.
- To extract values of all plain arguments. The delimiter may be
  omitted unless a plain argument starts with `-`.
- To document the options and to present the documentation to the user
  in form of a help text.

It is not necessary to support all of the above  explicitly. For
example, the validation of a type of a parameter may be performed
implicitly upon retrieval of the value by the user. An exception may be
thrown if the corresponding argument has an incorrect type.

The requirements are intentionally not exhaustive. Use your imagination
and be creative :-).


## What To Submit

You will be given access to Git repository hosted on the faculty GitLab
instance. You are expected to commit your solution there.

In this task, you are supposed to **ONLY** design the API. Therefore, you
are expected to write source code with **EMPTY** implementations of
individual methods (except for `return null` and similar as required by
the compiler). The API you submit must, however, fulfill the
requirements written above.

Apart from the bare API, also must also submit an example program that uses
the API to parse the following arguments (obviously, it will fail, because
the library has no implementation yet) that are used by `time` command.

```text
time [options] command [arguments...]

GNU Options
    -f FORMAT, --format=FORMAT
           Specify output format, possibly overriding the format specified
           in the environment variable TIME.
    -p, --portability
           Use the portable output format.
    -o FILE, --output=FILE
           Do not send the results to stderr, but overwrite the specified file.
    -a, --append
           (Used together with -o.) Do not overwrite but append.
    -v, --verbose
           Give very verbose output about all the program knows about.

GNU Standard Options
    --help Print a usage message on standard output and exit successfully.
    -V, --version
           Print version information on standard output, then exit successfully.
    --     Terminate option list.
```

Do **NOT** implement the actual command, only show how your API would be
used to set-up option parsing for this command.

The source code may be in any of these programming languages: Java, C#,
C or C++. When choosing the language, keep in mind that eventually, your
implementation will be required to build and run without IDE even on Linux
system. This is usually not much of problem with languages such as Java,
C, or C++, but in the case of C#, it may require using .NETCore runtime
which is supported on Linux.

Feel free to commit also files of your build system. But bear in mind
that we might ask you to change it later on to ensure smooth integration
with the CI environment (however, cmake, make, Ant and Maven are okay).
However, do **NOT** commit compiled files (`.class`, `.o`, etc.) and
other garbage produced by IDEs -- set up your `.gitignore` file properly.

## General suggestions

- Take a look at the existing libraries out there.
- Try to remedy their drawbacks and whatever annoyances you fancy.
- Modify some of your previous projects to use your API to parse its
  command-line options. This may help you discover design issues early
  in the process.
  
## Design considerations

- In what way will the user declare individual options, their
  parameters and synonyms? What data structures could capture these.
- In what way will the user react to options? How will the options be
  accessed? Callbacks? List of all options? On-demand access to
  particular options?
- In what way will the library validate the arguments?
  Explicitly/implicitly? Exceptions/Error codes? Will the library
  produce warnings displayed to the user directly?
- What classes will the library contain? What purpose will they have?


## Submission

When you are satisfied with your solution to this task, make sure it is
in the **master** branch of your repository and **tag the commit**
using the `task-1-submission` tag.

