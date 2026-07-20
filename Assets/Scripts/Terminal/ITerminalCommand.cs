public interface ITerminalCommand
{
    string Name { get; }
    string Description { get; }
    bool IsHidden => false;
    void Execute(TerminalController terminal, string args);
}