namespace DotNetSolutionTools.Photino.Models;

public class AppState
{
    public required string SolutionFolderPath { get; set; }
    public required string SolutionFilePath { get; set; }
    public required string CsprojFilePath { get; set; }
}
