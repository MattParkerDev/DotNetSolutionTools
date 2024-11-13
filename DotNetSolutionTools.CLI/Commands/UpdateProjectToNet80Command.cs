using DotNetSolutionTools.Core;
using Spectre.Console.Cli;

namespace DotNetSolutionTools.CLI.Commands;

public class UpdateProjectToNet80Command : AsyncCommand<UpdateProjectToNet80Command.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(1, "<CsprojFilePath>")]
        public required string CsprojFilePath { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (!Directory.Exists(settings.CsprojFilePath))
        {
            Console.WriteLine("Invalid file path. Please pass in a valid file path to a .csproj file.");
            return 1;
        }
        Console.WriteLine("Upgrading project to .NET 9.0");
        await DotNetUpgrade.UpdateProjectAtPathToNet80(settings.CsprojFilePath);

        Console.WriteLine("==================================================");
        Console.WriteLine("Done!");
        return 0;
    }
}
