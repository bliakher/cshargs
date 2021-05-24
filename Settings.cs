﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShargs
{

    /// <summary>
    /// Enumeration of different parsing configurations
    /// </summary>
    [Flags]
    public enum OptionFlags
    {

        /// <summary>
        /// Case sensitive, all syntax variants allowed.
        /// </summary>
        Default = 0,

        /// <summary>
        /// If present, flag options cannot aggregated together. ie "-a -b -c" is NOT equivalent to "-abc"
        /// </summary>
        ForbidAggregated = 1 << 0,

        /// <summary>
        /// If present, short flag options are case insensitive.
        /// </summary>
        ShortCaseInsensitive = 1 << 1,

        /// <summary>
        /// If present, long flag options are case insensitive.
        /// </summary>
        LongCaseInsensitive = 1 << 2,

        /// <summary>
        /// If present, <code>-o=5</code> will not be recognised as valid syntax.
        /// Note: depending on the bit <see cref="ForbidShortNoSpace"/> value "=5" might be considered.
        /// </summary>
        ForbidShortEquals = 1 << 3,

        /// <summary>
        /// If present, <code>-o 5</code> will not be recognised as valid syntax.
        /// </summary>
        ForbidShortSpace = 1 << 4,

        /// <summary>
        /// If present, <code>-o5</code> will not be recognised as valid syntax.
        /// </summary>
        ForbidShortNoSpace = 1 << 5,

        /// <summary>
        /// If present, <code>--option=value</code> will not be recognised as valid syntax.
        /// </summary>
        ForbidLongEquals = 1 << 6,

        /// <summary>
        /// If present, <code>--option value</code> will not be recognised as valid syntax.
        /// </summary>
        ForbidLongSpace = 1 << 7,

        /// <summary>
        /// If present, enum types will be parsed with ignoreCase=true in <see cref="Enum.Parse(Type, string, bool)"/>
        /// </summary>
        EnumCaseInsensitive = 1 << 8,

        /// <summary>
        /// If present, verb options are case insensitive
        /// </summary>
        VerbCaseInsensitive = 1 << 9,
    }


    /// <summary>
    /// Interface that represents configuration of parser
    /// </summary>
    public interface IParserConfig
    {
        /// <summary>
        /// Name of command
        /// </summary>
        string CommandName { get; }
        /// <summary>
        /// Parser configurations from <see cref="OptionFlags"/>> enum
        /// </summary>
        OptionFlags OptionFlags { get; }
        /// <summary>
        /// Symbol with which short options are denoted 
        /// </summary>
        string ShortOptionSymbol { get; }
        /// <summary>
        /// Symbol with which long options are denoted
        /// </summary>
        string LongOptionSymbol { get; }
        /// <summary>
        /// Symbol that separates plain arguments from options
        /// </summary>
        string DelimiterSymbol { get; }
        /// <summary>
        /// Symbol used with value options: -n=5 --number-of-cats=5
        /// </summary>
        string EqualsSymbol { get; }
    }

    /// <summary>
    /// Attribute for changing default parser configuration
    /// Target is the parser class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ParserConfigAttribute : Attribute, IParserConfig
    {

        /// <summary>
        /// Checks made for parser config:
        /// - If option symbols are the same for short and long options, aggregation must be forbidden.
        /// - Long option symbol must be same length or longer than short option symbol
        /// - Short and long option symbols and command name cannot be null.
        /// If checks fail, Configuration exception is thrown. <see cref="ConfigurationException"/>
        /// </summary>
        public ParserConfigAttribute(
            string commandName = "command",
            OptionFlags optionFlags = OptionFlags.Default,
            string shortOptionSymbol = "-",
            string longOptionSymbol = "--",
            string delimiterSymbol = "--",
            string equalsSymbol = "="
            )
        {
            ConfigurationException.ThrowIf(shortOptionSymbol == longOptionSymbol && !optionFlags.HasFlag(CShargs.OptionFlags.ForbidAggregated),
                "Cannot use same short and long symbol while allowing aggregated short options.");
            ConfigurationException.ThrowIf(shortOptionSymbol.Length > longOptionSymbol.Length,
                $"{nameof(shortOptionSymbol)} cannot be longer than {nameof(longOptionSymbol)}.");
            ConfigurationException.ThrowIf(shortOptionSymbol == null, nameof(shortOptionSymbol) + " cannot be null.");
            ConfigurationException.ThrowIf(longOptionSymbol == null, nameof(longOptionSymbol) + " cannot be null.");
            ConfigurationException.ThrowIf(commandName == null, nameof(commandName) + " cannot be null.");

            if (equalsSymbol == null) {
                optionFlags |= OptionFlags.ForbidLongEquals | OptionFlags.ForbidShortEquals;
            }

            CommandName = commandName;
            OptionFlags = optionFlags;
            ShortOptionSymbol = shortOptionSymbol;
            LongOptionSymbol = longOptionSymbol;
            DelimiterSymbol = delimiterSymbol;
            EqualsSymbol = equalsSymbol;
        }

        public string CommandName { get; }
        public OptionFlags OptionFlags { get; }
        public string ShortOptionSymbol { get; }
        public string LongOptionSymbol { get; }
        public string DelimiterSymbol { get; }
        public string EqualsSymbol { get; }
    }
}