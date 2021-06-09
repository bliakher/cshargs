## Základní dojem

Na první pohled knihovna působí solidním a kompletním dojmem. Nabízí všechnu základní funkcionalitu vyžadovanou přechozími zadáními, plus pár zajímavých validačních mechanismů navíc (skupiny optionů, závislosti mezi optiony). Seznámení s rozhraním knihovny přímo z kódu je ztíženo neoddělováním veřejných a implementačních částí. Vynahrazuje to však generovaná dokumentace pokrývající právě veřejné API a hlavně pak dobře napsané *readme*, které obsahuje dostatek informací pro efektivní práci s knihovnou. Součástí je také několik ukázkových programů demonstrujících konfiguraci knihovny. Funkcionalita knihovny je solidně pokryta dodanými testy (zejména v projektu `Tests-s11`).

Implementace "rozšíření" s parsováním datetime argumentů byla triviální, protože standardní .NET typ `System.Datetime` se umí ze zadaného ISO formátu naparsovat přímo. Stačilo tedy napsat deklaraci podle příkladů v *readme* a použít `Datetime` jako typ optionu.

## Návrh API

Tato sekce se věnuje zhodnocení veřejného rozhraní knihovny z pohledu uživatele-programátora. Samotné implementaci se věnuje následující sekce.

### Konfigurace a použití parseru

Konfigurace víceméně všech aspektů knihovny se provádí pomocí atributů. Uživatel si nadefinuje třídu odvozenou od knihovního typu `Parser` a jednotlivé aspekty konfigurace pak realizuje přidáváním atributů: globální vlastnosti parseru se nastavují pomocí atributů samotné třídy, jednotlivé flagy a optiony se nastavují přes atributy vlastností (properties) třídy, do kterých budou ukládána naparsovaná data, anebo atributy metod, které mají umožňovat parsovat libovolné objekty z libovolného množství argumentů.

Ačkoli nejsem příliš velký fanoušek atributů v C#, oceňuji, že na rozdíl od některých jiných návrhů, které míchají atributy s procedurální konfigurací, pomocí nich knihovna konzistentně realizuje veškerou konfiguraci (s výjimkou vlastnosti `PlainArgsRequired`). Výhodou atributů je, že se nachází nejblíže konfigurovaným objektům, jak to jen jde. Mezi nevýhody patří jistá nepřehlednost u atributů s dlouhými parametry nebo při řetězení mnoha atributů za sebou (které tu může nastat u `Parser` třídy). Praktičtější nevýhodou je také nemožnost používat jiné hodnoty parametrů, než compile-time konstanty. Důsledkem například je, že help text pro optiony nejde v kódu doplňovat dynamicky nebo načítat z externích zdrojů.

Nepřímým důsledkem použití atributů je asi též trochu zvláštní způsob, jakým je podporováno parsování nestandardních typů, které vyžaduje existenci statické metody `T Parse(string)` na daném typu. Alternativou k tomu je implementace metody `Parser` třídy s atributem  `CustomOption`, která představuje tak trochu "bianco šek" v rozhraní parseru, protože se od ní očekává akorát tvar `void(string)` a má přístup k `protected` vlastnostem zpřístupňujícím vnitřní stav parsujícího procesu (`Arguments`, `Skip`). Uživatel si tak může v rámci metody doimplementovat prakticky cokoli, třeba celý vlastní parser. :slightly_smiling_face:

