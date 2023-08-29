using System.ComponentModel;
using Microsoft.Build.Construction;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SlnAndCsprojParityChecker.Commands;

public class CompareCommand : Command<CompareCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<SolutionFolderPath>")]
        public required string SolutionFolderPath { get; set; }

        [CommandArgument(1, "<SolutionFilePath>")]
        public required string SolutionFilePath{ get; set; }

        [CommandOption("-l|--logprojectfiles")]
        [Description("true to enable logging of all project files. Default is false.")]
        [DefaultValue(false)]
        public bool LogAllProjectFileNames { get; set; } = false;
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var folderDirectory = settings.SolutionFolderPath; // Include the trailing slash
        var pathToSolutionFile = settings.SolutionFilePath;
        Console.WriteLine($"Retrieving C# Projects from {folderDirectory}");

        var csprojList = Directory.GetFiles(folderDirectory, "*.csproj", SearchOption.AllDirectories);
        csprojList = csprojList.Select(x => x.Replace(folderDirectory, "")).ToArray();

        if (settings.LogAllProjectFileNames)
        {
            foreach (var project in csprojList)
            {
                Console.WriteLine(project);
            }
        }

        Console.WriteLine($"Retrieved {csprojList.Length} C# Projects");
        Console.WriteLine("==================================================");

        Console.WriteLine($"Parsing Solution File: {pathToSolutionFile}");
        // Load the solution file
        var solutionFile = SolutionFile.Parse(pathToSolutionFile);
        if (solutionFile == null)
        {
            Console.WriteLine("Failed to parse solution file. The file was either not found or malformed.");
            return 1;
        }

        // Get the list of projects
        var projects = solutionFile.ProjectsInOrder;
        var projectsMissingFromSolution = new List<string>();

        foreach (var project in csprojList)
        {
            var projectInSolution = projects.FirstOrDefault(x => x.RelativePath == project);

            if (projectInSolution == null)
            {
                projectsMissingFromSolution.Add(project);
            }
        }

        Console.WriteLine("==================================================");
        Console.WriteLine($"Missing {projectsMissingFromSolution.Count} C# Projects from Solution File");

        foreach (var project in projectsMissingFromSolution)
        {
            Console.WriteLine(project);
        }
        Console.WriteLine("==================================================");
        Console.WriteLine("Done!");
        return 0;
    }
}