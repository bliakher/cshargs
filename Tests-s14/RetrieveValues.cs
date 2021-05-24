using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using CShargs;

namespace s14_api_testing
{
    public class RetrieveValues
    {
        enum MyEnum
        {
            One, Two, Three
        }

        class Args : Parser
        {
            [ValueOption("alpha", shortName: 'a', help: "Specify number alpha")]
            public int Alpha { get; set; }

            [ValueOption("beta", shortName: 'b', help: "Specify string beta")]
            public string Beta { get; set; }

            [ValueOption("gamma", shortName: 'g', help: "Specify bool gamma")]
            public bool Gamma { get; set; }

            [ValueOption("delta", shortName: 'd', help: "Specify short delta")]
            public short Delta { get; set; }

            [ValueOption("epsilon", shortName: 'e', help: "Specify long epsilon")]
            public long Epsilon { get; set; }

            [ValueOption("enum", shortName: 'n')]
            public MyEnum EnumVal { get; set; }
        }

        [ParserConfig(optionFlags: OptionFlags.EnumCaseInsensitive)]
        class EnumArgsCaseInsensitive : Parser
        {
            [ValueOption("enum", shortName: 'n')]
            public MyEnum EnumVal { get; set; }
        }

        [Fact]
        public void ReturnIntOptionValue()
        {
            // Arrange
            string[] args = { "-a", "424769" };
            var CLParser = new Args();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.Equal(424769, CLParser.Alpha);
        }

        [Fact]
        public void ReturnStringOptionValue()
        {
            // Arrange
            string[] args = { "-b", "Foo bar" };
            var CLParser = new Args();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.Equal("Foo bar", CLParser.Beta);
        }
        [Fact]
        public void ReturnBoolOptionValue()
        {
            // Arrange
            string[] args = { "-g", "true" };
            var CLParser = new Args();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.True(CLParser.Gamma);
        }
        [Fact]
        public void InvalidBoolOptionValueFails()
        {
            // Arrange
            string[] args = { "-g", "bambam" };
            var CLParser = new Args();

            // Act
            Assert.Throws<ValueOptionFormatException>(() => CLParser.Parse(args));
        }

        [Fact]
        public void ReturnShortOptionValue()
        {
            // Arrange
            string[] args = { "-d", "13" };
            var CLParser = new Args();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.Equal((short)13, CLParser.Delta);
        }
        [Fact]
        public void ReturnLongOptionValue()
        {
            // Arrange
            long iValue = 897465189465L;
            string[] args = { "-e", iValue.ToString() };
            var CLParser = new Args();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.Equal(iValue, CLParser.Epsilon);
        }

        [Fact]
        public void ValidEnumOptionValue()
        {
            // Arrange
            MyEnum val = MyEnum.Three;
            string[] args = { "--enum", val.ToString() };
            var CLParser = new Args();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.Equal(val, CLParser.EnumVal);
        }
        [Fact]
        public void CaseInsEnumOptionValue()
        {
            // Arrange
            MyEnum val = MyEnum.Three;
            string[] args = { "--enum", val.ToString() };
            var CLParser = new Args();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.Equal(val, CLParser.EnumVal);
        }

        [Fact]
        public void InvalidEnumValueThrows()
        {
            // Arrange
            MyEnum val = MyEnum.Three;
            string[] args = { "--enum", "threeeeeeeeeeee" };
            var CLParser = new Args();

            // Act
            Assert.Throws<ValueOptionFormatException>(() => CLParser.Parse(args));
        }


        [Fact]
        public void InvalidCaseEnumValueThrows()
        {
            // Arrange
            MyEnum val = MyEnum.Three;
            string[] args = { "--enum", val.ToString().ToUpper() };
            var CLParser = new EnumArgsCaseInsensitive();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.Equal(val, CLParser.EnumVal);
        }
    }
}
