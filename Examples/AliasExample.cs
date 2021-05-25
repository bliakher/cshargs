namespace CShargs.Examples
{

[AliasOption("R", nameof(Recursive))]
[AliasOption("a", nameof(Recursive), nameof(Force))]
class AliasFlagArguments : Parser
{
    [FlagOption("recursive", shortName: 'r', help: "Remove directories and their contents recursively.")]
    public bool Recursive { get; set; }

    [FlagOption("force", shortName: 'f', help: "Ignore nonexistent files and arguments, never prompt.")]
    public bool Force { get; set; }
}

/*
 *   -R, -r, --recursive
 *          Remove directories and their contents recursively.
 *   -f, --force
 *          Ignore nonexistent files and arguments, never prompt.
 *   -a
 *          Alias of -rf
 */

}
