using System;
using System.Text;
using CShargs;

namespace Tests.Data
{
    class FileOptionalArguments : Parser
    {
        [ValueOption("output", shortName: 'o', required: false, help: "Do not send the results to stderr, but overwrite the specified file.")]
        public string OutputFile { get; set; }

        [ValueOption("number-of-muskrats", shortName: 'm', required: false)]
        public int Muskrats { get; set; } = 1;

        [ValueOption("full-name", shortName: 'n', help: "Enter fullname in firstName;lastName format.")]
        public FullName Name { get; set; }
    }
    [ParserConfig(optionFlags: OptionFlags.ForbidLongSpace | OptionFlags.ForbidShortSpace)]
    class FileOptionalArgumentsForbidSpace : Parser
    {
        [ValueOption("output", shortName: 'o', required: false, help: "Do not send the results to stderr, but overwrite the specified file.")]
        public string OutputFile { get; set; }
    }
    [ParserConfig(optionFlags: OptionFlags.ForbidShortNoSpace)]
    class FileOptionalArgumentsForbidNoSpace : Parser
    {
        [ValueOption("output", shortName: 'o', required: false, help: "Do not send the results to stderr, but overwrite the specified file.")]
        public string OutputFile { get; set; }
    }
    [ParserConfig(optionFlags: OptionFlags.ForbidLongEquals | OptionFlags.ForbidShortEquals)]
    class FileOptionalArgumentsForbidEquals : Parser
    {
        [ValueOption("output", shortName: 'o', required: false, help: "Do not send the results to stderr, but overwrite the specified file.")]
        public string OutputFile { get; set; }
    }
    [ParserConfig(optionFlags: OptionFlags.ForbidLongEquals | OptionFlags.ForbidShortEquals | OptionFlags.ForbidShortNoSpace)]
    class FileOptionalArgumentsForbidEqualsNoSpace : Parser
    {
        [ValueOption("output", shortName: 'o', required: false, help: "Do not send the results to stderr, but overwrite the specified file.")]
        public string OutputFile { get; set; }
    }

    [ParserConfig(optionFlags: OptionFlags.ShortCaseInsensitive)]
    class FileOptionalArguments_ShortCI : Parser
    {
        [ValueOption("output", shortName: 'o', required: false, help: "Do not send the results to stderr, but overwrite the specified file.")]
        public string OutputFile { get; set; }

        [ValueOption("number-of-muskrats", shortName: 'm', required: false)]
        public int Muskrats { get; set; } = 1;

        [ValueOption("full-name", shortName: 'n', help: "Enter fullname in firstName;lastName format.")]
        public FullName Name { get; set; }
    }

    [ParserConfig(optionFlags: OptionFlags.LongCaseInsensitive)]
    class FileOptionalArguments_LongCI : Parser
    {
        [ValueOption("output", shortName: 'o', required: false, help: "Do not send the results to stderr, but overwrite the specified file.")]
        public string OutputFile { get; set; }

        [ValueOption("number-of-muskrats", shortName: 'm', required: false)]
        public int Muskrats { get; set; } = 1;

        [ValueOption("full-name", shortName: 'n', help: "Enter fullname in firstName;lastName format.")]
        public FullName Name { get; set; }
    }

    class FileMandatoryArguments : Parser
    {
        [ValueOption("output", shortName: 'o', required: true, help: "Do not send the results to stderr, but overwrite the specified file.")]
        public string OutputFile { get; set; }
    }

    class FullName
    {
        public string FirstName { get; }
        public string LastName { get; }
        public FullName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
        public static bool operator ==(FullName first, FullName second)
        {
            return ((first.FirstName == second.FirstName)
                    && (first.LastName == second.LastName));
        }
        public static bool operator !=(FullName first, FullName second)
        {
            return ((first.FirstName != second.FirstName)
                    || (first.LastName != second.LastName));
        }
        public static FullName Parse(string str)
        {
            const char delimiter = ';';
            int index = 0;
            var namePart = new StringBuilder();
            string firstName = string.Empty;
            string lastName = string.Empty;

            while (index < str.Length) {
                if (str[index] == delimiter) {
                    firstName = namePart.ToString();
                    namePart.Clear();
                } else {
                    namePart.Append(str[index]);
                }
                index++;
            }

            if (namePart.Length > 0) {
                lastName = namePart.ToString();
            }

            return new FullName(firstName, lastName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (ReferenceEquals(obj, null)) {
                return false;
            }

            if (obj.GetType() == this.GetType()) {
                var name = (FullName)obj;
                return (name.FirstName == this.FirstName)
                        && (name.LastName == this.LastName);
            }

            return false;
        }
    }
}
