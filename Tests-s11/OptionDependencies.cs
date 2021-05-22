using Xunit;

using Tests.Data;

// I had to set CShargs assembly InternalsVisibleTo("Tests")
// as Parser class, Option classes etc. are internal only
// and thus it is not possible to create their instance in
// a different project
using CShargs;

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
            Assert.Throws<OptionDependencyError>(() => arguments.Parse(new string[] { "-v", "command"})); // missing -p
        }
    }
}
