using CShargs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Data
{
    class BadParserDuplicateLong : Parser
    {
        [FlagOption("zelenina", shortName: 'c',
            help: "Test invalid parser construction")]
        public bool Cibula { get; set; }

        [FlagOption("zelenina", shortName: 'm', help: "Help string")]
        public bool Mrkva { get; set; }
    }

    class BadParserDuplicateShort : Parser
    {
        [FlagOption("cibula", shortName: 'm',
            help: "Test invalid parser construction")]
        public bool Cibula { get; set; }

        [FlagOption("mrkva", shortName: 'm', help: "Help string")]
        public bool Mrkva { get; set; }
    }

    [ParserConfig(shortOptionSymbol: "-", longOptionSymbol: "-")]
    class BadParserAggregateSameSymbols : Parser
    {
        [FlagOption("cibula", shortName: 'c')]
        public bool Cibula { get; set; }

        [FlagOption("mrkva", shortName: 'm')]
        public bool Mrkva { get; set; }
    }


    [AliasOption("r", nameof(Execute))]
    class BadParserAliasConflict : Parser
    {
        [FlagOption("execute", shortName: 'r')]
        public bool Execute { get; set; }
    }

    class BadParserPrivateOptions : Parser
    {
        [FlagOption("execute", shortName: 'r')]
        bool Execute { get; set; }
    }
}
