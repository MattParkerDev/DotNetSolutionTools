namespace DotNetSolutionTools.Core.Models;

public class Project
{
    public required string FullPath { get; set; }
    public required string Name { get; set; }
    public List<Project> DependsOn { get; set; } = [];
}
