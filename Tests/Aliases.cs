using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using CShargs;

namespace s14_api_testing
{
    public class Aliases
    {
        [AliasOption("x", nameof(Execute))]
        class MyArguments : Parser
        {
            [FlagOption("execute", shortName: 'r')]
            public bool Execute { get; set; }
        }

        [Fact]
        public void AliasReturnSameThing()
        {
            // Assign
            string[] args1 = { "-x" };
            string[] args2 = { "-r" };
            var parser1 = new MyArguments();
            var parser2 = new MyArguments();

            // Act
            parser1.Parse(args1);
            parser2.Parse(args2);

            // Assert
            Assert.Equal(parser1.Execute, parser2.Execute);
        }
    }
}
