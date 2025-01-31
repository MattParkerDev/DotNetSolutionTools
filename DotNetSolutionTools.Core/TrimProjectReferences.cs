using DotNetSolutionTools.Core.Common;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace DotNetSolutionTools.Core;

public class ProjectAndReferences
{
    public ProjectRootElement Project { get; set; }
    public List<ProjectItemElement> ProjectReferences { get; set; }
}

public static class TrimProjectReferences
{
    public static List<string> TrimUnusedProjectReferences(string solutionFilePath)
    { 
        //var csharpProjects = CsprojHelper.RetrieveAllCSharpProjectFullPathsFromSolution(solutionFilePath);
        var solutionFile = SlnHelper.ParseSolutionFileFromPath(solutionFilePath);
        ArgumentNullException.ThrowIfNull(solutionFile);
        var projects = SlnHelper.GetCSharpProjectObjectsFromSolutionFile(solutionFile);
        
        var projectReferences = projects.Select(project =>
        {
            //var projectReferences = project.Items.Where(x => x.ItemType == "ProjectReference").ToList();
            var projectReferences = project.AllChildren.OfType<ProjectItemElement>().Where(x => x.ElementName == "ProjectReference").ToList();
            return new ProjectAndReferences
            {
                Project = project,
                ProjectReferences = projectReferences
            };
        }).ToList();

        var testProject = projectReferences.First();
        var projectReferenceToRemove = testProject.ProjectReferences.First();
        var test = testProject.Project.AllChildren.First(s => s == projectReferenceToRemove);
        //test.ContainingProject.RemoveChild(test);
        //testProject.Project.RemoveChild(projectReferenceToRemove);
        //testProject.Project.Items.Remove(testProject.ProjectReferences.First());
        //testProject.Project.Save();
        var buildManager = BuildManager.DefaultBuildManager;
        var globalProperties = new Dictionary<string, string>
        {
            { "Configuration", "Debug" },
            { "FastBuild", "true" } 
            
            // Other properties as needed
        };        var buildRequestData = new BuildRequestData(solutionFilePath, globalProperties, null, new string[] { "Build" }, null);
        var buildParameters = new BuildParameters(ProjectCollection.GlobalProjectCollection)
        {
            Loggers = new List<ILogger> { new ConsoleLogger(LoggerVerbosity.Quiet) }
        };

        var buildResult = buildManager.Build(buildParameters, buildRequestData);

        return [];
    }
}