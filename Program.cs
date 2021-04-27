using System;
using System.Collections.Generic;
using System.Reflection;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tests")]

namespace CShargs {

    class Program {
        static void Main(string[] args) {

            var arguments = new Examples.TimeArguments();
            arguments.Parse(args);

            // check version option
            if (arguments.Version) {
                Console.WriteLine("Version option present.");
            }

            // generate structured help, write it to console
            arguments.GenerateHelp(Console.Out); 

            // get parsed plain arguments
            var plainArgs = arguments.PlainArgs;

        }
    }
}

