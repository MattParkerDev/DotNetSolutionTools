﻿using System.ComponentModel;
using DotNetSolutionTools.Core;
using DotNetSolutionTools.Core.Common;
using Spectre.Console.Cli;

namespace DotNetSolutionTools.CLI.Commands;

public class CompareCommand : Command<CompareCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<SolutionFolderPath>")]
        public required string SolutionFolderPath { get; set; }

        [CommandArgument(1, "<SolutionFilePath>")]
        public required string SolutionFilePath { get; set; }

        [CommandOption("-l|--logprojectfiles")]
        [Description("true to enable log output of all project files found in folder. Default is false.")]
        [DefaultValue(false)]
        public bool LogAllProjectFileNames { get; set; } = false;
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var folderDirectory = settings.SolutionFolderPath; // Include the trailing slash
        var pathToSolutionFile = settings.SolutionFilePath;
        Console.WriteLine($"Retrieving C# Projects from {folderDirectory}");

        var csprojList = CsprojHelper.RetrieveAllCSharpProjectFullPathsFromFolder(folderDirectory);

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
        var solutionFile = SlnHelper.ParseSolutionFileFromPath(pathToSolutionFile);
        if (solutionFile == null)
        {
            Console.WriteLine("Failed to parse solution file. The file was either not found or malformed.");
            return 1;
        }

        // Get the list of projects
        var projectsMissingFromSolution = SolutionProjectParity.FindProjectsMissingFromSolution(
            csprojList,
            solutionFile
        );

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
