public interface ITerminalCommand
{
    string Name { get; }
    string Description { get; }
    void Execute(TerminalController terminal, string args);
}