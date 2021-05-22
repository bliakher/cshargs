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
            public const string HelpMessage = "This is help message";

            [FlagOption("option", shortName: 'o', help: HelpMessage)]
            public bool Option { get; set; }

            [FlagOption("lastflag", shortName: 'l')]
            public bool LastFlag { get; set; }
        }

        class BadParserDuplicateLong : Parser
        {
            [FlagOption("zelenina", shortName: 'c',
                help: "Test invalid parser construction")]
            public bool Cibula { get; set; }

            [FlagOption("zelenina", shortName: 'm', help: "Help string")]
            public bool Mrkva { get; set; }
        }

        class BadParserDuplicateShort : Parser
        {
            [FlagOption("cibula", shortName: 'm',
                help: "Test invalid parser construction")]
            public bool Cibula { get; set; }

            [FlagOption("mrkva", shortName: 'm', help: "Help string")]
            public bool Mrkva { get; set; }
        }

        [Fact]
        public void HelpWordsPresentInGeneratedHelp()
        {
            // Arrange
            var CLParser = new Args();
            // Assert
            Assert.True(CLParser.GenerateHelp().Length > 0);
            foreach (var word in Args.HelpMessage.Split(' ')) {
                Assert.Contains(word, Args.HelpMessage);
            }
        }

        [Theory]
        [InlineData("--")]
        [InlineData("--", "foo")]
        [InlineData("--", "foo", "bar")]
        public void PostDelimiterPlainArgsCollected(params string[] args)
        {
            // Arrange
            var parser = new Args();

            // Act
            parser.Parse(args);

            // Assert
            parser.PlainArgs.SequenceEqual(args.Skip(1));
        }

        [Fact]
        public void DuplicateLongThrows()
        {
            // Arrange
            string[] args = { };

            // Assert
            Assert.Throws<ConfigurationException>(() => { var CLParser = new BadParserDuplicateLong(); });
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
