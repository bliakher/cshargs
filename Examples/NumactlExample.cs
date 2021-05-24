using System;
/**
 * We are very grateful to s13 for providing this beautiful example:
 */

namespace CShargs.Examples
{
    internal class NumactlProgram
    {
        public static void RunExample(string[] args)
        {

            var options = new Options();
            try
            {
                options.Parse(args);
            }
            catch
            {
                ShowHelp(options);
                return;
            }

            if (options.Show)
            {
                Console.WriteLine("policy: default ...");
            }
            else if (options.Hardware)
            {
                Console.WriteLine("available: 2 nodes (0-1)");
            }
            else if (options.PlainArgs == null || options.PlainArgs.Count == 0)
            {
                ShowHelp(options);
            }
            else
            {
                Console.WriteLine("will run " + options.PlainArgs[0]);
                if (options.Physcpubind != null)
                    Console.WriteLine("CPU node bind: " + options.Physcpubind);
                // ...
            }
        }

        private static void ShowHelp(Options options)
        {
            Console.WriteLine("usage: numactl [--interleave= | -i <nodes>] ..."); // Print the part of usage which can't be generated
            Console.WriteLine(options.GenerateHelp()); // Print the part of usage which can be generated
        }
    }

    internal class Options : Parser
    {
        [ValueOption("interleave", shortName:'i', help: "Interleave memory allocation across given nodes.", required: false)]
        public NodeNumbers Interleave { get; set; }

        [ValueOption("preferred", shortName: 'p', help: "Prefer memory allocations from given node.", required: false)]
        public NodeNumber Preferred { get; set; }

        [ValueOption("membind", shortName: 'm', help: "Allocate memory from given nodes only.", required: false)]
        public NodeNumbers Membind { get; set; }

        [ValueOption("physcpubind", shortName: 'C', help: "Run on given CPUs only.", required: false)]
        public NodeNumbers Physcpubind { get; set; }

        [FlagOption("show", shortName:'S', help: "Show current NUMA policy.")]
        public bool Hardware { get; set; }

        [FlagOption("hardware", shortName:'H', help: "Print hardware configuration.")]
        public bool Show { get; set; }

        public Options()
        {
            Interleave = new NodeNumbers(); // There should be some function which finds out default memory interleaving
            Membind = new NodeNumbers(); // Same
            Physcpubind = new NodeNumbers(); // Same
        }
    }

    internal class NodeNumbers
    {
        public int[] nodes;

        public static NodeNumbers Parse(string str)
        {
            var nodeNumbers = new NodeNumbers();
            nodeNumbers.nodes = new int[] { 1, 2 }; // Parse from str
            return nodeNumbers;
        }

        public override string ToString()
        {
            return string.Join(",", nodes);
        }
    }

    internal class NodeNumber
    {
        public int node;
        public bool isSet = false;

        public static NodeNumber Parse(string str)
        {
            var nodeNumber = new NodeNumber();
            nodeNumber.isSet = true;
            nodeNumber.node = 1; // Parse from str
            return nodeNumber;
        }

    }
}