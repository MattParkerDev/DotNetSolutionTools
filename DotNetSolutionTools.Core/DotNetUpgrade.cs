using DotNetSolutionTools.Core.Infrastructure;
using Microsoft.Build.Construction;
using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;

namespace DotNetSolutionTools.Core;

public static class DotNetUpgrade
{
    public static async Task UpdateProjectsInSolutionToNet80(string solutionFilePath)
    {
        var solutionFile = SolutionFile.Parse(solutionFilePath);
        var csprojList = SolutionProjectParity.GetCSharpProjectObjectsFromSolutionFile(
            solutionFile
        );
        await UpdateProjectsToNet80(csprojList);
    }

    private static async Task UpdateProjectsToNet80(List<ProjectRootElement> projects)
    {
        foreach (var project in projects)
        {
            var targetFramework = project
                .PropertyGroups
                .SelectMany(x => x.Properties)
                .FirstOrDefault(x => x.Name == "TargetFramework");
            if (targetFramework?.Value is "net7.0")
            {
                targetFramework.Value = "net8.0";
                project.Save();
                FormatCsproj.FormatCsprojFile(project.FullPath);
            }

            await UpdatePackagesToLatest(project);
        }
    }

    public static async Task UpdatePackagesToLatest(ProjectRootElement project)
    {
        var evalProject = Project.FromProjectRootElement(
            project,
            new ProjectOptions() { LoadSettings = ProjectLoadSettings.IgnoreMissingImports }
        );
        var packages = evalProject.Items.Where(x => x.ItemType == "PackageReference");
        var packageNameAndVersion = packages
            .Where(s => s.EvaluatedInclude.StartsWith("Microsoft."))
            .Select(
                x =>
                    new
                    {
                        Package = x,
                        Name = x.EvaluatedInclude,
                        Version = Version.Parse(
                            x.Metadata.First(s => s.Name == "Version").UnevaluatedValue
                        )
                    }
            )
            .ToList();

        var shouldFormat = false;
        foreach (var package in packageNameAndVersion)
        {
            var latestNugetVersion = await NugetLookup.FetchPackageMetadataAsync(package.Name);
            // it will compare 6.8.0.0 to 6.8.0, and says left is newer, we dont want to say its newer, so use the normalized string to make a new version
            var normalizedVersion = Version.Parse(latestNugetVersion.ToString());
            if (normalizedVersion > package.Version)
            {
                shouldFormat = true;
                package.Package.SetMetadataValue("Version", latestNugetVersion.ToString());
                project.Save();
            }
        }
        if (shouldFormat)
            FormatCsproj.FormatCsprojFile(project.FullPath);
    }
}
