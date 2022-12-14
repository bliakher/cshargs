using Xunit;

using Tests.Data;

// I had to set CShargs assembly InternalsVisibleTo("Tests")
// as Parser class, Option classes etc. are internal only
// and thus it is not possible to create their instance in
// a different project
using CShargs;

namespace Tests
{
    public class Configuration
    {
        [Theory]
        [InlineData(new string[] { "--verbose", "command1", "command2", "command3" }, 3)]
        [InlineData(new string[] { "command1", "command2", "--verbose", "command3" }, 3)]
        [InlineData(new string[] { "--verbose", "--", "command1", "command2" }, 2)]
        [InlineData(new string[] { "--", "--verbose", "command2", "command3" }, 3)]
        public void HasAllPlainArgumentsParsed(string[] args, int plainArgsCount)
        {
            // Arrange
            var arguments = new FlagArguments();

            // Act
            arguments.Parse(args);

            // Assert
            Assert.Equal(plainArgsCount, arguments.PlainArgs.Count);
        }

        [Theory]
        [InlineData(new string[] { "--verbose", "command1", "command2" }, 2)]
        [InlineData(new string[] { "--verbose", "--", "command1", "command2" }, 2)]
        [InlineData(new string[] { "--", "--verbose", "command1" }, 2)]
        public void InvalidNumberOfPlainArgumentsThrows(string[] args, int actualPlainArgsCount) // second parameter only prevents InlineData error
        {
            // Arrange
            var arguments = new FixedCountPlainArguments();

            // Act and assert
            Assert.Throws<PlainArgsCountException>(() => arguments.Parse(args));
        }

        [Theory]
        [InlineData(new string[] { "--verbose", "command1", "command2", "command3" }, 2, false)]
        [InlineData(new string[] { "command1", "--verbose", "--", "command2" }, 2, false)]
        [InlineData(new string[] { "--", "command1", "command2", "--verbose"}, 2, true)] // the delimiter gets skipped
        [InlineData(new string[] { "command1", "--verbose", "command2" }, 1, true)]
        public void HasOptionBeenSkipped(string[] args, int argsToSkip, bool optionPresent) // look into
        {
            // Arrange
            var arguments = new SkippedArguments(argsToSkip);
            // Act
            arguments.Parse(args);
            // Assert
            Assert.Equal(optionPresent, arguments.Verbose);
        }


        [Fact]
        public void DuplicateLongThrows()
        {
            // Assert
            Assert.Throws<ConfigurationException>(() => { var CLParser = new BadParserDuplicateLong(); });
        }
        [Fact]
        public void DuplicateShortThrows()
        {
            // Assert
            Assert.Throws<ConfigurationException>(() => { var CLParser = new BadParserDuplicateShort(); });
        }
        [Fact]
        public void AggregationWithSameSymbolsThrows()
        {
            // Assert
            Assert.Throws<ConfigurationException>(() => { var p = new BadParserAggregateSameSymbols(); });
        }

        [Fact]
        public void AliasConflictThrows()
        {
            Assert.Throws<ConfigurationException>(() => { var p = new BadParserAliasConflict(); });
        }

        [Fact]
        public void PrivateOptionsSetterThrows()
        {
            Assert.Throws<ConfigurationException>(() => { var p = new BadParserPrivateOptions(); });
        }

        [Fact]
        public void RequiredOptionInsideOptionGroupThrows()
        {
            // Arrange
            // Act and assert
            Assert.Throws<ConfigurationException>(() => new InvalidConfigurationGroupArguments());
        }

        [Fact]
        public void NonSubparserVerbThrows()
        {
            // Arrange
            // Act and assert
            Assert.Throws<ConfigurationException>(() => new InvalidConfigurationNonParserVerb());
        }
    }
}
