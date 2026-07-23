public class EchoCommand : ITerminalCommand
{
    public string Name => "echo";
    public string Description => "Prints text back to the terminal";
    public bool IsHidden { get; set; } = false;
    public void Execute(TerminalManager terminal, string args)
    {
        terminal.PrintLine(args);
    }
}