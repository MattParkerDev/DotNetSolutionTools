using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetSolutionTools.App.Models;
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
        ParityResults.Clear();
        ErrorMessages?.Clear();
        try
        {
            var results = SolutionProjectParity.CompareSolutionAndCSharpProjects(
                SolutionFolderPath,
                SolutionFilePath
            );
            foreach (var result in results)
                ParityResults.Add(result);
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }

    [RelayCommand]
    private async Task FormatCsProjFile(CancellationToken token)
    {
        FormatCsproj.FormatCsprojFile(CsprojFilePath);
    }

    [RelayCommand]
    private async Task FormatAllCsprojFilesInSolutionFile(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try
        {
            var csprojList = SolutionProjectParity.RetrieveAllCSharpProjectFullPathsFromFolder(
                SolutionFolderPath
            );
            foreach (var csproj in csprojList)
            {
                FormatCsproj.FormatCsprojFile(csproj);
            }
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }

    [RelayCommand]
    private async Task FormatAllCsprojFilesInSolutionFolder(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try
        {
            var csprojList = SolutionProjectParity.RetrieveAllCSharpProjectFullPathsFromFolder(
                SolutionFolderPath
            );
            foreach (var csproj in csprojList)
            {
                FormatCsproj.FormatCsprojFile(csproj);
            }
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }
    
    [RelayCommand]
    private async Task CheckForMissingImplicitUsingsInSolutionFile(CancellationToken token)
    {
        ErrorMessages?.Clear();
        ParityResults.Clear();
        try
        {
            var result = ImplicitUsings.FindCSharpProjectsMissingImplicitUsings(SolutionFilePath);
            result.ForEach(s => ParityResults.Add(s));
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
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
            await SaveLoadedState();
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
        await SaveLoadedState();
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
            await SaveLoadedState();
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
        await SaveLoadedState();
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
            await SaveLoadedState();
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
        await SaveLoadedState();
    }

    private async Task SaveLoadedState()
    {
        var dto = new LocalStateDto
        {
            SolutionFolderPath = SolutionFolderPath,
            SolutionFilePath = SolutionFilePath,
            CsprojFilePath = CsprojFilePath
        };
        var json = JsonSerializer.Serialize(dto);
        await File.WriteAllTextAsync("./localState.json", json);
    }
    
    private async Task LoadSavedState()
    {
        try
        {
            var json = await File.ReadAllTextAsync("./localState.json");
            if (string.IsNullOrEmpty(json))
                return;
            var dto = JsonSerializer.Deserialize<LocalStateDto>(json);
            if (dto is null)
                return;
            SolutionFolderPath = dto.SolutionFolderPath;
            SolutionFilePath = dto.SolutionFilePath;
            CsprojFilePath = dto.CsprojFilePath;
        }
        catch
        {
            // ignored
        }
    }
    
    public MainWindowViewModel()
    {
        LoadSavedState().ConfigureAwait(false);
    }
}
