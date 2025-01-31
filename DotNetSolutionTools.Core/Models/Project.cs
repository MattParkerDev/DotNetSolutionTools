namespace DotNetSolutionTools.Core.Models;

public class Project
{
    public string Id { get; set; } = null!;
    public required string FullPath { get; set; }
    public required string Name { get; set; }
    public List<Project> DependsOn { get; set; } = [];
    public List<Project> Dependents { get; set; } = [];

    public List<Project> GetNestedDependencies()
    {
        List<Project> dependencies = [];
        foreach (var dependency in DependsOn)
        {
            dependencies.Add(dependency);
            dependencies.AddRange(dependency.GetNestedDependencies());
        }

        return dependencies.Distinct().ToList();
    }
}
