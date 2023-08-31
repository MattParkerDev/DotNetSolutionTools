using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DotNetSolutionTools.App.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    protected ViewModelBase()
    {
        ErrorMessages = new ObservableCollection<string>();
    }

    [ObservableProperty]
    private ObservableCollection<string>? _errorMessages;
}
