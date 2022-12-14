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

## Celkov?? dojem ##

Z cel??ho API m??m pocit, ??e p??i n??vrhu byla na prvn??m m??st?? funk??nost. Obsahuje spoustu featur, kter?? v zad??n?? nebyly zm??n??ny a d??le jde nakonfigurovat tak, aby zvl??dlo i v??emo??n?? edge p????pady. Na druhou stranu, v n??kter??ch jednoduch??ch p????padech se zase nepou????v?? zcela nejsnadn??ji.

Z mnoha d??vod?? mi nep??ijde ide??ln??, ??e datov?? struktura, na kterou umis??uji parametry, mus?? d??dit z t????dy `Parser`. Preferoval bych m??t samostatn?? parser, pomoc?? n??j vytvo??it v??sledn?? objekt s daty a d??l u?? pou????vat jen ten.

## Pozn??mky k numactl ##

V API bych uv??tal mo??nost jednodu??e zjistit, zda u??ivatel parametr zadal nebo ne. Nap????klad bych mohl m??t voliteln?? parametr "-f", kter?? d??l?? to, ??e p??esm??ruje v??stup z konzole do zadan??ho souboru. Potom m?? zaj??m?? jak hodnota parametru "-f", tak to, zda byl zad??n nebo ne. Pokud je "-f" nullable typ, m????u si ov????it jestli nen?? null, ale to nen?? s??manticky zcela spr??vn??. U jin??ch typ?? to pak nejde rozli??it v??bec. To se projevilo v uveden??m p????kladu tak, ??e jsem pro parametr `Preferred` musel vytvo??it vlastn?? typ m??sto pou??it?? `int`, proto??e defaultn?? hodnota `0` je v tomto p????pad?? validn?? vstup.

Trochu ne??pln?? mi p??ijde i parsov??n?? plain argument?? - v??echny se ulo???? do listu string?? a jinak se neparsuj??. P??itom by ur??it?? mohly b??t parsov??ny stejn??, jako pojmenovan?? parametry.

Velmi kladn?? hodnot??m to, ??e alespo?? pojmenovan?? parametry jsou prom??nn?? s jasn??mi typy, tak??e u?? za p??ekladu m??m zaru??enou typovou bezpe??nost. Nemus??m potom ps??t n??co jako `args.get("preferred")` a doufat, ??e jsem se trefil v n??zvu parametru a ??e je skute??n?? typu kter?? pot??ebuji `int`. D??le oce??uji mo??nost parsovat libovoln?? typy, pro kter?? si nadefinuji vlastn?? parsovac?? funkci.

## Pozn??mky k checklistu ##

Zaj??mav??mi featurami jsou nap????klad `OptionGroup` a `useWith`. Pomoc?? prvn?? je mo??n?? nastavit, ??e z dan?? mno??iny parametr?? m????e b??t zvolen pouze jeden. Druh?? zase ????k??, ??e n??kter?? parametry m????e u??ivatel pou????t jen tehdy, pokud z??rove?? pou??ije i ur??it?? dal????. Pomoc?? obou featur se daj?? popsat t??m???? libovoln?? komplexn?? vztahy a knihovna p??i parsov??n?? ov?????? jejich platnost. Nicm??n?? toto u typick?? aplikace sp???? nebudu pot??ebovat a pokud u?? ano, zvl??dnu si to vy??e??it v programu (co?? mi nav??c umo??n?? upozornit u??ivatele na konkr??tn?? probl??m, nap??. "parametr -i je mo??n?? pou????t jen sou??asn?? s parametrem -f"). D??le t??eba `OptionGroup` jde ve v??t??in?? p????padech nahradit jedn??m parametrem s `enum` typem apod. Ur??it?? t??m nechci ????ct, ??e bylo chybou sem tyto featury d??vat, pouze si mysl??m, ??e by je bylo lep???? p??idat a?? n??kdy pozd??ji a te?? se sp???? soust??edit na n??kter?? ??ast??ji pou????van??.

Vzhledem k velk??mu rozsahu funkcionalit API bych uv??tal mo??nost automaticky zkontrolovat, zda nejsou v rozporu. Nap????klad zda nejsou dva povinn?? parametry ve stejn?? `OptionGroup`.

T????da `Parser` obsahuje n??kolik parametr??, kter?? ovliv??uj?? jej?? chov??n??, ale nejsou popsan?? v dokumentaci (`PlainArgsRequired`, `Skip`). Naopak metoda `OnFinish` je zm??n??na v dokumentaci, ale nen?? v k??du. U funkce `Parse` nen?? jasn??, co se stane v p????pad??, ??e parsov??n?? sel??e. Z??ejm?? dojde k vyhozen?? v??jimky, ale v dokumentaci nen?? naps??no ke kter?? (v??jimky nejsou ani implementovan?? v k??du). K dokumentaci dal???? v??hrady nem??m. K??d je dob??e form??tov??n a spl??uje konvence platformy.

I kdy?? se z textu v????e m????e zd??t, ??e API sp????e kritizuji, ve skute??nosti tomu tak nen??. Sp???? mi jen p??i??lo zbyte??n?? ps??t o t?? v??t??in?? v??c??, kter?? v API funguj?? jak maj??. M??sto toho mi p??i??lo p????nosn??j???? zam????it se na to, co by z m??ho pohledu cht??lo zlep??it. V z??sad?? plat??, ??e to, co jsem nezm??nil, pova??uji za spr??vn??.