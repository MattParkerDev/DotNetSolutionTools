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
    private string _solutionFolderPath = string.Empty;

    [ObservableProperty]
    private string _solutionFilePath = string.Empty;

    [ObservableProperty]
    private string _csprojFilePath = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _operationResults = new() { };

    [ObservableProperty]
    private string _resultsLabel = "Ready";

    [RelayCommand]
    private async Task ExecuteParityChecker(CancellationToken token)
    {
        OperationResults.Clear();
        ResultsLabel = string.Empty;
        try
        {
            ResultsLabel = "Running...";
            await Task.Run(() =>
            { 
                var results = SolutionProjectParity.CompareSolutionAndCSharpProjects(SolutionFolderPath, SolutionFilePath);
                foreach (var result in results)
                    OperationResults.Add(result);
                ResultsLabel = $"{results.Count} Projects in folder missing from solution";
            }, token);
        }
        catch (Exception e)
        {
            ResultsLabel = "Error";
            OperationResults?.Add(e.Message);
        }
    }

    [RelayCommand]
    private async Task FormatCsProjFile(CancellationToken token)
    {
        OperationResults.Clear();
        ResultsLabel = string.Empty;
        try
        {
            ResultsLabel = "Running...";
            await Task.Run(() =>
            { 
                FormatCsproj.FormatCsprojFile(CsprojFilePath);
            }, token);
            ResultsLabel = "Successfully formatted csproj file";
        }
        catch (Exception e)
        {
            ResultsLabel = "Failed to format csproj file";
            OperationResults?.Add(e.Message);
            OperationResults?.Add(e.ToString());
        }
        
    }

    [RelayCommand]
    private async Task FormatAllCsprojFilesInSolutionFile(CancellationToken token)
    {
        OperationResults.Clear();
        ResultsLabel = string.Empty;
        try
        {
            ResultsLabel = "Running...";
            await Task.Run(() =>
            { 
                var csprojList = SolutionProjectParity.RetrieveAllCSharpProjectFullPathsFromFolder(SolutionFolderPath);
                foreach (var csproj in csprojList)
                {
                    FormatCsproj.FormatCsprojFile(csproj);
                }
            }, token);
        }
        catch (Exception e)
        {
            ResultsLabel = "Failed to format all csproj files in solution file";
            OperationResults?.Add(e.Message);
            OperationResults?.Add(e.ToString());
        }
    }

    [RelayCommand]
    private async Task FormatAllCsprojFilesInSolutionFolder(CancellationToken token)
    {
        OperationResults.Clear();
        try
        {
            ResultsLabel = "Running...";
            await Task.Run(() =>
            { 
                var csprojList = SolutionProjectParity.RetrieveAllCSharpProjectFullPathsFromFolder(SolutionFolderPath);
                foreach (var csproj in csprojList)
                {
                    FormatCsproj.FormatCsprojFile(csproj);
                }
            }, token);
            
        }
        catch (Exception e)
        {
            ResultsLabel = "Failed to format all csproj files in solution folder";
            OperationResults?.Add(e.Message);
            OperationResults?.Add(e.ToString());
        }
    }

    [RelayCommand]
    private async Task CheckForMissingImplicitUsingsInSolutionFile(CancellationToken token)
    {
        OperationResults.Clear();
        ResultsLabel = string.Empty;
        try
        {
            ResultsLabel = "Running...";
            await Task.Run(() =>
            { 
                var results = ImplicitUsings.FindCSharpProjectsMissingImplicitUsings(SolutionFilePath);
                results.ForEach(s => OperationResults.Add(s));
                ResultsLabel = $"{results.Count} Projects missing ImplicitUsings";
            }, token);
        }
        catch (Exception e)
        {
            ResultsLabel = "Failed to check for missing implicit usings in solution file";
            OperationResults?.Add(e.Message);
            OperationResults?.Add(e.ToString());
        }
    }

    [RelayCommand]
    private async Task CheckForMissingTreatWarningsAsErrorsInSolutionFile(CancellationToken token)
    {
        OperationResults.Clear();
        ResultsLabel = string.Empty;
        try
        {
            ResultsLabel = "Running...";
            await Task.Run(() =>
            { 
                var results = WarningsAsErrors.FindCSharpProjectsMissingTreatWarningsAsErrors(SolutionFilePath);
                results.ForEach(s => OperationResults.Add(s));
                ResultsLabel = $"{results.Count} Projects missing TreatWarningsAsErrors";
            }, token);
        }
        catch (Exception e)
        {
            ResultsLabel = "Error";
            OperationResults?.Add(e.Message);
            OperationResults?.Add(e.ToString());
        }
    }

    [RelayCommand]
    private async Task DeleteBinAndObjFoldersInFolder(CancellationToken token)
    {
        OperationResults.Clear();
        ResultsLabel = string.Empty;
        try
        {
            ResultsLabel = "Running...";
            await Task.Run(() =>
            { 
                CleanFolder.DeleteBinObjAndNodeModulesFoldersInFolder(SolutionFolderPath);
            }, token);
            ResultsLabel = "Successfully deleted bin and obj folders";
        }
        catch (Exception e)
        {
            ResultsLabel = "Failed to delete bin and obj folders";
            OperationResults?.Add(e.Message);
            OperationResults?.Add(e.ToString());

        }
    }

    [RelayCommand]
    private async Task UpdateAllProjectsToNet80(CancellationToken token)
    {
        OperationResults.Clear();
        ResultsLabel = string.Empty;
        try
        {
            ResultsLabel = "Running...";
            await Task.Run(async () =>
            { 
                await DotNetUpgrade.UpdateProjectsInSolutionToNet80(SolutionFilePath);
            }, token);
            ResultsLabel = "Successfully updated all projects in solution to .NET 8";
        }
        catch (Exception e)
        {
            ResultsLabel = "Failed to update all projects in solution to .NET 8";
            OperationResults?.Add(e.Message);
            OperationResults?.Add(e.ToString());
        }
    }

    [RelayCommand]
    private async Task UpdateProjectToNet80(CancellationToken token)
    {
        OperationResults.Clear();
        ResultsLabel = string.Empty;
        try
        {
            ResultsLabel = "Running...";
            await Task.Run(async () =>
            { 
                await DotNetUpgrade.UpdateProjectAtPathToNet80(CsprojFilePath);
            }, token);
            ResultsLabel = "Successfully updated project to .NET 8";
        }
        catch (Exception e)
        {
            ResultsLabel = "Failed to update project to .NET 8";
            OperationResults?.Add(e.Message);
        }
    }

    [RelayCommand]
    private async Task LoadSolutionFile(CancellationToken token)
    {
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
            ResultsLabel = "Error";
            OperationResults?.Add(e.Message);
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
            ResultsLabel = "Error";
            OperationResults?.Add(e.Message);
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
            ResultsLabel = "Error";
            OperationResults?.Add(e.Message);
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
