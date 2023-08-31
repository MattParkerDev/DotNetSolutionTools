using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace DotNetSolutionTools.App.Services;

public class FileService
{
    public async Task<IStorageFile?> DoOpenFilePickerAsync()
    {
        if (
            Application.Current?.ApplicationLifetime
                is not IClassicDesktopStyleApplicationLifetime desktop
            || desktop.MainWindow?.StorageProvider is not { } provider
        )
            throw new NullReferenceException("Missing StorageProvider instance.");

        var files = await provider.OpenFilePickerAsync(
            new FilePickerOpenOptions() { Title = "Open Text File", AllowMultiple = false }
        );

        return files?.Count >= 1 ? files[0] : null;
    }

    public async Task<IStorageFolder?> DoOpenFolderPickerAsync()
    {
        if (
            Application.Current?.ApplicationLifetime
                is not IClassicDesktopStyleApplicationLifetime desktop
            || desktop.MainWindow?.StorageProvider is not { } provider
        )
            throw new NullReferenceException("Missing StorageProvider instance.");

        var folder = await provider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions() { Title = "Open Text File", AllowMultiple = false }
        );

        return folder?.Count >= 1 ? folder[0] : null;
    }
}
