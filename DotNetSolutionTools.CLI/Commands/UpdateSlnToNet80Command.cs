using DotNetSolutionTools.Core;
using Spectre.Console.Cli;

namespace DotNetSolutionTools.CLI.Commands;

public class UpdateSlnToNet80Command : AsyncCommand<UpdateSlnToNet80Command.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(1, "<SolutionFilePath>")]
        public required string SolutionFilePath { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (!File.Exists(settings.SolutionFilePath))
        {
            Console.WriteLine("Invalid file path. Please pass in a valid file path to a .csproj file.");
            return 1;
        }
        Console.WriteLine("Upgrading project to .NET 8.0");
        await DotNetUpgrade.UpdateProjectsInSolutionToNet80(settings.SolutionFilePath);

        Console.WriteLine("==================================================");
        Console.WriteLine("Done!");
        return 0;
    }
}
