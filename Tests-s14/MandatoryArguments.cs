using System;
using Xunit;
using CShargs;


namespace s14_api_testing
{
    public class MandatoryArguments
    {
        class TargetWithRequiredFalse : Parser
        {
            [ValueOption("severity", shortName:'s', required: false)]
            public string Severity { get; set; }
        }
        class TargetWithRequiredTrue : Parser
        {
            [ValueOption("severity", shortName: 's', required: true)]
            public string Severity { get; set; }
        }

        [Fact]
        public void ParseCommandLine_OptionalArgumentFalse_SuppliedArguments()
        {
            // Assign
            string[] args = { "-s", "test" };
            var CLParser = new TargetWithRequiredFalse();

            // Act
            CLParser.Parse(args);


            // Assert
            Assert.Equal("test", CLParser.Severity);
        }

        [Fact]
        public void ParseCommandLine_OptionalArgumentTrue_SuppliedArguments()
        {
            // Assign
            string[] args = { "-s", "test" };
            var CLParser = new TargetWithRequiredTrue();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.Equal("test", CLParser.Severity);
        }

        [Fact]
        public void OptionalArgumentTrue_MissingArgument_Works()
        {
            string[] args= { "-s" };
            var CLParser = new TargetWithRequiredFalse();

            CLParser.Parse(args);

            Assert.Null(CLParser.Severity);
        }

        [Theory]
        [InlineData("-s")]
        public void OptionalArgTrue_Missing_ThrowsExcept(params string[] args)
        {
            // Assign
            var CLParser = new TargetWithRequiredTrue();

            // Act & Assert
            Assert.Throws<MissingOptionException>(() => { CLParser.Parse(args); });
        }
    }
}
