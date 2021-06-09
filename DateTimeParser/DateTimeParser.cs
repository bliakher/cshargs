using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CShargs;

namespace DateTimeParser
{
    /// <summary>
    /// Parse a DateTime object from the provided date string and print it in the provided format.
    /// For all the valid input formats see Microsoft docs for DateTime.Parse(string input) and DateTime.ToString(string format)
    /// </summary>
    [ParserConfig(commandName:"extension_app")]
    class DateTimeParser : Parser
    {
        [ValueOption("date", required: true, shortName: 'd', help: "A date and time in any reasonable format")]
        public DateTime Date { get; set; }

        [ValueOption("format", required: true, shortName: 'f', help: "A format string to use when printing the date")]
        public string Format { get; set; }  
    }
}
