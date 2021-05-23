using Xunit;

using Tests.Data;

// I had to set CShargs assembly InternalsVisibleTo("Tests")
// as Parser class, Option classes etc. are internal only
// and thus it is not possible to create their instance in
// a different project

namespace Tests
{
    public class VerbOption
    {
        [Theory]
        [InlineData(new string[] {"push", "--force" }, true)]
        [InlineData(new string[] {"push" }, false)]
        public void IsSubcommandFlagOptionPresent(string[] args, bool subcommandPresent)
        {
            // Arrange
            var arguments = new CommandArguments();

            // Act
            arguments.Parse(args);

            // Assert
            Assert.Equal(subcommandPresent, arguments.Push.Force);
        }

        [Theory]
        [InlineData(new string[] { "process", "--output=File.out" }, "File.out")]
        [InlineData(new string[] { "process", "-o", "File.out" }, "File.out")]
        [InlineData(new string[] { "process", "-oFile.out" }, "File.out")]
        [InlineData(new string[] { "process" }, null)]
        public void HasSubcommandValueOptionParameterParsed(string[] args, string parameterValue)
        {
            // Arrange
            var arguments = new CommandArguments();

            // Act
            arguments.Parse(args);

            // Assert
            Assert.Equal(parameterValue, arguments.Process.OutputFile);
        }
    }
}
