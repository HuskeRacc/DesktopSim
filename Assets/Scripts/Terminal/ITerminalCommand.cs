public interface ITerminalCommand
{
    string Name { get; }
    string Description { get; }
    bool IsHidden { get; set; }

    void Execute(TerminalController terminal, string args);
}