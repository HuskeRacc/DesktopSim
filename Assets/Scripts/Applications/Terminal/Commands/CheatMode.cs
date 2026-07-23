using UnityEngine;

public class CheatMode : ITerminalCommand
{
    public string Name => "enablecheats";
    public string Description => "Enables all cheat commands";
    public bool IsHidden { get; set; } = true;

    public void Execute(TerminalManager terminal, string args)
    {
        int unlockedCount = 0;

        foreach (var kvp in terminal.CommandRegistry)
        {
            if (kvp.Value is ITerminalCommand command && command.IsHidden)
            {
                command.IsHidden = false;
                unlockedCount++;
            }
        }
        terminal.PrintLine($"<color=#00FF00>[SYSTEM]: Cheat mode activated! Unlocked {unlockedCount} hidden commands.</color> \n<color=red>You must restart the game to disable cheats.</color>");
    }
}
