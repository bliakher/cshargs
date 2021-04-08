﻿using Exception = System.Exception;
using FormatException = System.FormatException;

namespace CShargs {

    /// <summary>
    /// Thrown during initialization when there is an error in parser configuration.
    /// </summary>
    internal class ConfigurationException : Exception {
        internal ConfigurationException(string message, Exception innerException = null)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown during runtime when there is an error in the given arguments.
    /// </summary>
    public abstract class ParsingException : Exception {
        internal ParsingException(string message, Exception innerException = null)
            : base(message, innerException) { }
    }

    public abstract class OptionException : ParsingException {

        /// <summary>
        /// Name of the option in question
        /// </summary>
        public string OptionName { get; private set; }

        internal OptionException(string optionName, string message, Exception innerException = null)
            : base(message, innerException) {
            OptionName = optionName;
        }
    }

    /// <summary>
    /// Thrown when option has the wrong format. Please dont throw this exception in custom parsers. To tell the parser
    /// that your custom parsing has failed, throw <see cref="FormatException"/>, or see the docs for alternative ways.
    /// </summary>
    public class ValueOptionFormatException : OptionException {
        internal ValueOptionFormatException(string optionName, FormatException innerException)
            : base(optionName, $"Bad format in option '{optionName}': {innerException.Message}", innerException) { }
    }

    /// <summary>
    /// Thrown when a required option is missing
    /// </summary>
    public class MissingOptionException : OptionException {
        internal MissingOptionException(string optionName, Exception innerException = null)
            : base(optionName, $"Required option '{optionName}' is missing.", innerException) { }
    }

    /// <summary>
    /// Thrown when no option from a required option group is used.
    /// </summary>
    public class MissingOptionGroupException : ParsingException {

        /// <summary>
        /// Name of the group in question
        /// </summary>
        public string GroupName { get; private set; }
        internal MissingOptionGroupException(string groupName, Exception innerException = null)
            : base($"One of options in group 'groupName' must be specified.", innerException) {
            GroupName = groupName;
        }
    }
}
