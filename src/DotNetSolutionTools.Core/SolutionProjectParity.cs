using DotNetSolutionTools.Core.Common;
using Microsoft.Build.Construction;

namespace DotNetSolutionTools.Core;

public static class SolutionProjectParity
{
    public static List<string> CompareSolutionAndCSharpProjects(string solutionFolderPath, string solutionFilePath)
    {
        var csprojList = CsprojHelper.RetrieveAllCSharpProjectFullPathsFromFolder(solutionFolderPath);
        var solutionFile = SlnHelper.ParseSolutionFileFromPath(solutionFilePath);
        ArgumentNullException.ThrowIfNull(solutionFile);
        var projectsMissingFromSolution = FindProjectsMissingFromSolution(csprojList, solutionFile);

        return projectsMissingFromSolution;
    }

    public static List<string> FindProjectsMissingFromSolution(string[] csprojList, SolutionFile solutionFile)
    {
        var projects = solutionFile.ProjectsInOrder;
        var projectsMissingFromSolution = new List<string>();

        foreach (var project in csprojList)
        {
            var projectInSolution = projects.FirstOrDefault(x =>
                NormalizePath(x.AbsolutePath) == NormalizePath(project)
            );

            if (projectInSolution == null)
            {
                projectsMissingFromSolution.Add(project);
            }
        }

        return projectsMissingFromSolution;
    }

    private static string NormalizePath(string path)
    {
        return Path.GetFullPath(new Uri(path).LocalPath)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}
