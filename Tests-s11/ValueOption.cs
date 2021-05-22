using Xunit;

using Tests.Data;

// I had to set CShargs assembly InternalsVisibleTo("Tests")
// as Parser class, Option classes etc. are internal only
// and thus it is not possible to create their instance in
// a different project
using CShargs;

namespace Tests
{
    public class ValueOption
    {
        internal void HasOptionalValueOptionParameterParsed<T>(string[] args, FileOptionalArguments arguments, T expectedParameterValue, T optionProperty)
        {
            // Act
            arguments.Parse(args);

            // Assert
            Assert.Equal(expectedParameterValue, optionProperty);
        }

        [Theory]
        [InlineData(new string[] { "--output=File.out" }, "File.out")]
        [InlineData(new string[] { "-o File.out" }, "File.out")]
        [InlineData(new string[] { "-oFile.out" }, "File.out")]
        [InlineData(new string[] { "command" }, "")]
        public void HasOptionalValueOptionStringParameterParsed(string[] args, string parameterValue)
        {
            // Arrange
            var arguments = new FileOptionalArguments();

            // Act and assert
            HasOptionalValueOptionParameterParsed<string>(args, arguments, parameterValue, arguments.OutputFile);
        }

        [Theory]
        [InlineData(new string[] { "--number-of-muskrats=4" }, 4)]
        [InlineData(new string[] { "--number-of-muskrats", "4" }, 4)]
        [InlineData(new string[] { "-m", "4" }, 4)]
        [InlineData(new string[] { "-m4" }, 4)]
        [InlineData(new string[] { "-m=4" }, 4)]
        [InlineData(new string[] { "command" }, 1)] // default parameter value used as option is not required
        public void HasOptionalValueOptionIntParameterParsed(string[] args, int parameterValue)
        {
            // Arrange
            var arguments = new FileOptionalArguments();

            // Act and assert
            HasOptionalValueOptionParameterParsed<int>(args, arguments, parameterValue, arguments.Muskrats);
        }

        [Theory]
        [InlineData(new string[] { "--full-name=Freddie;Mercury" }, new string[] { "Freddie", "Mercury" })]
        [InlineData(new string[] { "--full-name", "Roger;Taylor" }, new string[] { "Roger", "Taylor" })]
        [InlineData(new string[] { "-nBrian;May" }, new string[] { "Brian", "May" })]
        [InlineData(new string[] { "-n", "John;Deacon" }, new string[] { "John", "Deacon" })]
        public void HasOptionalValueOptionUserDefinedParameterParsed(string[] args, string[] parameterValueParts)
        {
            // Arrange
            var arguments = new FileOptionalArguments();
            var parameterValue = new FullName(parameterValueParts[0], parameterValueParts[1]);

            // Act and assert
            HasOptionalValueOptionParameterParsed<FullName>(args, arguments, parameterValue, arguments.Name);
        }

        [Fact]
        public void DoesMissingMandatoryValueOptionThrowException()
        {   
            // Arrange
            var arguments = new FileMandatoryArguments();

            // Act and assert
            Assert.Throws<MissingOptionException>(() => arguments.Parse(new string[] { "command" }));
        }
    }
}
