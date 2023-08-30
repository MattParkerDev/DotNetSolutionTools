using System.ComponentModel;
using Spectre.Console.Cli;

namespace SolutionParityChecker.CLI.Commands;

public class FormatCsprojCommand : Command<FormatCsprojCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(1, "<CsprojFilePath>")]
        public required string CsprojFilePath { get; set; }
        
        [CommandOption("-a|--enable-all")]
        [Description("true to enable logging of all project files. Default is false.")]
        [DefaultValue(false)]
        public bool EnableAll { get; set; } = false;
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var pathToCsprojFile = settings.CsprojFilePath;
        Console.WriteLine($"Retrieving C# Project from {pathToCsprojFile}");
        FormatCsproj.FormatCsprojFile(pathToCsprojFile);
        return 0;
    }
}
