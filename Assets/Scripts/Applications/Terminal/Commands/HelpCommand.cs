using System.Linq;

public class HelpCommand : ITerminalCommand
{
    public string Name => "help";
    public string Description => "Shows all available commands";

    public bool IsHidden { get; set; } = false;

    public void Execute(TerminalManager terminal, string args)
    {
        terminal.PrintLine("Available commands:");

        // Filter out hidden commands and sort them alphabetically by name
        var sortedCommands = terminal.CommandRegistry.Values
            .Where(cmd => !cmd.IsHidden)
            .OrderBy(cmd => cmd.Name);

        foreach (var cmd in sortedCommands)
        {
            terminal.PrintLine($"{cmd.Name} - {cmd.Description}");
        }
    }
}