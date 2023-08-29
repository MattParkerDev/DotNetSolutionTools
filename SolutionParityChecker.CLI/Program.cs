using SolutionParityChecker.CLI.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
    config.SetApplicationName("SolutionParityChecker");
    config.ValidateExamples();

    config.AddCommand<CompareCommand>("compare");
});

return await app.RunAsync(args);