To mě přivádí k podle mého nejméně elegantní části návrhu API - skutečnosti, že konfigurovaná třída, do které jsou ukládány naparsované hodnoty, zároveň dědí od knihovní třídy `Parser`. V obecné rovině jde o už přílišné přetížení zodpovědností - jedna třída (a jedna její instance) slouží ke konfiguraci, jako nositel dat a zároveň jako samotný procesor dat, kvůli kterému jsou součástí třídy zděděné vlastnosti a metody mající smysl pouze před nebo během parsování. Prakticky to také znamená, že uživatel od začátku drží v ruce instanci své argumentové třídy, se kterou lze normálně nakládat (obsahuje defaultní hodnoty), ačkoli není z hlediska logiky programu validní, dokud na ní neproběhne úspěšný `Parse`. Tato skutečnost není nijak staticky ani běhově kontrolována a je pouze na uživatelově zodpovědnosti. Jistě nejde o velký požadavek, každopádně znakem dobrého návrhu je, že nejde použít špatně, nebo se aspoň uživatel o špatném použití dozví co nejdřív. 

Chtěl bych se ještě zmínit o funkcionalitě, kterou knihovna přidává navíc oproti základním požadavkům. Za velmi pěknou považuji možnost kontrolovat závislosti mezi nepovinnými optiony pomocí parametru `useWith` nebo zda byl nastaven jen jeden ze skupiny optionů pomocí atributu `OptionGroup`. To může skutečně usnadnit práci uživateli, který by jinak psal nad výsledkem parseru síto testů pro různé kombinace nastavení.

