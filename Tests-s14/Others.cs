using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using CShargs;

namespace s14_api_testing
{
    public class Others
    {
        class Args : Parser
        {
            [FlagOption("option", shortName: 'o', help: "This is help message")]
            public bool Option { get; set; }

            [FlagOption("lastflag", shortName: 'l')]
            public bool LastFlag { get; set; }
        }

        class BadParser : Parser
        {
            [FlagOption("zelenina", shortName: 'c', 
                help: "Test invalid parser construction\n" +
                "It should throw exception\n" +
                "Intended behaviour was not specified in documentation.")]
            public bool Cibula { get; set; }

            [FlagOption("zelenina", shortName: 'm', help: "Help string")]
            public bool Mrkva { get; set; }
        }

        [Fact]
        public void HelpTextShouldBePresentInGeneratedHelp()
        {
            // Arrange
            var CLParser = new Args();
            // Assert
            Assert.True(CLParser.GenerateHelp().Length > 0);
        }

        [Theory]
        [InlineData("--")]
        [InlineData("--", "foo")]
        [InlineData("--", "foo", "bar")]
        public void PlainArgumentsShouldBeReturned(params string[] args)
        {
            // Arrange
            var CLParser = new Args();

            // Act & Assign
            foreach(var arg in args)
            {
                Assert.Contains(arg, CLParser.PlainArgs);
            }
        }

        [Fact]
        public void DuplicateLongOptionShouldThrowConfigurationExceptiom()
        {
            // Arrange
            string[] args = { };

            // Assert
            Assert.Throws<ConfigurationException>(() => { var CLParser = new BadParser(); });
            //ShortOption duplicate should be handled exactly same
        }

        [Fact]
        public void OptionAfterDelimiterShouldNotBeProvided()
        {
            // Arrange
            string[] args = { "-o", "--", "--lastflag" };
            var CLParser = new Args();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.False(CLParser.LastFlag);
        }
    }
}
