using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    // The Singleton instance
    public static UnlockManager Instance { get; private set; }

    [Header("Unlockable UI Panels")]
    public GameObject speedMonitorUI;
    // You can add more things here later, like: public GameObject darkWebApp;

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
        // 1. Hide the unlockables immediately when the game boots up
        if (speedMonitorUI != null) speedMonitorUI.SetActive(false);
    }

    // 2. The Browser will call this method and pass in a text ID
    public void UnlockApp(string appID)
    {
        if (appID == "SpeedMonitor" && speedMonitorUI != null)
        {
            speedMonitorUI.SetActive(true);
        }
    }
}