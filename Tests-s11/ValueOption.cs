using Xunit;

using Tests.Data;

// I had to set CShargs assembly InternalsVisibleTo("Tests")
// as Parser class, Option classes etc. are internal only
// and thus it is not possible to create their instance in
// a different project
using CShargs;
using System;

namespace Tests
{
    public class ValueOption
    {
        [Theory]
        [InlineData(new string[] { "--output=File.out" }, "File.out")]
        [InlineData(new string[] { "--output", "File.out" }, "File.out")]
        [InlineData(new string[] { "-o", "File.out" }, "File.out")]
        [InlineData(new string[] { "-oFile.out" }, "File.out")]
        [InlineData(new string[] { "-o=File.out" }, "File.out")]
        [InlineData(new string[] { "command" }, null)]
        public void ValueOptionStringParsed(string[] args, string parsedValue, Type exceptionType = null)
        {
            // Arrange
            var arguments = new FileOptionalArguments();

            if (exceptionType == null) {
                // Act
                arguments.Parse(args);
                // Assert
                Assert.Equal(parsedValue, arguments.OutputFile);
            } else {
                Assert.Throws(exceptionType, () => arguments.Parse(args));
            }
        }
        [Theory]
        [InlineData(new string[] { "--output=File.out" }, "File.out")]
        [InlineData(new string[] { "--output", "File.out" }, null, typeof(MissingOptionValueException))]
        [InlineData(new string[] { "-o", "File.out" }, null, typeof(MissingOptionValueException))]
        [InlineData(new string[] { "-oFile.out" }, "File.out")]
        [InlineData(new string[] { "-o=File.out" }, "File.out")]
        [InlineData(new string[] { "command" }, null)]
        public void ValueOptionStringParsedForbidSpace(string[] args, string parsedValue, Type exceptionType = null)
        {
            // Arrange
            var arguments = new FileOptionalArgumentsForbidSpace();

            if (exceptionType == null) {
                // Act
                arguments.Parse(args);
                // Assert
                Assert.Equal(parsedValue, arguments.OutputFile);
            } else {
                Assert.Throws(exceptionType, () => arguments.Parse(args));
            }
        }
        [Theory]
        [InlineData(new string[] { "--output=File.out" }, "File.out")]
        [InlineData(new string[] { "--output", "File.out" }, "File.out")]
        [InlineData(new string[] { "-o", "File.out" }, "File.out")]
        [InlineData(new string[] { "-oFile.out" }, null, typeof(UnknownOptionException))]
        [InlineData(new string[] { "-o=File.out" }, "File.out")]
        [InlineData(new string[] { "command" }, null)]
        public void ValueOptionStringParsedForbidNoSpace(string[] args, string parsedValue, Type exceptionType = null)
        {
            // Arrange
            var arguments = new FileOptionalArgumentsForbidNoSpace();

            if (exceptionType == null) {
                // Act
                arguments.Parse(args);
                // Assert
                Assert.Equal(parsedValue, arguments.OutputFile);
            } else {
                Assert.Throws(exceptionType, () => arguments.Parse(args));
            }
        }
        [Theory]
        [InlineData(new string[] { "--output=File.out" }, null, typeof(UnknownOptionException))]
        [InlineData(new string[] { "-o", "File.out" }, "File.out", null)]
        [InlineData(new string[] { "-oFile.out" }, "File.out", null)]
        [InlineData(new string[] { "-o=File.out" }, "=File.out")]
        [InlineData(new string[] { "command" }, null, null)]
        public void ValueOptionStringParsedForbidEquals(string[] args, string parsedValue, Type exceptionType = null)
        {
            // Arrange
            var arguments = new FileOptionalArgumentsForbidEquals();

            if (exceptionType == null) {
                // Act
                arguments.Parse(args);
                // Assert
                Assert.Equal(parsedValue, arguments.OutputFile);
            } else {
                Assert.Throws(exceptionType, () => arguments.Parse(args));
            }
        }
        [Theory]
        [InlineData(new string[] { "--output=File.out" }, null, typeof(UnknownOptionException))]
        [InlineData(new string[] { "-o", "File.out" }, "File.out", null)]
        [InlineData(new string[] { "-oFile.out" }, null, typeof(UnknownOptionException))]
        [InlineData(new string[] { "-o=File.out" }, null, typeof(UnknownOptionException))]
        [InlineData(new string[] { "command" }, null, null)]
        public void ValueOptionStringParsedForbidEqualsNoSpace(string[] args, string parsedValue, Type exceptionType = null)
        {
            // Arrange
            var arguments = new FileOptionalArgumentsForbidEqualsNoSpace();

            if (exceptionType == null) {
                // Act
                arguments.Parse(args);
                // Assert
                Assert.Equal(parsedValue, arguments.OutputFile);
            } else {
                Assert.Throws(exceptionType, () => arguments.Parse(args));
            }
        }

