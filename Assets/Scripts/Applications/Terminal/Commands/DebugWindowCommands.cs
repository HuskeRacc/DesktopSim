public class DebugWindowCommands : ITerminalCommand
{
    public string Name => "debug";
    public string Description => "Opens Debug Menu";
    public bool IsHidden { get; set; } = true;

    public void Execute(TerminalManager terminal, string args)
    {
        terminal.PrintLine("<color=red>[SYSTEM]: Displaying debug menu...</color>");

        if (WindowManager.Instance != null)
        {
            WindowManager.Instance.OpenPopup("debug");
        }
    }
}
