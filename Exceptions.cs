using System;
using System.Collections.Generic;
using Exception = System.Exception;
using FormatException = System.FormatException;

namespace CShargs
{

    /// \addtogroup ConfigurationExceptions
    /// @brief Exceptions thrown during parser initialization.
    /// 
    /// <see cref="ConfigurationException"/> is thrown during parser initialization for parser configuration errors.
    /// @{
    
    /// <summary>
    /// Thrown during initialization when there is an error in parser configuration.
    /// </summary>
    public class ConfigurationException : Exception
    {
        internal ConfigurationException(string message, Exception innerException = null)
            : base(message, innerException) { }

        internal static void ThrowIf(bool condition, string message, Exception innerException = null)
        {
            if (condition) {
                throw new ConfigurationException(message, innerException);
            }
        }
    }
    /// @}
    
    /// \addtogroup ParsingExceptions
    /// @brief Exceptions thrown during parsing.
    /// 
    /// <see cref="ParsingException"/> is thrown during parsing.
    /// For details see individual exceptions.
    /// @{

    /// <summary>
    /// Thrown during runtime when there is an error in parsing the given arguments.
    /// </summary>
    public abstract class ParsingException : Exception
    {
        internal ParsingException(string message, Exception innerException = null)
            : base(message, innerException) { }
    }

    /// <summary>
    /// For errors that involve specific option
    /// </summary>
    public abstract class OptionException : ParsingException
    {

        /// <summary>
        /// Name of the option in question
        /// </summary>
        public string OptionName { get;  }

        internal OptionException(string optionName, string message, Exception innerException = null)
            : base(message, innerException)
        {
            OptionName = optionName;
        }
    }

    /// <summary>
    /// Thrown whem an unknow option is encountered.
    /// </summary>
    public sealed class UnknownOptionException : OptionException
    {
        internal UnknownOptionException(string optionName, Exception innerException = null)
            : base(optionName, $"Unknown option '{optionName}'", innerException) { }
    }

    /// <summary>
    /// Thrown when one option is encountered twice.
    /// </summary>
    public sealed class DuplicateOptionException : OptionException
    {
        internal DuplicateOptionException(string optionName, Exception innerException = null)
            : base(optionName, $"Duplicate option '{optionName}'", innerException) { }
    }

    /// <summary>
    /// Thrown when option has the wrong format. Please dont throw this exception in custom parsers. To tell the parser
    /// that your custom parsing has failed, throw <see cref="FormatException"/>, or see the docs for alternative ways.
    /// </summary>
    public sealed class ValueOptionFormatException : OptionException
    {
        internal ValueOptionFormatException(string optionName, Exception innerException)
            : base(optionName, $"Bad format in option '{optionName}': {innerException.Message}", innerException) { }
    }

    /// <summary>
    /// Thrown when a required option is missing.
    /// </summary>
    public sealed class MissingOptionException : OptionException
    {
        internal MissingOptionException(string optionName, Exception innerException = null)
            : base(optionName, $"Missing required option '{optionName}'.", innerException) { }
    }

    /// <summary>
    /// Thrown when parameter of value option is missing.
    /// </summary>
    public sealed class MissingOptionValueException : OptionException
    {
        internal MissingOptionValueException(string optionName, Exception innerException = null)
            : base(optionName, $"Missing value for option '{optionName}'.", innerException) { }
    }


    /// <summary>
    /// Thrown when no option from a required option group is used.
    /// </summary>
    public sealed class MissingGroupException : ParsingException
    {
        internal MissingGroupException(IEnumerable<string> groupOptions, Exception innerException = null)
            : base($"Missing option from required group ( {String.Join(" | ", groupOptions)} )", innerException) { }
    }

    /// <summary>
    /// Thrown when multiple options from an exclusive group are used.
    /// </summary>
    public sealed class MultipleOptionsFromGroupException : ParsingException
    {
        internal MultipleOptionsFromGroupException(IEnumerable<string> groupOptions, Exception innerException = null)
            : base($"You can't use multiple options from exclusive group ( {String.Join(" | ", groupOptions)} )", innerException) { }
    }

    /// <summary>
    /// Aggregation of options can be used only on short FlagOptions.
    /// </summary>
    public sealed class OptionAggregationException : ParsingException
    {
        internal OptionAggregationException(string message)
            : base(message){}
    }

    /// <summary>
    /// Thrown when a useWith dependency of option is missing
    /// </summary>
    public sealed class MissingDependencyException : OptionException
    {
        internal MissingDependencyException(string optionName, string dependencyName, Exception innerException = null)
            : base(optionName, $"Option {optionName} is used without dependency {dependencyName}", innerException) { }
    }

    /// <summary>
    /// Thrown when count of plain arguments doesn't match the required count
    /// </summary>
    public sealed class PlainArgsCountException : ParsingException
    {
        internal PlainArgsCountException(string message, Exception inner = null)
            : base(message, inner) { }
    }
    /// @}
}
