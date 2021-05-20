using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShargs {

    /// <summary>
    /// Enumeration of different option
    /// </summary>

    [Flags]
    public enum OptionSettings {

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
        /// Note: depending on the bit <see cref="ForbidShortNospace"/> value "=5" might be considered.
        /// </summary>
        ForbidShortEquals = 1 << 3,

        /// <summary>
        /// If present, <code>-o 5</code> will not be recognised as valid syntax.
        /// </summary>
        ForbidShortSpace = 1 << 4,

        /// <summary>
        /// If present, <code>-o5</code> will not be recognised as valid syntax.
        /// </summary>
        ForbidShortNospace = 1 << 5,

        /// <summary>
        /// If not present, <code>--option=value</code> will not be recognised as valid syntax.
        /// </summary>
        ForbidLongEquals = 1 << 6,

        /// <summary>
        /// If not present, <code>--option value</code> will not be recognised as valid syntax.
        /// </summary>
        ForbidLongSpace = 1 << 7,

    }
}

