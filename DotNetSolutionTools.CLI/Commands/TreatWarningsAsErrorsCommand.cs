﻿using System.ComponentModel;
using DotNetSolutionTools.Core;
using DotNetSolutionTools.Core.Common;
using Spectre.Console.Cli;

namespace DotNetSolutionTools.CLI.Commands;

public class TreatWarningsAsErrorsCommand : Command<TreatWarningsAsErrorsCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(1, "<SolutionFilePath>")]
        public required string SolutionFilePath { get; set; }

        [CommandOption("-m|--add-missing")]
        [DefaultValue(false)]
        public bool AddMissing { get; set; } = false;
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var pathToSolutionFile = settings.SolutionFilePath;
        Console.WriteLine($"Retrieving Solution from {pathToSolutionFile}");

        var solutionFile = SlnHelper.ParseSolutionFileFromPath(pathToSolutionFile);
        if (solutionFile == null)
        {
            Console.WriteLine("Failed to parse solution file. The file was either not found or malformed.");
            return 1;
        }
        var cSharpProjects = SlnHelper.GetCSharpProjectObjectsFromSolutionFile(solutionFile);
        Console.WriteLine($"Found {cSharpProjects.Count} C# Projects");
        Console.WriteLine("==================================================");

        // Get the list of projects
        var projectsMissingTreatWarningsAsErrors = WarningsAsErrors.FindCSharpProjectsMissingTreatWarningsAsErrors(
            cSharpProjects
        );

        Console.WriteLine(
            $"{projectsMissingTreatWarningsAsErrors.Count} C# Projects have missing Treat Warnings As Errors"
        );

        if (settings.AddMissing)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("Adding missing Warnings As Errors");
            WarningsAsErrors.AddMissingTreatWarningsAsErrors(projectsMissingTreatWarningsAsErrors);
            var updatedProjects = SlnHelper.GetCSharpProjectObjectsFromSolutionFile(solutionFile);
            var projectsWithMissing = WarningsAsErrors.FindCSharpProjectsMissingTreatWarningsAsErrors(updatedProjects);
            Console.WriteLine(
                $"There are now {projectsWithMissing.Count} C# Projects missing Treat Warnings As Errors"
            );
        }
        Console.WriteLine("==================================================");
        Console.WriteLine("Done!");
        return 0;
    }
}
