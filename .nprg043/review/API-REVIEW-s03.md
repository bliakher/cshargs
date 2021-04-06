# Review
## Overall impression
The documentation is nicely structured and has useful example for every feature the library offers. I like that the arguments are stored in a class and filled by the parser. But I am not sure if it makes sense semantically that the argument class is a child of the parser class. Code seems to be well organized so far.

The provided alias options are a nice feature. Verb option is also very useful in some programs, but it could be documented more (ie. on which class do I call the Parse method). The option dependencies might be useful in some cases, but they force the user to choose one of the options. I did not find a concept that would allow me to specify a group of options that are exclusive but not required (like the parameters -H and -s in numactl). Even though this concept is missing it is possible to work around it.

## Writing numactl 

The main thing missing from the library that would help me implement numactl is the possibility to define exclusive options (like -H and -s or also -p, -i and -m). I was not sure if I can use nullable ints as ValueOptions, that could also be useful so that I know if the option was specified.

## Detailed comments

I could not specify that the Preferred option should be between 0 and 4, but that has a easy workaround.

* Are the public classes/methods documented? Was the documentation good enough for writing numactl or not?

Yes everything I needed was well documented.

* Was the example program documented?

It could be documented more, but the examples in documentation make up for it.

* Is the formatting consistent?

Yes.

* Does the code follows platform conventions (naming etc.)?

Yes.

* How are mandatory options specified?

Using the "required = true" parameter in annotation.

* How are (optional) parameters specified

Same way as mandatory parameters but "required = false"

* How are plain arguments specified?

They are automatically saved in a read only list.

* How are synonyms defined?

Using the alias parameter and also using the AliasOption annotation.

* How are parameter types specified?

The types are specified in the member variable where they are filled.

* When are arguments verified?

They are parsed using the Parse(string) method on the argument type. No option for further verification is available.

* When are parameters read?

When the Parse method is called.

* Are bounded integers, enums and strings as parameters supported?

Bounded integers are not supported but enums and strings are.

* How do you add custom types for the parameters?

You can have any type that implements Parse(string) method.

* How are options (or plain arguments) documented?

"Plain arguments can be found in PlainArgs property as a list of strings."

* When is help printed?

When the method GenerateHelp is called.


# Numactl implementation

```csharp
using System;
using System.Collections.Generic;
using CShargs;

namespace NumactlExample
{
    class NumactlArguments : Parser
    {

        [FlagOption("H", alias: "hardware", help: "Show inventory of available nodes on the system.")]
        public bool Hardware { get; set; }

        [FlagOption("s", alias: "show", help: "Show NUMA policy settings of the current process.")]
        public bool Show { get; set; }

        [ValueOption("m", alias: "membind", required: false, help: "Only allocate memory from nodes. Allocation will fail when there is not enough memory available on these nodes. nodes may be specified as noted above.")]
        public string Membind { get; set; }

        [ValueOption("p", alias: "preferred", required: false, help: "Preferably allocate memory on node, but if memory cannot be allocated there fall back to other nodes. This option takes only a single node number. Relative notation may be used.")]
        public int Preferred { get; set; } = -1;

        [ValueOption("i", alias: "interleave", required: false, help: "Set a memory interleave policy. Memory will be allocated using round robin on nodes. When memory cannot be allocated on the current interleave target fall back to other nodes.")]
        public string Interleave { get; set; }

        [ValueOption("C", alias: "physcpubind", required: false, help: "Only execute process on cpus. This accepts cpu numbers as shown in the processor fields of /proc/cpuinfo, or relative cpus as in relative to the current cpuset.")]
        public string Physcpubind { get; set; }

    }

    class Program
    {
        static void Main(string[] args)
        {
            var arguments = new NumactlArguments();
            arguments.Parse(args);

            if (arguments.Hardware)
            {
                Console.WriteLine("available: 2 nodes (0-1)");
            }
            else if (arguments.Show)
            {
                Console.WriteLine("policy: default");
            }
            else if (arguments.PlainArgs.Count > 0)
            {
                RunProgram(arguments);
            }
            else
            {
                arguments.GenerateHelp(Console.Out);
            }
        }

        static void RunProgram(NumactlArguments args)
        {
            Console.WriteLine($"Will run program: {args.PlainArgs[0]}");
            if (args.PlainArgs.Count > 1) Console.WriteLine($"Program including its arguments: {string.Join(' ', args.PlainArgs)}");
            if (args.Physcpubind != null) Console.WriteLine($"Physical CPU bind: {string.Join(' ', ParseIntegerList(args.Physcpubind, 0, 31))}");
            if (!OptionsAreValid(args)) throw new ArgumentException("Conflicting options (-p | -i | -m)");
            if (args.Interleave != null) Console.WriteLine($"Interleave: {string.Join(' ', ParseIntegerList(args.Interleave, 0, 3))}");
            if (args.Membind != null) Console.WriteLine($"Membind: {string.Join(' ', ParseIntegerList(args.Membind, 0, 3))}");
            if (args.Preferred != -1)
            {
                if (args.Preferred < 0 || args.Preferred > 4) throw new Exception("Preferred option is out of range");
                Console.WriteLine($"Preferred node: {args.Preferred}");
            }
        }

        static bool OptionsAreValid(NumactlArguments args)
        {
            int exclusiveOptions = 0;
            if (args.Interleave != null) exclusiveOptions++;
            if (args.Membind != null) exclusiveOptions++;
            if (args.Preferred != -1) exclusiveOptions++;
            return exclusiveOptions > 1;
        }

        static List<int> ParseIntegerList(string input, int min, int max)
        {
            List<int> result = new();
            if (input == "all")
            {
                for (int i = min; i <= max; i++)
                {
                    result.Add(i);
                }
                return result;
            }

            string[] tokens = input.Split(',');
            foreach (var token in tokens)
            {
                ParseIntegerListToken(result, token, min, max);
            }
            return result;
        }

        static void ParseIntegerListToken(List<int> result, string token, int min, int max)
        {
            if (token.Contains('-'))
            {
                var bounds = token.Split('-');
                if (bounds.Length != 2 ||
                     int.TryParse(bounds[0], out int lower) == false ||
                     int.TryParse(bounds[1], out int upper) == false ||
                     lower > upper || lower < min || upper > max)
                {
                    throw new ParseListException("bad token: " + token);
                }
                for (int i = lower; i <= upper; i++)
                {
                    if (result.Contains(i)) throw new ParseListException($"Number {i} appears multiple times");
                    result.Add(i);
                }
            }
            else
            {
                if (int.TryParse(token, out int num))
                {
                    result.Add(num);
                }
                else throw new ParseListException("Int parse error: " + token);
            }
        }

        public class ParseListException : Exception
        {
            public ParseListException(){}
            public ParseListException(string message) : base(message) { }
            public ParseListException(string message, Exception inner) : base(message, inner) { }
        }
    }
}
```