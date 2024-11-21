using DotNetSolutionTools.Core.Common;
using DotNetSolutionTools.Core.Models;
using Microsoft.Build.Construction;

namespace DotNetSolutionTools.Core;

public static class SolutionBuildOrder
{
    public static List<ProjectRootElement> Projects { get; set; } = [];
    public static HashSet<Project> MappedProjects { get; set; } = [];

    public static List<Project> GetBuildOrder(string solutionFilePath)
    {
        var solutionFile = SlnHelper.ParseSolutionFileFromPath(solutionFilePath);
        ArgumentNullException.ThrowIfNull(solutionFile);
        var projects = SlnHelper.GetCSharpProjectObjectsFromSolutionFile(solutionFile);
        Projects = projects;

        foreach (var project in projects)
        {
            var projectName = Path.GetFileNameWithoutExtension(project.FullPath);
            var project2 = new Project { FullPath = project.FullPath, Name = projectName };
            MappedProjects.Add(project2);
        }

        foreach (var project in MappedProjects)
        {
            project.DependsOn = GetDependencies(project);
        }

        return MappedProjects.ToList();
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
            var subProject = MappedProjects.Single(s => s.FullPath == dependency.FullPath);
            if (subProject is null)
            {
                var projectName = Path.GetFileNameWithoutExtension(dependency.FullPath);
                subProject = new Project { FullPath = dependency.FullPath, Name = projectName };
                MappedProjects.Add(subProject);
            }

            subProject.DependsOn = GetDependencies(subProject);
            dependencies.Add(subProject);
        }

        return dependencies;
    }
}
