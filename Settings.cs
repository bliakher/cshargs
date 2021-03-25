using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShargs {

    [Flags]
    enum OptionSettings {

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
        /// If present, <code>-o=5</code> will assign value "5" to the option property.
        /// Otherwise the value "=5" might be considered.
        /// </summary>
        ShortAllowEquals = 1 << 3,

        /// <summary>
        /// If present, <code>-o 5</code> will assign value "5" to 
        /// </summary>
        ShortAllowSpace = 1 << 4,

        ShortAllowNospace = 1 << 5,

        /// <summary>
        /// If not present, <code>--option=value</code> will not be recognised as valid syntax.
        /// </summary>
        LongAllowEquals = 1 << 6,

        /// <summary>
        /// If not present, <code>--option value</code> will not be recognised as valid syntax.
        /// </summary>
        LongAllowSpace = 1 << 7,

    }

    class ParserSettingsAttribute : Attribute {

        public ParserSettingsAttribute(
            OptionSettings optionSettings = OptionSettings.Default
            ) { }
    }
}

