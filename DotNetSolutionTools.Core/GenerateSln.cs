using System.Diagnostics;
using Microsoft.Build.Construction;

namespace DotNetSolutionTools.Core;

public static class GenerateSln
{
    private const string BasePath = @"C:\Users\Matthew\Documents\Git\folder";

    public static async Task GenerateSlnWithDependenciesFromProjects(List<string> csprojPathList)
    {
        csprojPathList = [$"""{BasePath}/src/project1/project1.csproj"""];
        // Get csproj objects
        var projectObjects = csprojPathList.Select(ProjectRootElement.Open).ToList();

        // Initialize dependencies with the root projects
        var allDependencies = new HashSet<string>(csprojPathList);

        // Recursively find all dependencies
        FindAllDependenciesAsync(projectObjects, allDependencies);

        // At this point, allDependencies contains paths to all projects and their dependencies
        // Further processing to generate the solution file can be done here
        Console.WriteLine(allDependencies.Count);

        // delete the solution file if it exists
        if (File.Exists(BasePath + "\\Global.sln"))
        {
            File.Delete(BasePath + "\\Global.sln");
        }

        // powershell
        await RunPowerShellCommandAsync($"dotnet new sln -n Global -o {BasePath}");

        foreach (var project in allDependencies)
        {
            await RunPowerShellCommandAsync($"dotnet sln {BasePath}/Global.sln add {project}");
        }
    }

    private static void FindAllDependenciesAsync(List<ProjectRootElement> projects, HashSet<string> allDependencies)
    {
        var newDependencies = new List<ProjectRootElement>();

        foreach (var project in projects)
        {
            var dependencyPaths = project
                .Items.Where(x => x.ItemType == "ProjectReference")
                .Select(s => s.Include)
                .ToList();

            // projectrootelement can't handle paths with ../ so we need to remove them, but resolve them correctly
            dependencyPaths = dependencyPaths.Select(x => Path.GetFullPath(x, project.DirectoryPath)).ToList();

            foreach (var path in dependencyPaths)
            {
                // Add to the HashSet to prevent duplicates and check if the path was already processed
                if (allDependencies.Add(path))
                {
                    newDependencies.Add(ProjectRootElement.Open(path));
                }
            }
        }

        if (newDependencies.Any())
        {
            // Recursively process new dependencies found
            FindAllDependenciesAsync(newDependencies, allDependencies);
        }
    }

    private static async Task RunPowerShellCommandAsync(string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = command,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        await process.WaitForExitAsync();
        Console.WriteLine(await process.StandardOutput.ReadToEndAsync());
    }
}
