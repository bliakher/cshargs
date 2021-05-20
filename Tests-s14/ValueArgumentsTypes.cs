using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using CShargs;

namespace s14_api_testing
{
    public class ValueArgumentsTypes
    {
        class Arguments : Parser
        {

            [ValueOption("alpha", shortName: 'a', help: "Specify number alpha")]
            public int Alpha { get; set; }

            [ValueOption("beta", shortName: 'b', help: "Specify string beta")]
            public string Beta { get; set; }

            [ValueOption("gamma", shortName: 'g', help: "Specify bool gamma")]
            public bool Gamma { get; set; }

            [ValueOption("delta", shortName: 'd', help: "Specify short delta")]
            public short Delta { get; set; }

            [ValueOption("epsilon", shortName: 'e', help: "Specify long ")]
            public long Epsilon { get; set; }

            public enum ZetaTest
            {
                One,
                TWO
            }

            [ValueOption("zeta", shortName: 'z', help: "Specify enum zeta")]
            public ZetaTest Zeta { get; set; }

            [ValueOption("eta", help: "Specify nullable bool eta")]
            public bool? Eta { get; set; }
        }
        [Fact]
        public void IntIsAssignedAsInt()
        {
            // Arrange
            string[] args = { "-a", "10000"};
            var CLParser = new Arguments();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.IsType<int>(CLParser.Alpha);
        }

        [Fact]
        public void OtherTypeAssignedToInt_ThrowException()
        {
            // Arrange
            string[] args = { "-a", "ThisIsString" };
            var CLParser = new Arguments();

            // Act & Assert
            Assert.Throws<ValueOptionFormatException>(() => { CLParser.Parse(args); });
        }

        [Fact]
        public void StringIsAssignedAsString()
        {
            // Assign
            string[] args = { "-b", "test string" };
            var CLParser = new Arguments();

            // Act
            CLParser.Parse(args);

            //Assert
            Assert.IsType<string>(CLParser.Beta);
        }
        [Fact]
        public void BoolIsAssignedAsBool()
        {
            // Assign
            string[] args = { "-g", "true" };
            var CLParser = new Arguments();

            // Act
            CLParser.Parse(args);

            //Assert
            Assert.IsType<bool>(CLParser.Gamma);
        }
        [Fact]
        public void ShortIsAssignedAsShort()
        {
            // Assign
            string[] args = { "-d", "4" };
            var CLParser = new Arguments();

            // Act
            CLParser.Parse(args);

            //Assert
            Assert.IsType<short>(CLParser.Delta);
        }

        //%%%%%%%%%%%%%%%%%%%%%%% EXPECTED TO BE REMOVED %%%%%%%%%%%%%%%%%%%%%%
        /*[Fact]
        public void IntIsAssignedAsShort_ThrowException()
        {
            // Assign
            string[] args = { "-d", "4" };
            var CLParser = new Arguments();

            //Act
            var e = Assert.Throws<ParsingException>(() => CLParser.Parse(args));

            // Assert act
            Assert.Equal("a", e.Message);
        }

        [Fact]
        public void LongIsAssignedAsLong()
        {
            // Assign
            string[] args = { "-e", "5000000000" };
            var CLParser = new Arguments();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.IsType<long>(CLParser.Epsilon);
        }*/

        // %%%%%%%%%%%%%%%%% NEED TO BE RECONSIDERED %%%%%%%%%%%%%%%%%%%%%%%%%%
        /*[Fact]
        public void EnumIsAssignedAsEnum()
        {
            // Assign
            string[] args = { "-z", "One" };
            var CLParser = new Arguments();

            // Act
            CLParser.Parse(args);

            // Assert
        }*/

        [Fact]
        public void NullableBoolsWorkingProperly()
        {
            // Assign
            string[] args = { "--eta" };
            var CLParser = new Arguments();

            // Act
            CLParser.Parse(args);

            // Assert
            Assert.Null(CLParser.Eta);
        }
    }
}
