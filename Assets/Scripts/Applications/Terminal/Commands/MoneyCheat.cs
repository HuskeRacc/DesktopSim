public class MoneyCheat : ITerminalCommand
{
    public string Name => "c_money";
    public string Description => "Adds 1000 USD to your wallet";
    public bool IsHidden { get; set; } = true;

    public void Execute(TerminalManager terminal, string args)
    {
       if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddUSD(1000f);
            terminal.PrintLine("<color=#00FFFF>[SYSTEM]: Adding Money!</color>");
        }
        else
        {
            terminal.PrintLine("<color=#FF0000>[ERROR]: ResourceManager instance not found.</color>");
        }
    }
}
