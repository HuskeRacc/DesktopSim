using System.Collections.Generic;
using UnityEngine;

// Add any new stats you want to track to this list!
public enum DebugStat
{
    USD_Balance,
    BTC_Balance,
    Unbilled_Power,
    Mining_Rate,
    Net_Speed,
    Time_Paused
}

public class DebugPanelController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The text prefab with the DebugLineUI script attached")]
    public GameObject infoLinePrefab;
    
    [Tooltip("The object holding the Grid Layout Group")]
    public Transform gridContainer;

    [Header("Tracked Stats")]
    [Tooltip("Add stats here to instantly spawn them in the debug panel!")]
    public List<DebugStat> statsToWatch = new List<DebugStat>();

    // Tracks the spawned UI elements so we can update them every frame
    private Dictionary<DebugStat, DebugInfoLine> spawnedLines = new Dictionary<DebugStat, DebugInfoLine>();

    private void Start()
    {
        // 1. Clear out any placeholder text you left in the editor for layout testing
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Spawn a new text prefab for every stat you added to the list in the Inspector
        foreach (DebugStat stat in statsToWatch)
        {
            // Prevent spawning duplicates if you accidentally add the same stat twice
            if (spawnedLines.ContainsKey(stat)) continue; 

            GameObject newLine = Instantiate(infoLinePrefab, gridContainer);
            DebugInfoLine lineUI = newLine.GetComponent<DebugInfoLine>();
            
            if (lineUI != null)
            {
                spawnedLines.Add(stat, lineUI);
            }
        }
    }

    private void Update()
    {
        // 3. Constantly push live data to the spawned UI text
        foreach (var kvp in spawnedLines)
        {
            DebugStat stat = kvp.Key;
            DebugInfoLine ui = kvp.Value;

            // Format the enum name nicely (Replaces underscores with spaces)
            string label = stat.ToString().Replace("_", " "); 
            string value = GetStatValue(stat);

            ui.SetText(label, value);
        }
    }

    // 4. The Data Router: This pulls from your game's actual Singletons
    private string GetStatValue(DebugStat stat)
    {
        switch (stat)
        {
            case DebugStat.USD_Balance:
                return ResourceManager.Instance != null ? ResourceManager.Instance.GetUSD().ToString("F2") : "N/A";
            
            case DebugStat.BTC_Balance:
                return ResourceManager.Instance != null ? ResourceManager.Instance.GetBTC().ToString("F5") : "N/A";
            
            case DebugStat.Unbilled_Power:
                return ResourceManager.Instance != null ? ResourceManager.Instance.unbilledPower.ToString("F2") : "N/A";

            case DebugStat.Mining_Rate:
                return MiningManager.Instance != null ? MiningManager.Instance.miningRatePerSecond.ToString("F4") : "N/A";

            case DebugStat.Net_Speed:
                return ResourceManager.Instance != null ? ResourceManager.Instance.GetSpeedScoreMBps().ToString("F1") : "N/A";

            case DebugStat.Time_Paused:
                return TimeManager.Instance != null ? TimeManager.Instance.isTimePaused.ToString() : "N/A";

            default:
                return "Unknown Stat";
        }
    }
}