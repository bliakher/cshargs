using System;
using System.Numerics;

namespace CShargs.Examples
{
    [ParserConfig(optionFlags: OptionFlags.ForbidLongEquals)]
    class VectorArguments : Parser
    {
        public Vector3 Position { get; private set; }

// --pos <x> <y> <z>
[CustomOption("pos", true, help: "Position of the item.")]
void ParsePosition(string value)
{
    if (Arguments.Length < 3) {
        throw new FormatException("Not enough arguments");
    }

    Skip = 3;

    Position = new Vector3(
        int.Parse(Arguments[0]),
        int.Parse(Arguments[1]),
        int.Parse(Arguments[2])
    );
}
    }
}
