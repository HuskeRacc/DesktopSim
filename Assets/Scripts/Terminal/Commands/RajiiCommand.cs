using UnityEngine;

public class RajiiCommand : ITerminalCommand
{
    public string Name => "rajii";
    public string Description => "Displays secret image";

    public void Execute(TerminalController terminal, string args)
    {
        terminal.PrintLine("<color=#00FFFF>[SYSTEM]: Displaying classified file...</color>");

        // Option A: Load from Assets/Resources/Prefabs/RajiiWindow.prefab
        GameObject rajiiPrefab = Resources.Load<GameObject>("Prefabs/RajiiWindow");

        if (rajiiPrefab != null && WindowManager.Instance != null)
        {
            WindowManager.Instance.OpenCustomWindow(rajiiPrefab);
        }
        else
        {
            terminal.PrintLine("<color=#FF0000>Error: File corrupted or missing.</color>");
        }
    }
}