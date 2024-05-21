using System.Collections.Concurrent;
using DotNetSolutionTools.Core.Common;
using Microsoft.Build.Construction;
using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using NuGet.Versioning;

namespace DotNetSolutionTools.Core;

public class PackageVersionConsistency
{
    public static List<string> FindInconsistentNugetPackageVersions(string solutionFilePath)
    {
        var solutionFile = SolutionFile.Parse(solutionFilePath);
        var csprojList = SlnHelper.GetCSharpProjectObjectsFromSolutionFile(solutionFile);
        var packageList = new List<(string, NuGetVersion)>();
        //var packageListConcurrent = new ConcurrentBag<(string, NuGetVersion)>();
        var outOfSyncPackages = new List<(string packageName, NuGetVersion version)>();
        // Parallel.ForEach(
        //     csprojList,
        //     project =>
        //     {
        //         var projectPackages = GetPackageVersions(project);
        //         lock (packageList)
        //         {
        //             packageList.AddRange(projectPackages);
        //         }
        //     }
        // );
        foreach (var project in csprojList)
        {
            var projectPackages = GetPackageVersions(project);
            packageList.AddRange(projectPackages);
        }

        var groupedByPackage = packageList.GroupBy(s => s.Item1).ToList();
        foreach (var package in groupedByPackage)
        {
            var versions = package.Select(s => s.Item2).Distinct().ToList();
            if (versions.Count > 1)
            {
                outOfSyncPackages.Add((package.Key, versions.First()));
            }
        }

        Console.WriteLine("Logging out of sync packages");
        outOfSyncPackages.ForEach(s => Console.WriteLine(s.packageName));

        return outOfSyncPackages.Select(s => s.packageName).ToList();
    }

    private static List<(string, NuGetVersion)> GetPackageVersions(ProjectRootElement project)
    {
        try
        {
            var packages = project
                .Items.Where(x => x.ItemType == "PackageReference" && x.Metadata.Any(s => s.Name == "Version"))
                .ToList();

            var packageNameAndVersion = packages
                .Select(x => (x.Include, NuGetVersion.Parse(x.Metadata.First(s => s.Name == "Version").Value)))
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
