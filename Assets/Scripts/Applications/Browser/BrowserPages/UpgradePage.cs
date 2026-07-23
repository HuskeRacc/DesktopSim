using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class UpgradeUIRow
{
    [Tooltip("Drag your new Scriptable Object data file here!")]
    public UpgradeData data; 
    
    [Header("UI References")]
    public Button upgradeButton;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;
}

public class UpgradePage : MonoBehaviour
{
    [Header("Store Inventory UI")]
    public List<UpgradeUIRow> availableUpgrades = new List<UpgradeUIRow>();

    private void Start()
    {
        // Hook up the buttons to send the Data to the PlayerUpgradeManager
        foreach (var row in availableUpgrades)
        {
            if (row.upgradeButton != null && row.data != null)
            {
                UpgradeData dataRef = row.data; 
                row.upgradeButton.onClick.AddListener(() => PlayerUpgradeManager.Instance.AttemptPurchase(dataRef));
            }
        }
    }

    private void OnEnable()
    {
        RefreshAllUI();
        
        // Listen for events to update the UI efficiently
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnUSDChanged += CheckAffordability;
            
        if (PlayerUpgradeManager.Instance != null)
            PlayerUpgradeManager.Instance.OnUpgradePurchased += RefreshAllUI;
    }

    private void OnDisable()
    {
        // Always unsubscribe to prevent errors when the window closes
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnUSDChanged -= CheckAffordability;

        if (PlayerUpgradeManager.Instance != null)
            PlayerUpgradeManager.Instance.OnUpgradePurchased -= RefreshAllUI;
    }

    private void RefreshAllUI()
    {
        if (PlayerUpgradeManager.Instance == null) return;
        
        float currentWallet = ResourceManager.Instance != null ? ResourceManager.Instance.GetUSD() : 0f;

        foreach (var row in availableUpgrades)
        {
            if (row.data == null) continue;

            // Ask the backend for the live numbers
            int currentLevel = PlayerUpgradeManager.Instance.GetUpgradeLevel(row.data.upgradeType);
            float currentCost = PlayerUpgradeManager.Instance.GetCurrentCost(row.data);

            if (row.levelText != null) 
                row.levelText.text = $"Lvl: {currentLevel}";

            if (row.costText != null)
            {
                if (row.data.maxLevel > 0 && currentLevel >= row.data.maxLevel)
                {
                    row.costText.text = row.data.upgradeType == UpgradeType.UIUnlock ? "PURCHASED" : "MAXED OUT";
                }
                else
                {
                    row.costText.text = $"Cost: ${currentCost.ToString("F2")}";
                }
            }
        }
        
        // Refresh button states
        CheckAffordability(currentWallet);
    }

    private void CheckAffordability(float playerWallet)
    {
        if (PlayerUpgradeManager.Instance == null) return;

        foreach (var row in availableUpgrades)
        {
            if (row.upgradeButton == null || row.data == null) continue;

            int currentLevel = PlayerUpgradeManager.Instance.GetUpgradeLevel(row.data.upgradeType);
            float currentCost = PlayerUpgradeManager.Instance.GetCurrentCost(row.data);

            if (row.data.maxLevel > 0 && currentLevel >= row.data.maxLevel)
            {
                row.upgradeButton.interactable = false;
            }
            else
            {
                row.upgradeButton.interactable = playerWallet >= currentCost;
            }
        }
    }
}