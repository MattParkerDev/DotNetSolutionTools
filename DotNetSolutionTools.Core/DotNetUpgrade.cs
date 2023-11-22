using Microsoft.Build.Construction;
using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;

namespace DotNetSolutionTools.Core;

public static class DotNetUpgrade
{
    public static void UpdateProjectsInSolutionToNet80(string solutionFilePath)
    {
        var solutionFile = SolutionFile.Parse(solutionFilePath);
        var csprojList = SolutionProjectParity.GetCSharpProjectObjectsFromSolutionFile(
            solutionFile
        );
        UpdateProjectsToNet80(csprojList);
    }

    private static void UpdateProjectsToNet80(List<ProjectRootElement> projects)
    {
        foreach (var project in projects)
        {
            var evalProject = Project.FromProjectRootElement(project, new ProjectOptions(){LoadSettings = ProjectLoadSettings.IgnoreMissingImports});
            var packages = evalProject.GetItems("PackageReference");
            //packages.First().Metadata.First().UnevaluatedValue = "9.0.0";
            var targetFramework = project.PropertyGroups
                .SelectMany(x => x.Properties)
                .FirstOrDefault(x => x.Name == "TargetFramework");
            if (targetFramework?.Value is "net7.0")
            {
                targetFramework.Value = "net8.0";
                project.Save();
                FormatCsproj.FormatCsprojFile(project.FullPath);
            }
        }
    }
}