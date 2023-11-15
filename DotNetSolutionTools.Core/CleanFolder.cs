
namespace DotNetSolutionTools.Core;

public static class CleanFolder
{
    public static void DeleteFolderWithOnlyBinAndObjSubFolders(string folderPath)
    {
        var binAndObjFolders = Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories)
            .Where(x => x.EndsWith(Path.DirectorySeparatorChar + "bin") || x.EndsWith(Path.DirectorySeparatorChar + "obj"))
            .ToList();
        
        foreach (var folder in binAndObjFolders)
        {
            Directory.Delete(folder, true);
        }
    }
}