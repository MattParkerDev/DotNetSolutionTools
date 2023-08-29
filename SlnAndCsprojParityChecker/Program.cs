using SlnAndCsprojParityChecker.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
    config.SetApplicationName("SlnAndCsprojParityChecker");
    config.ValidateExamples();

    config.AddCommand<CompareCommand>("compare");
});

return await app.RunAsync(args);
