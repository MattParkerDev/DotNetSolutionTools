using DotNetSolutionTools.Core.Common;
using DotNetSolutionTools.Core.Models;
using Microsoft.Build.Construction;

namespace DotNetSolutionTools.Core;

public static class SolutionBuildOrder
{
    public static List<ProjectRootElement> Projects { get; set; } = [];

    public static string GetBuildOrder(string solutionFilePath)
    {
        var solutionFile = SlnHelper.ParseSolutionFileFromPath(solutionFilePath);
        ArgumentNullException.ThrowIfNull(solutionFile);
        var projects = SlnHelper.GetCSharpProjectObjectsFromSolutionFile(solutionFile);
        Projects = projects;

        List<Project> projects2 = [];
        foreach (var project in projects)
        {
            var project2 = new Project { FullPath = project.FullPath };
            project2.DependsOn = GetDependencies(project2);
            projects2.Add(project2);
        }

        return "";
    }

    public static List<Project> GetDependencies(Project project)
    {
        var projectReferences = Projects
            .Single(s => s.FullPath == project.FullPath)
            .AllChildren.OfType<ProjectItemElement>()
            .Where(x => x.ElementName == "ProjectReference")
            .ToList();

        List<Project> dependencies = [];
        foreach (var projectReference in projectReferences)
        {
            var fullPath = Path.Combine(Path.GetDirectoryName(project.FullPath)!, projectReference.Include);
            fullPath = Path.GetFullPath(fullPath);
            var dependency = Projects.Single(s => s.FullPath == fullPath);
            var subProject = new Project { FullPath = dependency.FullPath };
            subProject.DependsOn = GetDependencies(subProject);
            dependencies.Add(subProject);
        }

        return dependencies;
    }
}
