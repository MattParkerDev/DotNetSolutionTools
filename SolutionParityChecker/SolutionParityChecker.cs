using Microsoft.Build.Construction;

namespace SolutionParityChecker;

public static class SolutionParityChecker
{
    public static void CompareSolutionAndCSharpProjects(
        string solutionFolderPath,
        string solutionFilePath
    ) { }

    public static string[] RetrieveAllCSharpProjectNamesFromFolder(string solutionFolderPath)
    {
        var csprojList = Directory.GetFiles(
            solutionFolderPath,
            "*.csproj",
            SearchOption.AllDirectories
        );
        csprojList = csprojList.Select(x => x.Replace(solutionFolderPath, "")).ToArray();
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
            var projectInSolution = projects.FirstOrDefault(x => x.RelativePath == project);

            if (projectInSolution == null)
            {
                projectsMissingFromSolution.Add(project);
            }
        }

        return projectsMissingFromSolution;
    }
}
