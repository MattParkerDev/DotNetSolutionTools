using Microsoft.Build.Construction;

namespace DotNetSolutionTools.Core;

public static class WarningsAsErrors
{
    public static List<string> FindCSharpProjectsMissingTreatWarningsAsErrors(string solutionFilePath)
    {
        var solutionFile = SolutionFile.Parse(solutionFilePath);
        var csprojList = SolutionProjectParity.GetCSharpProjectObjectsFromSolutionFile(
            solutionFile
        );
        var projectsMissingImplicitUsings = FindCSharpProjectsMissingTreatWarningsAsErrors(csprojList);
        var projectsMissingImplicitUsingsStringList = projectsMissingImplicitUsings
            .Select(x => x.FullPath)
            .ToList();

        return projectsMissingImplicitUsingsStringList;
    }

    public static List<ProjectRootElement> FindCSharpProjectsMissingTreatWarningsAsErrors(
        List<ProjectRootElement> projectList
    )
    {
        var projectsMissingTreatWarningsAsErrors = new List<ProjectRootElement>();

        foreach (var project in projectList)
        {
            var treatWarningsAsErrors = project.PropertyGroups
                .SelectMany(x => x.Properties)
                .FirstOrDefault(x => x.Name == "TreatWarningsAsErrors");
            if (treatWarningsAsErrors is null || treatWarningsAsErrors.Value is not "true")
            {
                projectsMissingTreatWarningsAsErrors.Add(project);
            }
        }

        return projectsMissingTreatWarningsAsErrors;
    }

    public static void AddMissingTreatWarningsAsErrors(
        List<ProjectRootElement> projectsMissingImplicitUsings
    )
    {
        foreach (var project in projectsMissingImplicitUsings)
        {
            if (ProjectIsMissingTreatWarningsAsErrors(project))
            {
                
                project.AddProperty("TreatWarningsAsErrors", "true");
                project.Save();
                FormatCsproj.FormatCsprojFile(project.FullPath);
            }
        }
    }

    public static bool ProjectIsMissingTreatWarningsAsErrors(ProjectRootElement project)
    {
        var implicitUsings = project.PropertyGroups
            .SelectMany(x => x.Properties)
            .FirstOrDefault(x => x.Name == "TreatWarningsAsErrors");
        if (implicitUsings is null)
        {
            return true;
        }

        return false;
    }

    public static bool ProjectBuildSuccessfully(ProjectRootElement project)
    {
        // build the project
        var buildProject = new Microsoft.Build.Evaluation.Project(project); 
        // retrieve warnings
        var buildResult = buildProject.Build();
        return buildResult;
    }
}
