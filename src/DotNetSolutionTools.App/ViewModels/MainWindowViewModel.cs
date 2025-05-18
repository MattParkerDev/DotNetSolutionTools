using System.Collections.ObjectModel;
using System.Text.Json;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetSolutionTools.App.Models;
using DotNetSolutionTools.App.Services;
using DotNetSolutionTools.Core;
using DotNetSolutionTools.Core.Common;
using NuGet.Packaging;

namespace DotNetSolutionTools.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
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

    [ObservableProperty]
    private IImmutableSolidColorBrush _resultsLabelColor = new ImmutableSolidColorBrush(Colors.Black);

    public MainWindowViewModel()
    {
        LoadSavedState().ConfigureAwait(false);
    }

    private void SetBeginCommandState()
    {
        OperationResults.Clear();
        ResultsLabel = "Running...";
        ResultsLabelColor = new ImmutableSolidColorBrush(Colors.Black);
    }

    private void SetCommandSuccessState(string message, IEnumerable<string>? results = null)
    {
        ResultsLabel = message;
        ResultsLabelColor = new ImmutableSolidColorBrush(Colors.Green);
        if (results is not null)
            OperationResults.AddRange(results);
    }

    private void SetCommandFailureState(string message, Exception e)
    {
        ResultsLabel = message;
        ResultsLabelColor = new ImmutableSolidColorBrush(Colors.DarkRed);
        OperationResults?.Add(e.Message);
        OperationResults?.Add(e.ToString());
    }

    [RelayCommand]
    private async Task ExecuteParityChecker(CancellationToken token)
    {
        SetBeginCommandState();
        try
        {
            await Task.Run(
                () =>
                {
                    var results = SolutionProjectParity.CompareSolutionAndCSharpProjects(
                        SolutionFolderPath,
                        SolutionFilePath
                    );
                    SetCommandSuccessState($"{results.Count} Projects in folder missing from solution", results);
                },
                token
            );
        }
        catch (Exception e)
        {
            SetCommandFailureState("Failed to compare solution and csharp projects", e);
        }
    }

    [RelayCommand]
    private async Task FormatCsProjFile(CancellationToken token)
    {
        SetBeginCommandState();
        try
        {
            await Task.Run(
                () =>
                {
                    FormatCsproj.FormatCsprojFile(CsprojFilePath);
                },
                token
            );
            SetCommandSuccessState("Successfully formatted csproj file");
        }
        catch (Exception e)
        {
            SetCommandFailureState("Failed to format csproj file", e);
        }
    }

    [RelayCommand]
    private async Task FormatAllCsprojFilesInSolutionFile(CancellationToken token)
    {
        SetBeginCommandState();
        try
        {
            await Task.Run(
                () =>
                {
                    var csprojList = CsprojHelper.RetrieveAllCSharpProjectFullPathsFromSolution(SolutionFilePath);
                    foreach (var csproj in csprojList)
                    {
                        FormatCsproj.FormatCsprojFile(csproj);
                    }
                },
                token
            );
            SetCommandSuccessState("Successfully formatted all csproj files in solution file");
        }
        catch (Exception e)
        {
            SetCommandFailureState("Failed to format all csproj files in solution file", e);
        }
    }

    [RelayCommand]
    private async Task FormatAllCsprojFilesInSolutionFolder(CancellationToken token)
    {
        SetBeginCommandState();
        try
        {
            await Task.Run(
                () =>
                {
                    var csprojList = CsprojHelper.RetrieveAllCSharpProjectFullPathsFromFolder(SolutionFolderPath);
                    foreach (var csproj in csprojList)
                    {
                        FormatCsproj.FormatCsprojFile(csproj);
                    }
                },
                token
            );
            SetCommandSuccessState("Successfully formatted all csproj files in folder");
        }
        catch (Exception e)
        {
            SetCommandFailureState("Failed to format all csproj files in folder", e);
        }
    }

    [RelayCommand]
    private async Task ResolveInconsistentNugetPackageVersionsInSolutionFile(CancellationToken token)
    {
        SetBeginCommandState();
        try
        {
            await Task.Run(
                () =>
                {
                    PackageVersionConsistency.ResolvePackageVersionsToLatestInstalled(SolutionFilePath);
                    SetCommandSuccessState("Resolved all packages with inconsistent versions");
                },
                token
            );
        }
        catch (Exception e)
        {
            SetCommandFailureState("Failed to resolve inconsistent package versions in solution file", e);
        }
    }

    [RelayCommand]
    private async Task CheckForInconsistentNugetPackageVersionsInSolutionFile(CancellationToken token)
    {
        SetBeginCommandState();
        try
        {
            await Task.Run(
                () =>
                {
                    var results = PackageVersionConsistency.FindInconsistentNugetPackageVersions(SolutionFilePath);
                    SetCommandSuccessState($"{results.Count} packages with inconsistent versions", results);
                },
                token
            );
        }
        catch (Exception e)
        {
            SetCommandFailureState("Failed to check for inconsistent package versions in solution file", e);
        }
    }

    [RelayCommand]
    private async Task CheckForMissingImplicitUsingsInSolutionFile(CancellationToken token)
    {
        SetBeginCommandState();
        try
        {
            await Task.Run(
                () =>
                {
                    var results = ImplicitUsings.FindCSharpProjectsMissingImplicitUsings(SolutionFilePath);
                    SetCommandSuccessState($"{results.Count} Projects missing ImplicitUsings", results);
                },
                token
            );
        }
        catch (Exception e)
        {
            SetCommandFailureState("Failed to check for missing implicit usings in solution file", e);
        }
    }

    [RelayCommand]
    private async Task CheckForMissingTreatWarningsAsErrorsInSolutionFile(CancellationToken token)
    {
        SetBeginCommandState();
        try
        {
            await Task.Run(
                () =>
                {
                    var results = WarningsAsErrors.FindCSharpProjectsMissingTreatWarningsAsErrors(SolutionFilePath);
                    SetCommandSuccessState($"{results.Count} Projects missing TreatWarningsAsErrors", results);
                },
                token
            );
        }
        catch (Exception e)
        {
            SetCommandFailureState("Failed to check for missing treat warnings as errors in solution file", e);
        }
    }

    [RelayCommand]
    private async Task DeleteBinAndObjFoldersInFolder(CancellationToken token)
    {
        SetBeginCommandState();
        try
        {
            await Task.Run(
                () =>
                {
                    CleanFolder.DeleteBinObjAndNodeModulesFoldersInFolder(SolutionFolderPath);
                },
                token
            );
            SetCommandSuccessState("Successfully deleted bin and obj folders");
        }
        catch (Exception e)
        {
            SetCommandFailureState("Failed to delete bin and obj folders", e);
        }
    }

    [RelayCommand]
    private async Task UpdateAllProjectsToNet80(CancellationToken token)
    {
        SetBeginCommandState();
        try
        {
            await Task.Run(
                async () =>
                {
                    await DotNetUpgrade.UpdateProjectsInSolutionToNet80(SolutionFilePath);
                },
                token
            );
            SetCommandSuccessState("Successfully updated all projects in solution to .NET 8");
        }
        catch (Exception e)
        {
            SetCommandFailureState("Failed to update all projects in solution to .NET 8", e);
        }
    }

    [RelayCommand]
    private async Task UpdateProjectToNet80(CancellationToken token)
    {
        SetBeginCommandState();
        try
        {
            await Task.Run(
                async () =>
                {
                    await DotNetUpgrade.UpdateProjectAtPathToNet80(CsprojFilePath);
                },
                token
            );
            SetCommandSuccessState("Successfully updated project to .NET 8");
        }
        catch (Exception e)
        {
            SetCommandFailureState("Failed to update project to .NET 8", e);
        }
    }

    [RelayCommand]
    private async Task LoadSolutionFile(CancellationToken token)
    {
        try
        {
            var file = await _fileService.DoOpenFilePickerSlnAsync();
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
            var folder = await _fileService.DoOpenFilePickerCsprojAsync();
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
}
