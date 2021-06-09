using System;
using CShargs;


namespace DateTimeParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new DateTimeParser();
            try
            {
                parser.Parse(args);
            }
            catch(ParsingException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine(parser.GenerateHelp());
                Console.WriteLine(parser.GenerateHelp(false));
                return;
            }

            Console.WriteLine(parser.Date.ToString(parser.Format));
        }
    }
}
