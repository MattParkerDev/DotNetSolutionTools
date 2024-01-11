using DotNetSolutionTools.Core.Infrastructure;
using Microsoft.Build.Construction;
using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using NuGet.Versioning;

namespace DotNetSolutionTools.Core;

public static class DotNetUpgrade
{
    public static async Task UpdateProjectsInSolutionToNet80(string solutionFilePath)
    {
        var solutionFile = SolutionFile.Parse(solutionFilePath);
        var csprojList = SolutionProjectParity.GetCSharpProjectObjectsFromSolutionFile(solutionFile);
        await UpdateProjectsToNet80(csprojList);
    }

    public static async Task UpdateProjectAtPathToNet80(string csprojFilePath)
    {
        var csproj = ProjectRootElement.Open(csprojFilePath);
        await UpdateProjectToNet80(csproj!);
    }

    private static async Task UpdateProjectsToNet80(List<ProjectRootElement> projects)
    {
        foreach (var project in projects)
        {
            await UpdateProjectToNet80(project);
        }
    }

    private static async Task UpdateProjectToNet80(ProjectRootElement project)
    {
        var targetFramework = project
            .PropertyGroups
            .SelectMany(x => x.Properties)
            .FirstOrDefault(x => x.Name == "TargetFramework");
        if (targetFramework?.Value is "net8.0" or "net7.0" or "net6.0" or "net5.0")
        {
            if (targetFramework.Value is not "net8.0")
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
        Project? evalProject = null;
        try
        {
            evalProject = Project.FromProjectRootElement(
                project,
                new ProjectOptions() { LoadSettings = ProjectLoadSettings.IgnoreMissingImports }
            );
            var packages = evalProject
                .Items
                .Where(
                    x =>
                        x.ItemType == "PackageReference"
                        && x.HasMetadata("Version")
                        && x.EvaluatedInclude.StartsWith("Microsoft.")
                )
                .ToList();

            var packageNameAndVersion = packages
                .Select(
                    x =>
                        new
                        {
                            Package = x,
                            Name = x.EvaluatedInclude,
                            NugetVersion = NuGetVersion.Parse(
                                x.Metadata.First(s => s.Name == "Version").UnevaluatedValue
                            )
                        }
                )
                .ToList();

            var shouldSave = false;
            foreach (var package in packageNameAndVersion)
            {
                var latestNugetVersion = await NugetLookup.FetchPackageMetadataAsync(
                    package.Name,
                    package.NugetVersion.IsPrerelease
                );

                if (latestNugetVersion > package.NugetVersion)
                {
                    shouldSave = true;
                    package.Package.SetMetadataValue("Version", latestNugetVersion.ToString());
                }
            }

            if (shouldSave)
            {
                project.Save();
                FormatCsproj.FormatCsprojFile(project.FullPath);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            if (evalProject is not null)
            {
                ProjectCollection.GlobalProjectCollection.UnloadProject(evalProject);
                ProjectCollection.GlobalProjectCollection.TryUnloadProject(project);
            }
        }
    }
}
