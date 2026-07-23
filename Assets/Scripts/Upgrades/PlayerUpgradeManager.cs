using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
    public static PlayerUpgradeManager Instance;

    // This Dictionary is the ONLY thing you will ever need to save/load!
    // It maps the UpgradeType directly to the Player's Level.
    private Dictionary<UpgradeType, int> upgradeLevels = new Dictionary<UpgradeType, int>();

    // This event tells the UI to refresh when an upgrade is bought
    public event Action OnUpgradePurchased;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 1. Check a level
    public int GetUpgradeLevel(UpgradeType type)
    {
        if (upgradeLevels.TryGetValue(type, out int level))
        {
            return level;
        }
        return 0; // If they don't own it yet, the level is 0
    }

    // 2. Dynamically calculate the cost using exponential math
    public float GetCurrentCost(UpgradeData data)
    {
        int currentLevel = GetUpgradeLevel(data.upgradeType);
        // Formula: BaseCost * (Multiplier ^ CurrentLevel)
        return data.baseCost * Mathf.Pow(data.costMultiplier, currentLevel);
    }

    // 3. Attempt to buy it
    public void AttemptPurchase(UpgradeData data)
    {
        float cost = GetCurrentCost(data);

        if (ResourceManager.Instance != null && ResourceManager.Instance.SpendUSD(cost))
        {
            // Increase the level in our dictionary
            int newLevel = GetUpgradeLevel(data.upgradeType) + 1;
            upgradeLevels[data.upgradeType] = newLevel;

            // Apply the actual stat change
            ApplyUpgradeEffect(data);

            // Tell the UI to update
            OnUpgradePurchased?.Invoke();
        }
    }

    // 4. Send the stats to the rest of the game
    private void ApplyUpgradeEffect(UpgradeData data)
    {
        switch (data.upgradeType)
        {
            case UpgradeType.InternetSpeed:
                if (ResourceManager.Instance != null)
                    ResourceManager.Instance.UpgradeInternetSpeed(data.effectAmount);
                break;

            case UpgradeType.MiningSpeed:
                if (ResourceManager.Instance != null)
                    ResourceManager.Instance.UpgradeMiningRate(data.effectAmount);
                break;

            case UpgradeType.UIUnlock:
                if (UnlockManager.Instance != null && data.appToUnlock != UnlockableApp.None)
                {
                    UnlockManager.Instance.UnlockApp(data.appToUnlock);
                }
                break;
        }
    }
}