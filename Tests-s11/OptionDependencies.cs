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
    public class OptionDependencies
    {
        [Fact]
        public void MissingDependencyThrows()
        {
            // Arrange
            var arguments = new OptionDependenciesArguments();

            // Act and assert
            Assert.Throws<MissingDependencyException>(() => arguments.Parse(new string[] { "-v", "command" })); // missing -p
        }


        [Theory]
        [InlineData(new string[] { }, false)]
        [InlineData(new[] { "file1.txt", "file2.txt" }, false)]
        [InlineData(new[] { "file1.txt", "file2.txt", "--print", "--out=stdout" }, false)]
        [InlineData(new[] { "file1.txt", "file2.txt", "--print" }, true)]
        [InlineData(new[] { "--print" }, true)]
        public void MissingDependencyDisablesRequired(string[] args, bool expectThrows)
        {
            // Arrange
            var parser = new RequiredDependenciesArguments();

            if (expectThrows) {
                Assert.Throws<MissingOptionException>(() => parser.Parse(args));
            } else {
                parser.Parse(args);
                if (parser.Print) {
                    Assert.NotNull(parser.Output);
                }
            }
        }

        [Theory]
        [InlineData(new string[] { }, null)]
        [InlineData(new[] { "file1.txt", "file2.txt" }, null)]
        [InlineData(new[] { "file1.txt", "file2.txt", "--print", "--tostdout" }, null)]
        [InlineData(new[] { "file1.txt", "file2.txt", "--print", "--tostderr" }, null)]
        [InlineData(new[] { "file1.txt", "file2.txt", "--print", "--tofile=output.txt" }, null)]
        [InlineData(new[] { "file1.txt", "file2.txt", "--tostdout" }, typeof(MissingDependencyException))]
        [InlineData(new[] { "file1.txt", "file2.txt", "--tostderr" }, typeof(MissingDependencyException))]
        [InlineData(new[] { "file1.txt", "file2.txt", "--tofile=output.txt" }, typeof(MissingDependencyException))]
        [InlineData(new[] { "file1.txt", "file2.txt", "--print" }, typeof(MissingGroupException))]
        [InlineData(new[] { "--print" }, typeof(MissingGroupException))]
        public void MissingDependencyDisablesRequiredOfGroup(string[] args, Type expectedException)
        {
            // Arrange
            var parser = new RequiredGroupDependenciesArguments();

            if (expectedException != null) {
                Assert.Throws(expectedException, () => parser.Parse(args));
            } else {
                parser.Parse(args);
                if (parser.Print) {
                    Assert.True(parser.ToFile != null || parser.ToStdErr || parser.ToStdOut);
                }
            }
        }
    }
}
