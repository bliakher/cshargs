using System;
using System.Collections.Generic;
using Exception = System.Exception;
using FormatException = System.FormatException;

namespace CShargs
{

    /// <summary>
    /// Thrown during initialization when there is an error in parser configuration.
    /// </summary>
    public class ConfigurationException : Exception
    {
        internal ConfigurationException(string message, Exception innerException = null)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown during runtime when there is an error in the given arguments.
    /// </summary>
    public abstract class ParsingException : Exception
    {
        internal ParsingException(string message, Exception innerException = null)
            : base(message, innerException) { }
    }

    public abstract class OptionException : ParsingException
    {

        /// <summary>
        /// Name of the option in question
        /// </summary>
        public string OptionName { get; private set; }

        internal OptionException(string optionName, string message, Exception innerException = null)
            : base(message, innerException)
        {
            OptionName = optionName;
        }
    }

    /// <summary>
    /// Thrown whem an unknow option is encountered.
    /// </summary>
    public class UnknownOptionException : OptionException
    {
        internal UnknownOptionException(string optionName, Exception innerException = null)
            : base(optionName, $"Unknown option '{optionName}'", innerException) { }
    }

    public class DuplicateOptionException : OptionException
    {
        internal DuplicateOptionException(string optionName, Exception innerException = null)
            : base(optionName, $"Duplicate option '{optionName}'", innerException) { }
    }

    /// <summary>
    /// Thrown when option has the wrong format. Please dont throw this exception in custom parsers. To tell the parser
    /// that your custom parsing has failed, throw <see cref="FormatException"/>, or see the docs for alternative ways.
    /// </summary>
    public class ValueOptionFormatException : OptionException
    {
        internal ValueOptionFormatException(string optionName, Exception innerException)
            : base(optionName, $"Bad format in option '{optionName}': {innerException.Message}", innerException) { }
    }

    /// <summary>
    /// Thrown when a required option is missing
    /// </summary>
    public class MissingOptionException : OptionException
    {
        internal MissingOptionException(string optionName, Exception innerException = null)
            : base(optionName, $"Missing required option '{optionName}'.", innerException) { }
    }

    public class MissingOptionValueException : OptionException
    {
        internal MissingOptionValueException(string optionName, Exception innerException = null)
            : base(optionName, $"Missing value for option '{optionName}'.", innerException) { }
    }


    /// <summary>
    /// Thrown when no option from a required option group is used.
    /// </summary>
    public class MissingGroupException : ParsingException
    {
        internal MissingGroupException(IEnumerable<string> groupOptions, Exception innerException = null)
            : base($"Missing option from required group ( {String.Join(" | ", groupOptions)} )", innerException) { }
    }

    public class TooManyOptionsException : ParsingException
    {
        internal TooManyOptionsException(IEnumerable<string> groupOptions, Exception innerException = null)
            : base($"You can't use multiple options from exclusive group ( {String.Join(" | ", groupOptions)} )", innerException) { }
    }

    /// <summary>
    /// Aggregation of options can be used only on short FlagOptions.
    /// </summary>
    public class OptionAggregationException : ParsingException
    {
        internal OptionAggregationException(string message)
            : base(message){}
    }

    /// <summary>
    /// Thrown when a a useWith dependency of option is missing
    /// </summary>
    public class MissingDependencyException : OptionException
    {
        internal MissingDependencyException(string optionName, string dependencyName, Exception innerException = null)
            : base(optionName, $"Option {optionName} is used without dependency {dependencyName}", innerException) { }
    }

    public class PlainArgsCountException : ParsingException
    {
        internal PlainArgsCountException(string message, Exception inner = null)
            : base(message, inner) { }
    }
}
