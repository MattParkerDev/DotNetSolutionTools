namespace DotNetSolutionTools.App.Models;

public class LocalStateDto
{
    public string SolutionFolderPath { get; set; } = string.Empty;
    public string SolutionFilePath { get; set; } = string.Empty;
    public string CsprojFilePath { get; set; } = string.Empty;
}