Naopak nepříliš užitečný je koncept `VerbOption`. Ten autoři podle příkladů zamýšleli pro implementaci podpříkazů ve stylu komplexních CLI aplikací typu `git` (kde např. `git commit` nebo `git push` představují separátní akce s vlastními disjunktními sadami argumentů). Tak, jak je ale `VerbOption` navržena, slouží pouze k zanořování konfigurací parseru, jehož výstupem je ale stejně jen zapsání dat. Kdyby autoři dokončili ve svém příkladu třídu `GitArguments`, tak by obsahovala spoustu položek potenciálně držících výsledky dílčích parserů a uživatel by testoval, která z nich byla nastavená. Neberu to jako nedostatek, protože taková funkcionalita byla podle mě mimo scope zadání. (Příkladem, jak podporovat stavbu složitějších konzolových rozhraní, je v .NETu knihovna [System.CommandLine](https://github.com/dotnet/command-line-api).)

Ačkoli předchozí odstavce možná zněly kriticky, chci zdůraznit, že považuji návrh rozhraní za zdařilý a pro běžné použití přiměřeně přívětivý.

### Help, version, zpracování chyb

Knihovna nemá speciální podporu pro příkazy *help* a *version*, které by měly prioritu nad ostatními argumenty a přerušovaly parsování a la klasické unixové příkazy. Místo toho se očekává použití standardních boolovských flagů nebo univerzálního nástroje v podobě `CustomOption` metody (ze které lze parsování ukončit). To považuji za legitimní rozhodnutí - je to otázka obecnosti a jednotnosti rozhraní vs usnadnění běžných způsobů použití. Každopádně knihovna poskytuje užitečné generování textu helpu na základě konfigurace optionů. Autoři si s tímto textem vyhráli, zahrnuli do něj informace nad rámec názvu a popisu optionu a udělali mu slušné formátování. (Přiznávám, že jde o alibismus z mé strany, protože my jsme se na to kvůli času vybodli.)

Veškeré zpracování chyb knihovna provádí pomocí vlastních výjimek. Používá poměrně extenzivní rodokmen typů výjimek. Je ovšem diskutabilní, jestli je pro uživatele detailní rozlišení typů parsovacích výjimek přínosem. Pro uživatele má totiž prakticky smysl jen zachytávat společného předka výjimek `ParsingError` a případně vypisovat zprávu, kterou obsahuje.

## Implementace

Tato sekce se věnuje kvalitě kódu a aspektům architektury řešení, které nebyly zmíněny v rámci diskuze veřejného rozhraní.

### Struktura

Z hlediska struktury neobsahuje knihovna žádné velké excesy, zmíním ale několik problémů.

První, globální "nitpick" je ne úplně šťastné rozmístění projektů a souborů v rámci samotného repozitáře řešení. Zdrojáky knihovny jsou v jedné, kořenové úrovni adresářové struktury. Další projekty (testy, program pro tento úkol, správně i existující ukázkové programy) pak musí být ve vnořených adresářích, což vzhledem k principu, na kterém funguje projektová konfigurace v moderním .NETu, znamená, že musí být explicitně vylučovány v `cshargs.csproj`, který by je jinak považoval za svou součást.

Omezíme-li se na kód knihovny, důležitějším problémem je v úvodu zmíněna ne-separace kódu veřejného rozhraní a interní implementace knihovny. Ta se projevuje jak na úrovni tříd, potažmo zdrojáků (strčte interní věci do separátní složky/namespacu), tak v rámci konkrétních tříd. V nich se totiž často nesystematicky míchají metody a vlastnosti s různými access modifikátory (public, protected, internal, private) i různými deklaracemi z hlediska polymorfismu (abstract, virtual, non-virtual). Obojí zbytečně ztěžuje orientaci v architektuře knihovny.

Tento problém souvisí s obecnějšími otázkami separace zodpovědností. Co do "business logiky" největší třídy `Parser` a `OptionMetadata` (resp. od ní dědící typy) totiž implementují řadu netriviálních zodpovědností, které by mohly být delegovány (v případě metod veřejného rozhraní) nebo dekomponovány (v případě low-level detailů v principiálně orchestračních metodách) na jiné třídy. Pomohlo by to zpřehlednit veřejné rozhraní, ve kterém stačí ponechat validaci vstupů a předání vnitřní infrastruktuře. Dekompozice by dále pomohla snížit opakování technických kousků kódu typu parsování "key=value" párů. Abych ale jen nekritizoval, příkladem dobré separace zodpovědností je třeba systém pravidel implementovaný v `Rules.cs`. 

Na závěr bych jako detail k diskuzi o struktuře zmínil texty chybových hlášek, které jsou potenciálně vypisované uživateli. Za zvážení stojí nějaká forma centralizace těchto napříč kódem rozesetých textů (nebo jejich generátorů), protože ačkoli chyby vznikají v různých kontextech, obsah hlášek je ve výsledku součástí jedné separátní vrstvy (de facto prezentační), kterou je užitečné nenechat nekontrolovaně prosakovat po celém programu.

### Styl

Nejnápadnějším stylistickým problémem je pojmenování metod a vlastností tříd. Nevadí ani tolik použití "camelCase" místo v C# kanonického "PascalCase", ale hlavně to, že se styly míchají, a to i v rámci jedné třídy. Kromě toho kód obsahuje množství drobných formátovacích nedostatků v indentaci nebo nekonzistentním odřádkování okolo deklarací a okolo řídících konstruktů (if, while, atd.). Dohromady ale nejde o nic, co by nespravila chvilka s linterem. (Pro týmy se smíšenými zvyklostmi doporučuju hned na začátku zapnout a dle dohody nakonfigurovat [StyleCop](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) nebo podobný analyzér.)

Názvy identifikátorů samotné, kromě uvedené nekonzistence stylu, jsou voleny vhodně a pomáhají srozumitelnosti kódu. Celkově nemám další připomínky ke stylu, které by šly nad rámec osobních preferencí. 

### Dokumentace

Kód tvořící veřejné rozhraní knihovny je dobře (a zdá se kompletně) popsán dokumentačními komentáři. Dodána je konfigurace pro doxygen, která z nich generuje klasickou html dokumentaci. Interní kód je víceméně nekomentovaný, což typicky nevadí, protože je kód srozumitelný sám o sobě. V několika případech rozsáhlejších metod s vrstevnatou logikou by se komentáře hodily - lze ale namítnout, že ještě lepší by byla dekompozice do menších vhodně pojmenovaných metod. Každopádně interní dokumentace je pouze "nice-to-have" věc, kterou u jednorázového projektu vzhledem k časovým nárokům nepovažuji za nutnou.
  
  

