using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// 1. The Upgrade Types
public enum UpgradeType
{
    InternetSpeed,
    MiningSpeed,
    UIUnlock
}

// 2. The Data Structure 
[System.Serializable]
public class UpgradeItem
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
    
[Tooltip("Type the exact ID to unlock (e.g., 'SpeedMonitor')")]
    public string unlockID;

    [Header("UI References")]
    public Button upgradeButton;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;

    // Hidden state variables tracked by the script
    [HideInInspector] public int currentLevel = 0;
    [HideInInspector] public float currentCost;
}

// 3. The Page Manager
public class UpgradeManager : MonoBehaviour
{
    [Header("Store Inventory")]
    public List<UpgradeItem> availableUpgrades = new List<UpgradeItem>();

    private void Start()
    {
        // Loop through the list and set up every upgrade button automatically
        for (int i = 0; i < availableUpgrades.Count; i++)
        {
            int index = i; 
            var item = availableUpgrades[i];
            
            item.currentCost = item.baseCost;

            if (item.upgradeButton != null)
            {
                item.upgradeButton.onClick.AddListener(() => AttemptUpgrade(index));
            }

            UpdateUpgradeUI(item);
        }
    }

    private void Update()
    {
        if (ResourceManager.Instance == null) return;
        
        float playerWallet = ResourceManager.Instance.GetUSD();

        // Constantly check if the player can afford each upgrade in the list
        foreach (var item in availableUpgrades)
        {
            if (item.upgradeButton == null) continue;

            if (item.maxLevel > 0 && item.currentLevel >= item.maxLevel)
            {
                item.upgradeButton.interactable = false;
            }
            else
            {
                item.upgradeButton.interactable = playerWallet >= item.currentCost;
            }
        }
    }

    private void AttemptUpgrade(int index)
    {
        var item = availableUpgrades[index];

        if (ResourceManager.Instance.GetUSD() >= item.currentCost)
        {
            if (ResourceManager.Instance.SpendUSD(item.currentCost))
            {
                // We now pass the whole 'item' instead of just the type and amount
                ApplyUpgradeEffect(item); 

                item.currentLevel++;
                item.currentCost *= item.costMultiplier;
                
                UpdateUpgradeUI(item);
            }
        }
    }

    private void ApplyUpgradeEffect(UpgradeItem item)
    {
        switch (item.upgradeType)
        {
            case UpgradeType.InternetSpeed:
                if (BrowserManager.Instance != null)
                    BrowserManager.Instance.UpgradeInternetSpeed(item.effectAmount);
                break;

            case UpgradeType.MiningSpeed:
                if (MiningManager.Instance != null)
                    MiningManager.Instance.UpgradeMiningRate(item.effectAmount);
                break;

            case UpgradeType.UIUnlock:
                if (UnlockManager.Instance != null)
                {
                    // Pass the string ID to the bridge!
                    UnlockManager.Instance.UnlockApp(item.unlockID);
                }
                break;
        }
    }

    private void UpdateUpgradeUI(UpgradeItem item)
    {
        if (item.levelText != null) 
            item.levelText.text = $"Lvl: {item.currentLevel}";

        if (item.costText != null)
        {
            if (item.maxLevel > 0 && item.currentLevel >= item.maxLevel)
            {
                // Displays "PURCHASED" for apps, and "MAXED OUT" for stats
                item.costText.text = item.upgradeType == UpgradeType.UIUnlock ? "PURCHASED" : "MAXED OUT";
            }
            else
            {
                item.costText.text = $"Cost: ${item.currentCost.ToString("F2")}";
            }
        }
    }
}