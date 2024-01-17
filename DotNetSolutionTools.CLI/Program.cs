using DotNetSolutionTools.CLI.Commands;
using Microsoft.Build.Locator;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
    config.SetApplicationName("SolutionParityChecker");
    config.ValidateExamples();

    config.AddCommand<CompareCommand>("compare");
    config.AddCommand<ImplicitUsingsCommand>("implicit-usings");
    config.AddCommand<FormatCsprojCommand>("format-csproj");
    config.AddCommand<TreatWarningsAsErrorsCommand>("warnings-as-errors");
});

var instance = MSBuildLocator.QueryVisualStudioInstances().OrderByDescending(instance => instance.Version).First();
MSBuildLocator.RegisterInstance(instance);

return await app.RunAsync(args);
