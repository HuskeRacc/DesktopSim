public class RajiiCommand : ITerminalCommand
{
    public string Name => "rajii";
    public string Description => "Displays secret image";
    public bool IsHidden { get; set; } = true;

    public void Execute(TerminalController terminal, string args)
    {
        terminal.PrintLine("<color=#00FFFF>[SYSTEM]: Displaying classified file...</color>");

        if (WindowManager.Instance != null)
        {
            WindowManager.Instance.OpenPopup("rajii");
        }
    }
}