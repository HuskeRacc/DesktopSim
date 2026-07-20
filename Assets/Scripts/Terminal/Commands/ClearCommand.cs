public class ClearCommand : ITerminalCommand
{
    public string Name => "clear";
    public string Description => "Clears the terminal screen";

    public void Execute(TerminalController terminal, string args)
    {
        terminal.ClearScreen();
    }
}