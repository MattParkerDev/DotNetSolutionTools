using DotNetSolutionTools.Core;
using Spectre.Console.Cli;

namespace DotNetSolutionTools.CLI.Commands;

public class UpdateProjectToNet80Command : Command<UpdateProjectToNet80Command.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(1, "<CsprojFilePath>")]
        public required string CsprojFilePath { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        // validate a real folder path was passed in
        if (!Directory.Exists(settings.CsprojFilePath))
        {
            Console.WriteLine("Invalid folder path");
            return 1;
        }
        Console.WriteLine("Deleting bin, obj, and node_modules folders");
        CleanFolder.DeleteBinObjAndNodeModulesFoldersInFolder(settings.CsprojFilePath);

        Console.WriteLine("==================================================");
        Console.WriteLine("Done!");
        return 0;
    }
}
