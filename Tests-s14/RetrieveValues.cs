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

            [ValueOption("epsilon", shortName: 'e', help: "Specify long ")]
            public long Epsilon { get; set; }
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
        public void ReturnShortOptionValue()
        {
            // Arrange
            string[] args = { "-d", "13" };
            var CLParser = new Args();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.Equal(13, CLParser.Delta);
        }
        [Fact]
        public void ReturnLongOptionValue()
        {
            // Arrange
            string[] args = { "-e", "5000000000" };
            var CLParser = new Args();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.Equal(5000000, CLParser.Epsilon);
        }
    }
}
