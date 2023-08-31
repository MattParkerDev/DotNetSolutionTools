using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetSolutionTools.App.Services;
using DotNetSolutionTools.Core;

namespace DotNetSolutionTools.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly FileService _fileService = new();

    [ObservableProperty]
    private string _solutionFolderPath;

    [ObservableProperty]
    private string _solutionFilePath;

    [ObservableProperty]
    private string _csprojFilePath;

    [ObservableProperty]
    private ObservableCollection<string> _parityResults = new() { };

    [RelayCommand]
    private async Task ExecuteParityChecker(CancellationToken token)
    {
        var results = SolutionProjectParity.CompareSolutionAndCSharpProjects(
            SolutionFolderPath,
            SolutionFilePath
        );
        ParityResults.Clear();
        foreach (var result in results)
            ParityResults.Add(result);
    }

    [RelayCommand]
    private async Task FormatCsProjFile(CancellationToken token)
    {
        FormatCsproj.FormatCsprojFile(CsprojFilePath);
    }

    [RelayCommand]
    private async Task FormatAllCsprojFilesInSolutionFile(CancellationToken token)
    {
        var csprojList = SolutionProjectParity.RetrieveAllCSharpProjectFullPathsFromFolder(
            SolutionFolderPath
        );
        foreach (var csproj in csprojList)
        {
            FormatCsproj.FormatCsprojFile(csproj);
        }
    }

    [RelayCommand]
    private async Task FormatAllCsprojFilesInSolutionFolder(CancellationToken token)
    {
        var csprojList = SolutionProjectParity.RetrieveAllCSharpProjectFullPathsFromFolder(
            SolutionFolderPath
        );
        foreach (var csproj in csprojList)
        {
            FormatCsproj.FormatCsprojFile(csproj);
        }
    }
    
    [RelayCommand]
    private async Task CheckForMissingImplicitUsingsInSolutionFile(CancellationToken token)
    {
        ImplicitUsings.FindCSharpProjectsMissingImplicitUsings(SolutionFilePath);
    }

    [RelayCommand]
    private async Task LoadSolutionFile(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try
        {
            var file = await _fileService.DoOpenFilePickerAsync();
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
    private async Task ClearSolutionFile(CancellationToken token)
    {
        SolutionFilePath = string.Empty;
    }

    [RelayCommand]
    private async Task LoadSolutionFolder(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try
        {
            var folder = await _fileService.DoOpenFolderPickerAsync();
            if (folder is null)
                return;

            SolutionFolderPath = folder.Path.AbsolutePath;
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }

    [RelayCommand]
    private async Task ClearSolutionFolder(CancellationToken token)
    {
        SolutionFolderPath = string.Empty;
    }

    [RelayCommand]
    private async Task LoadCsprojFile(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try
        {
            var folder = await _fileService.DoOpenFilePickerAsync();
            if (folder is null)
                return;

            CsprojFilePath = folder.Path.AbsolutePath;
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }

    [RelayCommand]
    private async Task ClearCsprojFile(CancellationToken token)
    {
        CsprojFilePath = string.Empty;
    }
}
