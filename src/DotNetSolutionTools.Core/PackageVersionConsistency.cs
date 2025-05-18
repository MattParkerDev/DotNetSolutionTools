using DotNetSolutionTools.Core.Common;
using Microsoft.Build.Construction;
using NuGet.Versioning;

namespace DotNetSolutionTools.Core;

public class PackageVersionConsistency
{
    public static void ResolvePackageVersionsToLatestInstalled(string solutionFilePath)
    {
        var result = GetInconsistentNugetPackageVersions(solutionFilePath);
        var inconsistentPackages = result
            .GroupBy(s => s.packageName)
            .Select(s =>
                (s.Key, s.Select(x => (x.version, x.metadataElement)).OrderByDescending(x => x.version).ToList())
            )
            .ToList();

        foreach (var package in inconsistentPackages)
        {
            var latestInstalledVersion = package.Item2.First().version;
            foreach (var (version, metadataElement) in package.Item2.Skip(1))
            {
                metadataElement.Value = latestInstalledVersion.ToString();
                metadataElement.ContainingProject.Save();
                FormatCsproj.FormatCsprojFile(metadataElement.ContainingProject.FullPath);
            }
        }
    }

    public static List<string> FindInconsistentNugetPackageVersions(string solutionFilePath)
    {
        var result = GetInconsistentNugetPackageVersions(solutionFilePath);
        return result.Select(s => s.packageName).Distinct().ToList();
    }

    private static List<(
        string packageName,
        NuGetVersion version,
        ProjectMetadataElement metadataElement
    )> GetInconsistentNugetPackageVersions(string solutionFilePath)
    {
        var solutionFile = SolutionFile.Parse(solutionFilePath);
        var csprojList = SlnHelper.GetCSharpProjectObjectsFromSolutionFile(solutionFile);
        var packageList = new List<(string, NuGetVersion, ProjectMetadataElement)>();
        var outOfSyncPackages = new List<(string packageName, NuGetVersion version, ProjectMetadataElement element)>();

        foreach (var project in csprojList)
        {
            var projectPackages = GetPackageVersions(project);
            packageList.AddRange(projectPackages);
        }

        var groupedByPackage = packageList.GroupBy(s => s.Item1).ToList();
        foreach (var grouping in groupedByPackage)
        {
            var versions = grouping.Select(s => (s.Item2, s.Item3)).DistinctBy(s => s.Item1).ToList();
            if (versions.Count > 1)
            {
                //outOfSyncPackages.AddRange(versions.Select(s => (grouping.Key, s.Item1, s.Item2)));
                outOfSyncPackages.AddRange(grouping.Select(s => (s.Item1, s.Item2, s.Item3)));
            }
        }

        return outOfSyncPackages.ToList();
    }

    private static List<(string, NuGetVersion, ProjectMetadataElement)> GetPackageVersions(ProjectRootElement project)
    {
        try
        {
            var packages = project
                .Items.Where(x => x.ItemType == "PackageReference" && x.Metadata.Any(s => s.Name == "Version"))
                .ToList();

            var packageNameAndVersion = packages
                .Select(x =>
                    (
                        x.Include,
                        NuGetVersion.Parse(x.Metadata.First(s => s.Name == "Version").Value),
                        x.Metadata.First(s => s.Name == "Version")
                    )
                )
                .ToList();
            return packageNameAndVersion;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
