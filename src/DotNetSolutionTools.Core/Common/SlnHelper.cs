using Microsoft.Build.Construction;

namespace DotNetSolutionTools.Core.Common;

public static class SlnHelper
{
    public static SolutionFile? ParseSolutionFileFromPath(string solutionFilePath)
    {
        var solutionFile = SolutionFile.Parse(solutionFilePath);

        return solutionFile;
    }

    public static List<ProjectRootElement> GetCSharpProjectObjectsFromSolutionFile(SolutionFile solutionFile)
    {
        var projectList = solutionFile
            .ProjectsByGuid.Where(x => x.Value.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
            .Select(s => ProjectRootElement.Open(s.Value.AbsolutePath))
            .ToList();

        return projectList;
    }
}