        [Theory]
        [InlineData(new string[] { "--number-of-muskrats=4" }, true, 4)]
        [InlineData(new string[] { "--Number-Of-Muskrats=4" }, false, 4)]
        [InlineData(new string[] { "--number-of-muskrats", "4" }, true, 4)]
        [InlineData(new string[] { "--Number-Of-Muskrats", "4" }, false, 4)]
        [InlineData(new string[] { "-m", "4" }, true, 4)]
        [InlineData(new string[] { "-M", "4" }, false, 4)]
        [InlineData(new string[] { "-m4" }, true, 4)]
        [InlineData(new string[] { "-M4" }, false, 4)]
        [InlineData(new string[] { "-m=4" }, true, 4)]
        [InlineData(new string[] { "-M=4" }, false, 4)]
        [InlineData(new string[] { "command" }, true, 1)] // default parameter value used as option is not required
        public void ValueOptionIntParsed(string[] args, bool expectSuccess, int parameterValue)
        {
            // Arrange
            var arguments = new FileOptionalArguments();
            // Assert
            if (expectSuccess) {
                arguments.Parse(args);
                Assert.Equal(parameterValue, arguments.Muskrats);
            } else {
                Assert.Throws<UnknownOptionException>(() => arguments.Parse(args));
            }
        }

        [Theory]
        [InlineData(new string[] { "--number-of-muskrats=4" }, true, 4)]
        [InlineData(new string[] { "--Number-Of-Muskrats=4" }, false, 4)]
        [InlineData(new string[] { "--number-of-muskrats", "4" }, true, 4)]
        [InlineData(new string[] { "--Number-Of-Muskrats", "4" }, false, 4)]
        [InlineData(new string[] { "-m", "4" }, true, 4)]
        [InlineData(new string[] { "-M", "4" }, true, 4)]
        [InlineData(new string[] { "-m4" }, true, 4)]
        [InlineData(new string[] { "-M4" }, true, 4)]
        [InlineData(new string[] { "-m=4" }, true, 4)]
        [InlineData(new string[] { "-M=4" }, true, 4)]
        [InlineData(new string[] { "command" }, true, 1)] // default parameter value used as option is not required
        public void ValueOptionIntParsed_ShortCI(string[] args, bool expectSuccess, int parameterValue)
        {
            // Arrange
            var arguments = new FileOptionalArguments_ShortCI();
            // Assert
            if (expectSuccess) {
                arguments.Parse(args);
                Assert.Equal(parameterValue, arguments.Muskrats);
            } else {
                Assert.Throws<UnknownOptionException>(() => arguments.Parse(args));
            }
        }

        [Theory]
        [InlineData(new string[] { "--number-of-muskrats=4" }, true, 4)]
        [InlineData(new string[] { "--Number-Of-Muskrats=4" }, true, 4)]
        [InlineData(new string[] { "--number-of-muskrats", "4" }, true, 4)]
        [InlineData(new string[] { "--Number-Of-Muskrats", "4" }, true, 4)]
        [InlineData(new string[] { "-m", "4" }, true, 4)]
        [InlineData(new string[] { "-M", "4" }, false, 4)]
        [InlineData(new string[] { "-m4" }, true, 4)]
        [InlineData(new string[] { "-M4" }, false, 4)]
        [InlineData(new string[] { "-m=4" }, true, 4)]
        [InlineData(new string[] { "-M=4" }, false, 4)]
        [InlineData(new string[] { "command" }, true, 1)] // default parameter value used as option is not required
        public void ValueOptionIntParsed_LongCI(string[] args, bool expectSuccess, int parameterValue)
        {
            // Arrange
            var arguments = new FileOptionalArguments_LongCI();
            // Assert
            if (expectSuccess) {
                arguments.Parse(args);
                Assert.Equal(parameterValue, arguments.Muskrats);
            } else {
                Assert.Throws<UnknownOptionException>(() => arguments.Parse(args));
            }
        }

        [Theory]
        [InlineData(new string[] { "--full-name=Freddie;Mercury" }, new string[] { "Freddie", "Mercury" })] // <3
        [InlineData(new string[] { "--full-name", "Roger;Taylor" }, new string[] { "Roger", "Taylor" })]
        [InlineData(new string[] { "-nBrian;May" }, new string[] { "Brian", "May" })]
        [InlineData(new string[] { "-n", "John;Deacon" }, new string[] { "John", "Deacon" })]
        public void ValueOptionUserTypeParsed(string[] args, string[] expectedParts)
        {
            // Arrange
            var arguments = new FileOptionalArguments();
            var expecteValue = new FullName(expectedParts[0], expectedParts[1]);
            // Act
            arguments.Parse(args);
            // Assert
            Assert.Equal(expecteValue, arguments.Name);
        }

        [Fact]
        public void MissingMandatoryValueOptionThrows()
        {
            // Arrange
            var arguments = new FileMandatoryArguments();

            // Act and assert
            Assert.Throws<MissingOptionException>(() => arguments.Parse(new string[] { "command" }));
        }
    }
}
