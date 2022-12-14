using Xunit;

using Tests.Data;
using System;

// I had to set CShargs assembly InternalsVisibleTo("Tests")
// as Parser class, Option classes etc. are internal only
// and thus it is not possible to create their instance in
// a different project

namespace Tests
{
    public class FlagOption
    {
        [Theory]
        [InlineData(new string[] { "--verbose" }, true)]
        [InlineData(new string[] { "-v" }, true)]
        [InlineData(new string[] { "--verbose", "command" }, true)]
        [InlineData(new string[] { "command", "file" }, false)]
        public void IsFlagOptionPresent(string[] args, bool optionParsed)
        {
            // Arrange
            var arguments = new FlagArguments();

            // Act
            arguments.Parse(args);

            // Assert
            Assert.Equal(optionParsed, arguments.Verbose);
        }

        [Theory]
        [InlineData(new string[] { "--verbose" }, true, null)]
        [InlineData(new string[] { "-v" }, true, null)]
        [InlineData(new string[] { "--verbose", "command" }, true, null)]
        [InlineData(new string[] { "command", "file" }, false, null)]
        public void IsFlagOptionPresentForbidSpace(string[] args, bool optionParsed, Type exceptionType)
        {
            // Arrange
            var arguments = new FlagArgumentsForbidSpace();

            if (exceptionType == null) {
                // Act
                arguments.Parse(args);
                // Assert
                Assert.Equal(optionParsed, arguments.Verbose);
            } else {
                Assert.Throws(exceptionType, () => arguments.Parse(args));
            }
        }

        [Theory]
        [InlineData(new string[] { "--verbose" }, true, null)]
        [InlineData(new string[] { "-v" }, true, null)]
        [InlineData(new string[] { "--verbose", "command" }, true, null)]
        [InlineData(new string[] { "command", "file" }, false, null)]
        public void IsFlagOptionPresentForbidNoSpace(string[] args, bool optionParsed, Type exceptionType)
        {
            // Arrange
            var arguments = new FlagArgumentsForbidNoSpace();

            if (exceptionType == null) {
                // Act
                arguments.Parse(args);
                // Assert
                Assert.Equal(optionParsed, arguments.Verbose);
            } else {
                Assert.Throws(exceptionType, () => arguments.Parse(args));
            }
        }
        [Theory]
        [InlineData(new string[] { "--verbose" }, true, null)]
        [InlineData(new string[] { "-v" }, true, null)]
        [InlineData(new string[] { "--verbose", "command" }, true, null)]
        [InlineData(new string[] { "command", "file" }, false, null)]
        public void IsFlagOptionPresentForbidEquals(string[] args, bool optionParsed, Type exceptionType)
        {
            // Arrange
            var arguments = new FlagArgumentsForbidEquals();

            if (exceptionType == null) {
                // Act
                arguments.Parse(args);
                // Assert
                Assert.Equal(optionParsed, arguments.Verbose);
            } else {
                Assert.Throws(exceptionType, () => arguments.Parse(args));
            }
        }

        [Theory]
        [InlineData(new string[] { "-r" }, true)]
        [InlineData(new string[] { "-R" }, true)]
        [InlineData(new string[] { "--force", "-R", "file" }, true)]
        [InlineData(new string[] { "-rR" }, true)] 
        public void IsFlagOptionWithOneAliasPresent(string[] args, bool optionParsed)
        {
            // Arrange
            var arguments = new AliasFlagArguments();

            // Act
            arguments.Parse(args);

            // Assert
            Assert.Equal(optionParsed, arguments.Recursive);
        }

        [Theory]
        [InlineData(new string[] { "-a" }, true)]
        [InlineData(new string[] { "-rf" }, true)]
        [InlineData(new string[] { "-Rf" }, true)]
        [InlineData(new string[] { "-R" }, false)]
        [InlineData(new string[] { "-f" }, false)]
        [InlineData(new string[] { "file" }, false)]
        public void AreFlagOptionsWithAggregationAliasPresent(string[] args, bool optionsParsed)
        {
            // Arrange
            var arguments = new AliasFlagArguments();

            // Act
            arguments.Parse(args);

            // Assert
            Assert.Equal(optionsParsed, arguments.Recursive && arguments.Force);
        }
    } 
}
