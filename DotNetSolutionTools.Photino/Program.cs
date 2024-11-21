using System.Text.Json;
using DotNetSolutionTools.Photino.Models;
using Microsoft.Build.Locator;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Photino.Blazor;

namespace DotNetSolutionTools.Photino;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

        appBuilder.Services.AddLogging();
        appBuilder.Services.AddMudServices();
        appBuilder.Services.AddSingleton<AppState>();

        appBuilder.RootComponents.Add<App>("app");

        var app = appBuilder.Build();

        app.MainWindow.SetSize(1400, 900)
            .SetDevToolsEnabled(true)
            .SetLogVerbosity(0)
            //.SetIconFile("favicon.ico")
            .SetTitle("DotNetSolutionTools.Photino");

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        var instance = MSBuildLocator
            .QueryVisualStudioInstances()
            .OrderByDescending(instance => instance.Version)
            .First();
        MSBuildLocator.RegisterInstance(instance);

        var configFilePath = GetConfigFilePath();

        using var scope = app.Services.CreateScope();
        var appState = scope.ServiceProvider.GetRequiredService<AppState>();

        LoadAppStateFromConfigFile(appState, configFilePath);

        app.MainWindow.RegisterWindowClosingHandler(
            (sender, eventArgs) =>
            {
                using var stream = File.Create(configFilePath);
                JsonSerializer.Serialize(stream, appState);
                stream.Flush();
                return false;
            }
        );

        app.Run();
    }

    private static string GetConfigFilePath()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var configFolder = Path.Combine(folder, "DotNetSolutionTools.Photino");
        Directory.CreateDirectory(configFolder);
        var configFilePath = Path.Combine(configFolder, "config.json");
        return configFilePath;
    }

    private static void LoadAppStateFromConfigFile(AppState appState, string configFilePath)
    {
        if (File.Exists(configFilePath) is false)
        {
            File.WriteAllText(configFilePath, string.Empty);
        }

        using var stream = File.OpenRead(configFilePath);
        if (stream.Length is 0)
        {
            return;
        }
        var deserializedAppState = JsonSerializer.Deserialize<AppState>(stream);
        if (deserializedAppState is not null)
        {
            appState.SolutionFolderPath = deserializedAppState.SolutionFolderPath;
            appState.SolutionFilePath = deserializedAppState.SolutionFilePath;
            appState.CsprojFilePath = deserializedAppState.CsprojFilePath;
        }
    }
}
