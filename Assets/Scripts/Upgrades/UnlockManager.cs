using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    public static UnlockManager Instance { get; private set; }

    // Creates a neat little box in the Inspector to pair an Enum with a UI Panel
    [System.Serializable]
    public struct AppUnlockMapping
    {
        public UnlockableApp appType;
        public GameObject uiPanel;
    }

    [Header("Unlockable Apps Inventory")]
    public List<AppUnlockMapping> unlockableApps = new List<AppUnlockMapping>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Instantly hide all mapped panels on boot[cite: 5]
        foreach (var mapping in unlockableApps)
        {
            if (mapping.uiPanel != null) 
                mapping.uiPanel.SetActive(false);
        }
    }

    // The Browser now passes the Enum instead of a string
    public void UnlockApp(UnlockableApp targetApp)
    {
        // Search our list for the matching app and turn it on
        foreach (var mapping in unlockableApps)
        {
            if (mapping.appType == targetApp && mapping.uiPanel != null)
            {
                mapping.uiPanel.SetActive(true);
                Debug.Log($"[OS] Unlocked App: {targetApp}");
                return; // Stop searching once we find it
            }
        }
    }
}