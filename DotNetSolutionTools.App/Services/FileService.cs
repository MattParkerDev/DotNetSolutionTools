using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace DotNetSolutionTools.App.Services;

public class FileService
{
    private readonly FilePickerFileType _csprojFileType = new("C# Project File") { Patterns = new[] { "*.csproj" } };
    private readonly FilePickerFileType _slnFileType = new("C# Solution File") { Patterns = new[] { "*.sln" } };

    public async Task<IStorageFile?> DoOpenFilePickerCsprojAsync()
    {
        return await DoOpenFilePickerAsync(_csprojFileType);
    }

    public async Task<IStorageFile?> DoOpenFilePickerSlnAsync()
    {
        return await DoOpenFilePickerAsync(_slnFileType);
    }

    public async Task<IStorageFile?> DoOpenFilePickerAsync(FilePickerFileType fileType)
    {
        if (
            Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop
            || desktop.MainWindow?.StorageProvider is not { } provider
        )
            throw new NullReferenceException("Missing StorageProvider instance.");

        var files = await provider.OpenFilePickerAsync(
            new FilePickerOpenOptions()
            {
                Title = "Open File",
                AllowMultiple = false,
                FileTypeFilter = [fileType]
            }
        );

        return files?.Count >= 1 ? files[0] : null;
    }

    public async Task<IStorageFolder?> DoOpenFolderPickerAsync()
    {
        if (
            Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop
            || desktop.MainWindow?.StorageProvider is not { } provider
        )
            throw new NullReferenceException("Missing StorageProvider instance.");

        var folder = await provider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions() { Title = "Select Folder", AllowMultiple = false }
        );

        return folder?.Count >= 1 ? folder[0] : null;
    }
}
