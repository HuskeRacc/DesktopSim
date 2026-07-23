using UnityEngine;

// 1. Keep the enum global so all scripts know what an upgrade is
public enum UpgradeType
{
    InternetSpeed,
    MiningSpeed,
    UIUnlock
}

public enum UnlockableApp
{
    None,
    SpeedMonitor,
    DarkWebApp // Placeholder
}

// 2. Add this attribute to add a custom option to Unity's Right-Click menu!
[CreateAssetMenu(fileName = "New Upgrade", menuName = "OS/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    [Header("Identity")]
    public string upgradeName;
    public UpgradeType upgradeType;

    [Header("Cost & Scaling")]
    public float baseCost = 50.00f;
    public float costMultiplier = 1.5f;
    public int maxLevel = 10; 

    [Header("Effect")]
    public float effectAmount = 0.5f;
    
    [Header("Unlock Settings")]
    [Tooltip("Select the app to unlock from the dropdown!")]
    public UnlockableApp appToUnlock;
}