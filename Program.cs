using System;
using System.Collections.Generic;
using System.Reflection;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tests")]

namespace CShargs
{

    class Program
    {
        static void Main(string[] args)
        {
            var arguments = new Examples.TimeArguments();
            arguments.Parse(new[] { "--help" });

            // check version option
            if (arguments.Version) {
                Console.WriteLine("Version option present.");
            }

            // generate structured help, write it to console
            if (arguments.Help) {
                arguments.GenerateHelp(Console.Out);
                arguments.GenerateHelp(Console.Out, false);
            }

            // get parsed plain arguments
            var plainArgs = arguments.PlainArgs;

        }
    }
}

