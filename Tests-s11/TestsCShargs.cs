using System;
using Xunit;

using Tests.Data;

// I had to set CShargs assembly InternalsVisibleTo("Tests")
// as Parser class, Option classes etc. are internal only
// and thus it is not possible to create their instance in
// a different project
using CShargs;

namespace Tests
{
    public class TestFlagOption
    {
        [Theory]
        [InlineData(new string[]{"--verbose"}, true)]
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
        [InlineData(new string[] { "-r" }, true)]
        [InlineData(new string[] { "-R" }, true)]
        [InlineData(new string[] { "--force", "-R", "file" }, true)]
        [InlineData(new string[] { "-rR" }, false)] // really false?
        public void IsFlagOptionWithOneAliasPresent(string[] args, bool optionParsed) // look into
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
        [InlineData(new string[] { "-RF" }, false)]
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

    public class TestValueOption
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

    public class TestVerbOption
    {
        [Theory]
        [InlineData(new string[] { "git", "push", "--force" }, true)]
        [InlineData(new string[] { "git", "push" }, false)]
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
        [InlineData(new string[] { "process", "-o File.out" }, "File.out")]
        [InlineData(new string[] { "process", "-oFile.out" }, "File.out")]
        [InlineData(new string[] { "process" }, "")]
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

    public class TestCustomOption
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

    public class TestParserConfiguration
    {
        [Theory]
        [InlineData(new string[] { "--verbose", "command1", "command2", "command3" }, 3)]
        [InlineData(new string[] { "--verbose", "--", "command1", "command2" }, 2)]
        [InlineData(new string[] { "--", "--verbose", "command1", "command2" }, 3)]
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
        public void DoesInvalidNumberOfPlainArgumentsThrowException(string[] args, int actualPlainArgsCount) // second parameter only prevents InlineData error
        {
            // Arrange
            var arguments = new FixedCountPlainArguments();

            // Act and assert
            Assert.Throws<PlainArgsCountException>(() => arguments.Parse(args));
        }

        [Theory]
        [InlineData(new string[] { "--verbose", "command1", "command2", "command3" }, 2, true)]
        [InlineData(new string[] { "command1", "--verbose", "--", "command2" }, 2, true)]
        [InlineData(new string[] { "--", "command1", "command2", "--verbose"}, 2, false)] // skipped because of the delimiter
        [InlineData(new string[] { "command1", "--verbose", "command2" }, 1, true)]
        public void HasOptionBeenSkipped(string[] args, int argsToSkip, bool optionSkipped) // look into
        {
            // Arrange
            var arguments = new SkippedArguments(argsToSkip);

            // Act
            arguments.Parse(args);

            // Assert
            Assert.Equal(optionSkipped, arguments.Verbose);
        }
    }

    public class TestOptionGroup
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

    public class TestOptionDependencies
    {
        [Fact]
        public void DoesUsageOfDependentOptionWithoutItsDependencyThrowException()
        {
            // Arrange
            var arguments = new OptionDependenciesArguments();

            // Act and assert
            Assert.Throws<ParsingException>(() => arguments.Parse(new string[] { "-v", "command"}));
        }
    }
}
