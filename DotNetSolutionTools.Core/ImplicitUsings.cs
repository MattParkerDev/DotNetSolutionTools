using Microsoft.Build.Construction;

namespace DotNetSolutionTools.Core;

public static class ImplicitUsings
{
    public static List<ProjectRootElement> FindCSharpProjectsMissingImplicitUsings(List<ProjectRootElement> projectList)
    {
        var projectsMissingImplicitUsings = new List<ProjectRootElement>();
        
        foreach (var project in projectList)
        {
            var implicitUsings = project.PropertyGroups
                .SelectMany(x => x.Properties)
                .FirstOrDefault(x => x.Name == "ImplicitUsings");
            if (implicitUsings is null || implicitUsings.Value is not "enable")
            {
                projectsMissingImplicitUsings.Add(project);
            }
        }

        return projectsMissingImplicitUsings;
    }

    public static void AddMissingImplicitUsings(List<ProjectRootElement> projectsMissingImplicitUsings)
    {
        foreach (var project in projectsMissingImplicitUsings)
        {
            if (ProjectIsMissingImplicitUsings(project))
            {
                project.AddProperty("ImplicitUsings", "enable");
                project.Save();
                FormatCsproj.FormatCsprojFile(project.FullPath);
            }
        }
    }

    public static void EnableDisabledImplicitUsings(List<ProjectRootElement> projectsMissingImplicitUsings)
    {
        throw new NotImplementedException();
    }

    public static void EnableAllImplicitUsings(List<ProjectRootElement> projectsMissingImplicitUsings)
    {
        throw new NotImplementedException();
    }
    
    public static bool ProjectIsMissingImplicitUsings(ProjectRootElement project)
    {
        var implicitUsings = project.PropertyGroups
            .SelectMany(x => x.Properties)
            .FirstOrDefault(x => x.Name == "ImplicitUsings");
        if (implicitUsings is null)
        {
            return true;
        }

        return false;
    }
}