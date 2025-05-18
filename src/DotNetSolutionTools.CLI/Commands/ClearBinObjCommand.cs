using DotNetSolutionTools.Core;
using Spectre.Console.Cli;

namespace DotNetSolutionTools.CLI.Commands;

public class ClearBinObjCommand : Command<ClearBinObjCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(1, "<FolderPath>")]
        public required string FolderPath { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        // validate a real folder path was passed in
        if (!Directory.Exists(settings.FolderPath))
        {
            Console.WriteLine("Invalid folder path");
            return 1;
        }
        Console.WriteLine("Deleting bin, obj, and node_modules folders");
        CleanFolder.DeleteBinObjAndNodeModulesFoldersInFolder(settings.FolderPath);

        Console.WriteLine("==================================================");
        Console.WriteLine("Done!");
        return 0;
    }
}
