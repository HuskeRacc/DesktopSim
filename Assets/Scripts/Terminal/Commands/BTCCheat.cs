using UnityEngine;

public class BTCCheat : ITerminalCommand
{
    public string Name => "c_btc";
    public string Description => "Adds 1 BTC to your wallet";
    public bool IsHidden { get; set; } = true;

    public void Execute(TerminalController terminal, string args)
    {
       if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddBitcoin(1f);
            terminal.PrintLine("<color=#00FFFF>[SYSTEM]: Adding 1 BTC!</color>");
        }
        else
        {
            terminal.PrintLine("<color=#FF0000>[ERROR]: ResourceManager instance not found.</color>");
        }
    }
}
