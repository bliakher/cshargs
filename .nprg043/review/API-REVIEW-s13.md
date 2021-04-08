## Numactl ##

```cs
using System;

namespace CShargs
{
    class Program
    {
        static void Main(string[] args)
        {

            var options = new Options();
            try
            {
                options.Parse(args);
            }
            catch
            {
                ShowHelp(options);
                return;
            }

            if (options.Show)
            {
                Console.WriteLine("policy: default ...");
            }
            else if (options.Hardware)
            {
                Console.WriteLine("available: 2 nodes (0-1)");
            }
            else if (options.PlainArgs == null || options.PlainArgs.Count == 0)
            {
                ShowHelp(options);
            }
            else
            {
                Console.WriteLine("will run " + options.PlainArgs[0]);
                if (options.Physcpubind != null)
                    Console.WriteLine("CPU node bind: " + options.Physcpubind);
                // ...
            }
        }

        private static void ShowHelp(Options options)
        {
            Console.WriteLine("usage: numactl [--interleave= | -i <nodes>] ..."); // Print the part of usage which can't be generated
            Console.WriteLine(options.GenerateHelp()); // Print the part of usage which can be generated
        }
    }

    class Options : Parser
    {
        [ValueOption("interleave", alias: "i", help: "Interleave memory allocation across given nodes.", required: false)]
        public NodeNumbers Interleave { get; set; }

        [ValueOption("preferred", alias: "p", help: "Prefer memory allocations from given node.", required: false)]
        public NodeNumber Preferred { get; set; }

        [ValueOption("membind", alias: "m", help: "Allocate memory from given nodes only.", required: false)]
        public NodeNumbers Membind { get; set; }

        [ValueOption("physcpubind", alias: "C", help: "Run on given CPUs only.", required: false)]
        public NodeNumbers Physcpubind { get; set; }

        [FlagOption("show", alias: "S", help: "Show current NUMA policy.")]
        public bool Hardware { get; set; }

        [FlagOption("hardware", alias: "H", help: "Print hardware configuration.")]
        public bool Show { get; set; }

        public Parser()
        {
            Interleave = new NodeNumbers(); // There should be some function which finds out default memory interleaving
            Membind = new NodeNumbers(); // Same
            Physcpubind = new NodeNumbers(); // Same
        }
    }

    class NodeNumbers
    {
        public int[] nodes;

        public static NodeNumbers Parse(string str)
        {
            var nodeNumbers = new NodeNumbers();
            nodeNumbers.nodes = new int[] { 1, 2 }; // Parse from str
            return nodeNumbers;
        }

        public override string ToString()
        {
            return string.Join(",", nodes);
        }
    }

    class NodeNumber
    {
        public int node;
        public bool isSet = false;

        public static NodeNumber Parse(string str)
        {
            var nodeNumber = new NodeNumber();
            nodeNumber.isSet = true;
            nodeNumber.node = 1 // Parse from str
            return nodeNumber;   
        }

    }
}

```

## Celkový dojem ##

Z celého API mám pocit, že při návrhu byla na prvním místě funkčnost. Obsahuje spoustu featur, které v zadání nebyly zmíněny a dále jde nakonfigurovat tak, aby zvládlo i všemožné edge případy. Na druhou stranu, v některých jednoduchých případech se zase nepoužívá zcela nejsnadněji.

Z mnoha důvodů mi nepřijde ideální, že datová struktura, na kterou umisťuji parametry, musí dědit z třídy `Parser`. Preferoval bych mít samostatný parser, pomocí něj vytvořit výsledný objekt s daty a dál už používat jen ten.

## Poznámky k numactl ##

V API bych uvítal možnost jednoduše zjistit, zda uživatel parametr zadal nebo ne. Například bych mohl mít volitelný parametr "-f", který dělá to, že přesměruje výstup z konzole do zadaného souboru. Potom mě zajímá jak hodnota parametru "-f", tak to, zda byl zadán nebo ne. Pokud je "-f" nullable typ, můžu si ověřit jestli není null, ale to není sémanticky zcela správně. U jiných typů to pak nejde rozlišit vůbec. To se projevilo v uvedeném příkladu tak, že jsem pro parametr `Preferred` musel vytvořit vlastní typ místo použití `int`, protože defaultní hodnota `0` je v tomto případě validní vstup.

Trochu neúplné mi přijde i parsování plain argumentů - všechny se uloží do listu stringů a jinak se neparsují. Přitom by určitě mohly být parsovány stejně, jako pojmenované parametry.

Velmi kladně hodnotím to, že alespoň pojmenované parametry jsou proměnné s jasnými typy, takže už za překladu mám zaručenou typovou bezpečnost. Nemusím potom psát něco jako `args.get("preferred")` a doufat, že jsem se trefil v názvu parametru a že je skutečně typu který potřebuji `int`. Dále oceňuji možnost parsovat libovolné typy, pro které si nadefinuji vlastní parsovací funkci.

## Poznámky k checklistu ##

Zajímavými featurami jsou například `OptionGroup` a `useWith`. Pomocí první je možné nastavit, že z dané množiny parametrů může být zvolen pouze jeden. Druhá zase říká, že některé parametry může uživatel použít jen tehdy, pokud zároveň použije i určité další. Pomocí obou featur se dají popsat téměř libovolně komplexní vztahy a knihovna při parsování ověří jejich platnost. Nicméně toto u typické aplikace spíš nebudu potřebovat a pokud už ano, zvládnu si to vyřešit v programu (což mi navíc umožní upozornit uživatele na konkrétní problém, např. "parametr -i je možné použít jen současně s parametrem -f"). Dále třeba `OptionGroup` jde ve většině případech nahradit jedním parametrem s `enum` typem apod. Určitě tím nechci říct, že bylo chybou sem tyto featury dávat, pouze si myslím, že by je bylo lepší přidat až někdy později a teď se spíš soustředit na některé častěji používané.

Vzhledem k velkému rozsahu funkcionalit API bych uvítal možnost automaticky zkontrolovat, zda nejsou v rozporu. Například zda nejsou dva povinné parametry ve stejné `OptionGroup`.

Třída `Parser` obsahuje několik parametrů, které ovlivňují její chování, ale nejsou popsané v dokumentaci (`PlainArgsRequired`, `Skip`). Naopak metoda `OnFinish` je zmíněna v dokumentaci, ale není v kódu. U funkce `Parse` není jasné, co se stane v případě, že parsování selže. Zřejmě dojde k vyhození výjimky, ale v dokumentaci není napsáno ke které (výjimky nejsou ani implementované v kódu). K dokumentaci další výhrady nemám. Kód je dobře formátován a splňuje konvence platformy.

I když se z textu výše může zdát, že API spíše kritizuji, ve skutečnosti tomu tak není. Spíš mi jen přišlo zbytečné psát o té většině věcí, které v API fungují jak mají. Místo toho mi přišlo přínosnější zaměřit se na to, co by z mého pohledu chtělo zlepšit. V zásadě platí, že to, co jsem nezmínil, považuji za správné.