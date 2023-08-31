using System.ComponentModel;
using DotNetSolutionTools.Core;
using Spectre.Console.Cli;

namespace DotNetSolutionTools.CLI.Commands;

public class ImplicitUsingsCommand : Command<ImplicitUsingsCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(1, "<SolutionFilePath>")]
        public required string SolutionFilePath { get; set; }

        [CommandOption("-m|--add-missing")]
        [Description("Add Implicit Usings=true to projects missing them. Default is false.")]
        [DefaultValue(false)]
        public bool AddMissing { get; set; } = false;

        [CommandOption("-d|--enable-disabled")]
        [Description(
            "Sets Implicit Usings to true for any projects with it disabled. Default is false."
        )]
        [DefaultValue(false)]
        public bool EnableDisabled { get; set; } = false;

        [CommandOption("-a|--enable-all")]
        [Description("Enables Implicit Usings for all projects. Default is false.")]
        [DefaultValue(false)]
        public bool EnableAll { get; set; } = false;
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var pathToSolutionFile = settings.SolutionFilePath;
        Console.WriteLine($"Retrieving Solution from {pathToSolutionFile}");

        var solutionFile = SolutionParityChecker.ParseSolutionFileFromPath(pathToSolutionFile);
        if (solutionFile == null)
        {
            Console.WriteLine(
                "Failed to parse solution file. The file was either not found or malformed."
            );
            return 1;
        }
        var cSharpProjects = SolutionParityChecker.GetCSharpProjectObjectsFromSolutionFile(
            solutionFile
        );
        Console.WriteLine($"Found {cSharpProjects.Count} C# Projects");
        Console.WriteLine("==================================================");

        // Get the list of projects
        var projectsMissingImplicitUsings = ImplicitUsings.FindCSharpProjectsMissingImplicitUsings(
            cSharpProjects
        );

        Console.WriteLine(
            $"{projectsMissingImplicitUsings.Count} C# Projects have missing or disabled implicit usings"
        );

        foreach (var project in projectsMissingImplicitUsings)
        {
            Console.WriteLine(project.DirectoryPath);
        }

        if (settings.AddMissing)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("Adding missing implicit usings");
            ImplicitUsings.AddMissingImplicitUsings(projectsMissingImplicitUsings);
            var updatedProjects = SolutionParityChecker.GetCSharpProjectObjectsFromSolutionFile(
                solutionFile
            );
            var projectsWithMissing = ImplicitUsings.FindCSharpProjectsMissingImplicitUsings(
                updatedProjects
            );
            Console.WriteLine(
                $"There are now {projectsWithMissing.Count} C# Projects missing/disabled implicit usings"
            );
        }
        if (settings.EnableDisabled)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("Enabling disabled implicit usings");
            ImplicitUsings.EnableDisabledImplicitUsings(projectsMissingImplicitUsings);
        }
        if (settings.EnableAll)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("Enabling all implicit usings");
            ImplicitUsings.EnableAllImplicitUsings(projectsMissingImplicitUsings);
        }
        Console.WriteLine("==================================================");
        Console.WriteLine("Done!");
        return 0;
    }
}
