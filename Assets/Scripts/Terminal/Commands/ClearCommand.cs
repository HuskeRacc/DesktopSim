public class ClearCommand : ITerminalCommand
{
    public string Name => "clear";
    public string Description => "Clears the terminal screen";

    public bool IsHidden { get; set; } = false;
    public void Execute(TerminalController terminal, string args)
    {
        terminal.ClearScreen();
    }
}