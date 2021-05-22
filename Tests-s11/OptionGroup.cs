﻿using Xunit;

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
        public void DoesMissingMandatoryOptionGroupOptionThrowException()
        {
            // Arrange
            var arguments = new CountGroupArguments();

            // Act and assert
            Assert.Throws<MissingOptionGroupException>(() => arguments.Parse(new string[] { "--verbose", "file" }));
        }

        [Fact]
        public void DoesSettingRequiredOptionInsideOptionGroupThrowException()
        {
            // Arrange
            var arguments = new InvalidConfigurationGroupArguments();

            // Act and assert
            Assert.Throws<ConfigurationException>(() => arguments.Parse(new string[] { "--input", "file" }));
        }
    }
}