using DotNetSolutionTools.Core;
using DotNetSolutionTools.Core.Common;
using Spectre.Console.Cli;

namespace DotNetSolutionTools.CLI.Commands;

public class FormatCsprojCommand : Command<FormatCsprojCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("--project <CsprojFilePath>")]
        public string? CsprojFilePath { get; set; }

        [CommandOption("--folder <SolutionFolderPath>")]
        public string? SolutionFolderPath { get; set; }

        [CommandOption("--sln <SolutionFilePath>")]
        public string? SolutionFilePath { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        if (!string.IsNullOrWhiteSpace(settings.CsprojFilePath))
        {
            var pathToCsprojFile = settings.CsprojFilePath;
            Console.WriteLine($"Retrieving C# Project from {pathToCsprojFile}");
            FormatCsproj.FormatCsprojFile(pathToCsprojFile);
            Console.WriteLine("Done!");
            return 0;
        }
        else if (!string.IsNullOrWhiteSpace(settings.SolutionFilePath))
        {
            var test = SlnHelper.ParseSolutionFileFromPath(settings.SolutionFilePath);
            if (test == null)
            {
                Console.WriteLine("Failed to parse solution file. The file was either not found or malformed.");
                return 1;
            }
            var cSharpProjects = SlnHelper.GetCSharpProjectObjectsFromSolutionFile(test);
            Console.WriteLine($"Found {cSharpProjects.Count} C# Projects");
            Console.WriteLine("==================================================");
            foreach (var project in cSharpProjects)
            {
                FormatCsproj.FormatCsprojFile(project.FullPath);
            }
            Console.WriteLine("Done!");
            return 0;
        }
        else if (!string.IsNullOrWhiteSpace(settings.SolutionFolderPath))
        {
            var folderDirectory = settings.SolutionFolderPath; // Include the trailing slash
            Console.WriteLine($"Retrieving C# Projects from {folderDirectory}");

            var csprojList = CsprojHelper.RetrieveAllCSharpProjectFullPathsFromFolder(folderDirectory);

            Console.WriteLine($"Retrieved {csprojList.Length} C# Projects");
            Console.WriteLine("==================================================");

            foreach (var project in csprojList)
            {
                FormatCsproj.FormatCsprojFile(project);
            }
            Console.WriteLine("Done!");
            return 0;
        }
        else
        {
            Console.WriteLine("No arguments were provided. Please provide a csproj, folder, or solution file.");
            return 1;
        }
    }
}
