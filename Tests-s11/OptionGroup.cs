using Xunit;

using Tests.Data;

// I had to set CShargs assembly InternalsVisibleTo("Tests")
// as Parser class, Option classes etc. are internal only
// and thus it is not possible to create their instance in
// a different project
using CShargs;

namespace Tests
{
    public class OptionGroup
    {
        [Theory]
        [InlineData(new string[] { "--words", "file"}, true)]
        [InlineData(new string[] { "--lines", "file" }, true)]
        public void HasExactlyOneOfGroupOptionsParsed(string[] args, bool oneOptionParsed)
        {
            // Arrange
            var arguments = new CountGroupArguments();

            // Act
            arguments.Parse(args);

            // Assert
            Assert.Equal(oneOptionParsed, 
                        (arguments.Lines && !arguments.Words) || (!arguments.Lines && arguments.Words));
        }

        [Fact]
        public void MissingMandatoryOptionGroupThrows()
        {
            // Arrange
            var arguments = new CountGroupArguments();

            // Act and assert
            Assert.Throws<MissingGroupException>(() => arguments.Parse(new string[] { "--verbose", "file" }));
        }

        [Theory]
        [InlineData(new[] { "file.txt" }, false)]
        [InlineData(new[] { "file.txt", "--print", "--tostderr" }, false)]
        [InlineData(new[] { "file.txt", "--print", "--tofile=out.txt" }, false)]
        [InlineData(new[] { "file.txt", "--tostderr" }, true)]
        [InlineData(new[] { "file.txt", "--tofile=out.txt" }, true)]
        public void MissingGroupDependencyThrows(string[] args, bool expectThrows)
        {
            // Arrange
            var parser = new GroupDependenciesArguments();

            // Act and assert
            if (expectThrows) {
                Assert.Throws<MissingDependencyException>(() => parser.Parse(args));
            } else {
                parser.Parse(args);
            }
        }
    }
}
