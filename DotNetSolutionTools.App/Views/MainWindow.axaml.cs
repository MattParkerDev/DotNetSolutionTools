using Avalonia.Controls;
using Microsoft.Build.Locator;

namespace DotNetSolutionTools.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        var instance = MSBuildLocator
            .QueryVisualStudioInstances()
            .OrderByDescending(instance => instance.Version)
            .First();
        MSBuildLocator.RegisterInstance(instance);
        InitializeComponent();
    }
}
