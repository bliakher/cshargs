using System;

using CShargs;

namespace Tests.Data
{
    // example: --pos <x> <y> <z>
    // note: here we assume that "=" in value options are disabled via OptionSettings.ForbidLongEquals
    class VectorArguments : Parser // rework
    {
        public VectorArguments() : base(OptionFlags.ForbidLongEquals) { }
        public Vector3 Position { get; private set; }

        [CustomOption("pos", true, help: "Position of the item.")]
        void ParsePosition(string value)
        {
            if (Arguments.Count <= 3)
            {
                throw new FormatException("Not enough arguments");
            }

            Skip = 3;

            Position = new Vector3(
                int.Parse(Arguments[1]),
                int.Parse(Arguments[2]),
                int.Parse(Arguments[3])
            );
        }
    }

    struct Vector3
    {
        public int First { get; }
        public int Second { get; }
        public int Third { get; }
        public Vector3(int first, int second, int third)
        {
            First = first;
            Second = second;
            Third = third;
        }
    }
}
