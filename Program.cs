using System;
using System.Reflection;

namespace CShargs {

    class Program {
        static void Main(string[] args) {

            var arguments = new Examples.TimeArguments();
            arguments.Parse(args); // parse args, populate TimeArguments instance with options from args

            if (arguments.Version)  // check version option
            {
                Console.WriteLine("Version option present.");
            }

            arguments.GenerateHelp(Console.Out); // generate structured help, write it to console

            string[] plainArgs = arguments.PlainArgs;   // get parsed plain arguments

        }
    }
}

