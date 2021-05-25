
namespace CShargs.Examples
{
// parser representing a command with a subcommand ei. git push
internal class GitArguments : Parser
{
    [FlagOption("version", shortName: 'v', help: "Display version.")]
    private bool showVersion { get; set; }

    // git push --force -> to see if force flag present, access GitArguments.pushArguments.Force
    [VerbOption("push", help: "Update remote refs along with associated objects.")]
    private GitPushArguments pushArguments { get; set; }
        
}
    
// subcommand parser
internal class GitPushArguments : Parser
{
    [FlagOption("force", shortName: 'f', help: "Force push.")]
    private bool Force { get; set; }
}
}