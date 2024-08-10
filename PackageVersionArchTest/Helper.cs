using DotNetSolutionTools.Core;
using Microsoft.Build.Construction;
using Microsoft.Build.Locator;

namespace PackageVersionArchTest;

public static class PackageArchTest
{
    public static bool AllNugetVersionsAreConsistent(string solutionFileName)
    {
        var instance = MSBuildLocator
            .QueryVisualStudioInstances()
            .OrderByDescending(instance => instance.Version)
            .First();
        MSBuildLocator.RegisterInstance(instance);

        var currentDirectory = Directory.GetCurrentDirectory();
        while (!File.Exists(Path.Combine(currentDirectory, solutionFileName)))
        {
            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
            if (currentDirectory is null)
            {
                throw new FileNotFoundException("Solution file not found.");
            }
        }
        var solutionFilePath = Path.Combine(currentDirectory, solutionFileName);

        var result = PackageVersionConsistency.FindInconsistentNugetPackageVersions(solutionFilePath);

        if (result.Count > 0)
        {
            throw new Exception($"Inconsistent package versions found: {string.Join(", ", result)}");
        }

        return true;
    }
}
