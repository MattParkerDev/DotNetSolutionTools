using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DotNetSolutionTools.App.ViewModels;

public partial class PreviewMainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _solutionFolderPath =
        "C:\\Users\\matt\\source\\repos\\DotNetSolutionTools\\DotNetSolutionTools.App";

    [ObservableProperty]
    private string _solutionFilePath =
        "C:\\Users\\matt\\source\\repos\\DotNetSolutionTools\\DotNetSolutionTools.App.sln";

    [ObservableProperty]
    private string _csprojFilePath =
        "C:\\Users\\matt\\source\\repos\\DotNetSolutionTools\\DotNetSolutionTools.App.csproj";

    [ObservableProperty]
    private ObservableCollection<string> _parityResults = new() { "Error Message" };

    [RelayCommand]
    private async Task ExecuteParityChecker(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private async Task FormatCsProjFile(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private async Task LoadSolutionFile(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private async Task LoadSolutionFolder(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private async Task LoadCsprojFile(CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
