using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShargs
{

    /// <summary>
    /// Enumeration of different option
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
    }


    public interface IParserConfig
    {
        OptionFlags OptionFlags { get; }
        string ShortOptionSymbol { get; }
        string LongOptionSymbol { get; }
        string DelimiterSymbol { get; }
        string EqualsSymbol { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ParserConfigAttribute : Attribute, IParserConfig
    {

        public ParserConfigAttribute(
            OptionFlags optionFlags = OptionFlags.Default,
            string shortOptionSymbol = "-",
            string longOptionSymbol = "--",
            string delimiterSymbol = "--",
            string equalsSymbol = "="
            )
        {
            ThrowIf.ArgumentValue(shortOptionSymbol == longOptionSymbol && !optionFlags.HasFlag(CShargs.OptionFlags.ForbidAggregated),
                "Cannot use same short and long symbol while allowing aggregated short options.");
            ThrowIf.ArgumentValue(shortOptionSymbol.Length > longOptionSymbol.Length, $"{nameof(shortOptionSymbol)} cannot be longer than {nameof(longOptionSymbol)}.");

            OptionFlags = optionFlags;
            ShortOptionSymbol = shortOptionSymbol;
            LongOptionSymbol = longOptionSymbol;
            DelimiterSymbol = delimiterSymbol;
            EqualsSymbol = equalsSymbol;
        }

        public OptionFlags OptionFlags { get; private init; }
        public string ShortOptionSymbol { get; private init; }
        public string LongOptionSymbol { get; private init; }
        public string DelimiterSymbol { get; private init; }
        public string EqualsSymbol { get; private init; }
    }
}