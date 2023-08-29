using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SolutionParityChecker.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void LoadSolutionFolder(object? sender, RoutedEventArgs e)
    {
        SolutionFolderPath.Text = "Solution folder path";
    }

    private void LoadSolutionFile(object? sender, RoutedEventArgs e)
    {
        SolutionFilePath.Text = "Solution file path";
    }
}
