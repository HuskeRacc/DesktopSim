public class Rajii2Command : ITerminalCommand
{
    public string Name => "rajii2";
    public string Description => "Displays secret image";
    public bool IsHidden { get; set; } = true;

    public void Execute(TerminalController terminal, string args)
    {
        terminal.PrintLine("<color=#00FFFF>[SYSTEM]: Displaying classified file...</color>");

        if (WindowManager.Instance != null)
        {
            WindowManager.Instance.OpenPopup("rajii2");
        }
    }
}