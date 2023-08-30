using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SolutionParityChecker.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _solutionFolderPath;

    [ObservableProperty]
    private string _solutionFilePath;

    [ObservableProperty]
    private string? _fileText;

    [ObservableProperty]
    private ObservableCollection<string> _parityResults = new ObservableCollection<string>()
    {
        "Test"
    };

    [RelayCommand]
    private async Task ExecuteParityChecker(CancellationToken token)
    {
        var results = SolutionParityChecker.CompareSolutionAndCSharpProjects(
            SolutionFolderPath,
            SolutionFilePath
        );
        ParityResults.Clear();
        foreach (var result in results)
        {
            ParityResults.Add(result);
        }
    }
    
    [RelayCommand]
    private async Task FormatCsProjFile(CancellationToken token)
    {
        FormatCsproj.FormatCsprojFile(SolutionFilePath);
    }

    [RelayCommand]
    private async Task LoadSolutionFile(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try
        {
            var file = await DoOpenFilePickerAsync();
            if (file is null)
                return;

            SolutionFilePath = file.Path.AbsolutePath;
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }

    [RelayCommand]
    private async Task LoadSolutionFolder(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try
        {
            var folder = await DoOpenFolderPickerAsync();
            if (folder is null)
                return;

            SolutionFolderPath = folder.Path.AbsolutePath;
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }

    private async Task<IStorageFile?> DoOpenFilePickerAsync()
    {
        // For learning purposes, we opted to directly get the reference
        // for StorageProvider APIs here inside the ViewModel.

        // For your real-world apps, you should follow the MVVM principles
        // by making service classes and locating them with DI/IoC.

        // See IoCFileOps project for an example of how to accomplish this.
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

    private async Task<IStorageFolder?> DoOpenFolderPickerAsync()
    {
        // For learning purposes, we opted to directly get the reference
        // for StorageProvider APIs here inside the ViewModel.

        // For your real-world apps, you should follow the MVVM principles
        // by making service classes and locating them with DI/IoC.

        // See IoCFileOps project for an example of how to accomplish this.
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
