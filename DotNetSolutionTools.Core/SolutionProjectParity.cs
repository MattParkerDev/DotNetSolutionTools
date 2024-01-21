using Microsoft.Build.Construction;

namespace DotNetSolutionTools.Core;

public static class SolutionProjectParity
{
    public static List<string> CompareSolutionAndCSharpProjects(
        string solutionFolderPath,
        string solutionFilePath
    )
    {
        var csprojList = RetrieveAllCSharpProjectNamesFromFolder(solutionFolderPath);
        var solutionFile = ParseSolutionFileFromPath(solutionFilePath);
        ArgumentNullException.ThrowIfNull(solutionFile);
        var projectsMissingFromSolution = FindProjectsMissingFromSolution(csprojList, solutionFile);

        return projectsMissingFromSolution;
    }

    public static string[] RetrieveAllCSharpProjectNamesFromFolder(string solutionFolderPath)
    {
        var csprojList = RetrieveAllCSharpProjectFullPathsFromFolder(solutionFolderPath);

        return csprojList;
    }

    public static string[] RetrieveAllCSharpProjectFullPathsFromFolder(string solutionFolderPath)
    {
        // if solutionFolderPath does not end with a slash, add one
        if (solutionFolderPath[^1] != Path.DirectorySeparatorChar)
        {
            solutionFolderPath += Path.DirectorySeparatorChar;
        }
        var csprojList = Directory.GetFiles(
            solutionFolderPath,
            "*.csproj",
            SearchOption.AllDirectories
        );
        return csprojList;
    }
    
    public static string[] RetrieveAllCSharpProjectFullPathsFromSolution(SolutionFile solution)
    {
        var result = GetCSharpProjectObjectsFromSolutionFile(solution);
        var csprojList = result.Select(x => x.FullPath).ToArray();
        return csprojList;
    }

    public static SolutionFile? ParseSolutionFileFromPath(string solutionFilePath)
    {
        var solutionFile = SolutionFile.Parse(solutionFilePath);

        return solutionFile;
    }

    public static List<string> FindProjectsMissingFromSolution(
        string[] csprojList,
        SolutionFile solutionFile
    )
    {
        var projects = solutionFile.ProjectsInOrder;
        var projectsMissingFromSolution = new List<string>();

        foreach (var project in csprojList)
        {
            var projectInSolution = projects.FirstOrDefault(x => NormalizePath(x.AbsolutePath) == NormalizePath(project));

            if (projectInSolution == null)
            {
                projectsMissingFromSolution.Add(project);
            }
        }

        return projectsMissingFromSolution;
    }

    public static List<ProjectRootElement> GetCSharpProjectObjectsFromSolutionFile(
        SolutionFile solutionFile
    )
    {
        var projectList = solutionFile.ProjectsByGuid
            .Where(x => x.Value.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
            .Select(s => ProjectRootElement.Open(s.Value.AbsolutePath))
            .ToList();

        return projectList;
    }
    
    private static string NormalizePath(string path)
    {
        return Path.GetFullPath(new Uri(path).LocalPath)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}
