using Microsoft.Build.Construction;

namespace DotNetSolutionTools.Core.Common;

public static class CsprojHelper
{
    public static string[] RetrieveAllCSharpProjectFullPathsFromFolder(string solutionFolderPath)
    {
        // if solutionFolderPath does not end with a slash, add one
        if (solutionFolderPath[^1] != Path.DirectorySeparatorChar)
        {
            solutionFolderPath += Path.DirectorySeparatorChar;
        }
        var csprojList = Directory.GetFiles(solutionFolderPath, "*.csproj", SearchOption.AllDirectories);
        return csprojList;
    }

    public static string[] RetrieveAllCSharpProjectFullPathsFromSolution(string solutionFilePath)
    {
        var solutionFile = SlnHelper.ParseSolutionFileFromPath(solutionFilePath);
        ArgumentNullException.ThrowIfNull(solutionFile);
        var result = SlnHelper.GetCSharpProjectObjectsFromSolutionFile(solutionFile);
        var csprojList = result.Select(x => x.FullPath).ToArray();
        return csprojList;
    }
}
