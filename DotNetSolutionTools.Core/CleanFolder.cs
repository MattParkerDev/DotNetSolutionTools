namespace DotNetSolutionTools.Core;

public static class CleanFolder
{
    public static void DeleteBinObjAndNodeModulesFoldersInFolder(string folderPath)
    {
        var nodeModulesFolders = Directory
            .GetDirectories(folderPath, "*", SearchOption.AllDirectories)
            .Where(x => x.EndsWith(Path.DirectorySeparatorChar + "node_modules"))
            .ToList();

        foreach (var folder in nodeModulesFolders)
        {
            Directory.Delete(folder, true);
        }

        var binAndObjFolders = Directory
            .GetDirectories(folderPath, "*", SearchOption.AllDirectories)
            .Where(x =>
                x.EndsWith(Path.DirectorySeparatorChar + "bin") || x.EndsWith(Path.DirectorySeparatorChar + "obj")
            )
            .ToList();

        foreach (var folder in binAndObjFolders)
        {
            Directory.Delete(folder, true);
        }
    }
}
