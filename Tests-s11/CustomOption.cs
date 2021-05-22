using System;
using Xunit;

using Tests.Data;

// I had to set CShargs assembly InternalsVisibleTo("Tests")
// as Parser class, Option classes etc. are internal only
// and thus it is not possible to create their instance in
// a different project

namespace Tests
{
    public class CustomOption
    {
        [Theory]
        [InlineData(new string[] { "--pos", "1", "2", "3" }, new int[] { 1, 2, 3 })]
        public void HasCustomOptionWithParametersParsed(string[] args, int[] vectorValues)
        {
            // Arrange
            var arguments = new VectorArguments();

            // Act
            arguments.Parse(args);

            // Assert
            Assert.Equal(vectorValues[0], arguments.Position.First);
            Assert.Equal(vectorValues[1], arguments.Position.Second);
            Assert.Equal(vectorValues[2], arguments.Position.Third);
        }

        [Fact]
        public void DoesInvalidParameterCountThrowException()
        {
            // Arrange
            var arguments = new VectorArguments();

            // Act and assert
            Assert.Throws<FormatException>(() => arguments.Parse(new string[] { "--pos", "1", "2" }));
        }
    }
}
