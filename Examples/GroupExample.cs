using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShargs.Examples
{
[OptionGroup(required: true, nameof(Words), nameof(Lines))]
class CountArguments : Parser
{

    [FlagOption("words", shortName: 'w')]
    bool Words { get; set; }

    [FlagOption("lines", shortName: 'l')]
    bool Lines { get; set; }

    // the user must now decide, whether they want to count words or lines
}
}
