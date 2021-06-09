using System;
using System.Text;
using CShargs;

namespace ExtensionProgram
{
    class Arguments : Parser
    {
        [ValueOption("date", required: true, help: "Datetime value in the ISO 8601 format (e.g. 2018-12-31T14:45).")]
        public DateTime Date { get; set; }

        [ValueOption("format", required: true, help: "String format to be printed. Optional datetime substitions are denoted with % followed by a special char (e.g. 'y' for year).")]
        public string Format { get; set; }
    }

    class Program
    {
        static string FormatDate(DateTime date, string format)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < format.Length; i++)
            {
                char c = format[i];

                if (c != '%')
                {
                    sb.Append(c);
                }
                else if (++i < format.Length)
                {
                    int value = format[i] switch
                    {
                        'y' => date.Year,
                        'm' => date.Month,
                        'd' => date.Day,
                        'h' => date.Hour,
                        'i' => date.Minute,
                        's' => date.Second,
                        _ => throw new FormatException()
                    };

                    sb.Append(value);
                }
                else
                {
                    throw new FormatException();
                }
            }

            return sb.ToString();
        }

        static void Main(string[] cmdArgs)
        {
            var arguments = new Arguments();

            try
            {
                arguments.Parse(cmdArgs);
                Console.WriteLine(FormatDate(arguments.Date, arguments.Format));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                arguments.GenerateHelp(Console.Error, shortHelp: false);
            }
        }
    }
}
