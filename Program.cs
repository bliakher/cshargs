using System;
using System.Reflection;

namespace CShargs {

    class Program {
        static void Main(string[] args) {

            //var parsed = TimeArguments.Parse<TimeArguments>(args);
            var arguments = new Examples.TimeArguments();
            arguments.Parse(args);

        }
    }
}

