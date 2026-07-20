public class HelpCommand : ITerminalCommand
{
    public string Name => "help";
    public string Description => "Shows all available commands";

    public void Execute(TerminalController terminal, string args)
    {
        terminal.PrintLine("Available commands:");
        foreach (var cmd in terminal.CommandRegistry.Values)
        {
            if (cmd.IsHidden) continue; // Skip secret commands!

            terminal.PrintLine($"{cmd.Name} - {cmd.Description}");
        }
    }
}