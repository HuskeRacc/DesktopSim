using UnityEngine;
using TMPro;

public class TaskbarStats : MonoBehaviour
{
    [Header("Clock UI")]
    [SerializeField] private TextMeshProUGUI dateTimeText;

    [Header("Speed UI")]
    [SerializeField] private TextMeshProUGUI netSpeedText;
    [SerializeField] private TextMeshProUGUI mineSpeedText;

    [Header("Wallet UI")]
    [SerializeField] private TextMeshProUGUI usdText;
    [SerializeField] private TextMeshProUGUI btcText;

    // Tracking variables (Prevents Unity from redrawing text every frame unless it changes)
    private float lastKnownNetSpeed = -1f;
    private float lastKnownMineSpeed = -1f;
    private float lastKnownUsd = -1f;
    private float lastKnownBtc = -1f;

    private void Update()
    {
        UpdateClock();
        UpdateSpeeds();
        UpdateWallet();
    }

    private void UpdateClock()
    {
        if (dateTimeText != null)
        {
            // Pulls the exact formatting from your DateTime script[cite: 1]
            dateTimeText.text = System.DateTime.Now.ToString("HH:mm:ss\ndd-MM-yyyy");
        }
    }

    private void UpdateSpeeds()
    {
        // 1. Internet Speed
        if (BrowserManager.Instance != null)
        {
            float currentNetSpeed = BrowserManager.Instance.GetSpeedScoreMBps(); 
            if (currentNetSpeed != lastKnownNetSpeed)
            {
                lastKnownNetSpeed = currentNetSpeed;
                if (netSpeedText != null) netSpeedText.text = currentNetSpeed.ToString("F3"); 
            }
        }

        // 2. Mining Speed
        if (MiningManager.Instance != null)
        {
            float currentMineSpeed = MiningManager.Instance.miningRatePerSecond; 
            if (currentMineSpeed != lastKnownMineSpeed)
            {
                lastKnownMineSpeed = currentMineSpeed;
                if (mineSpeedText != null) mineSpeedText.text = currentMineSpeed.ToString("F4"); 
            }
        }
    }

    private void UpdateWallet()
    {
        if (ResourceManager.Instance != null)
        {
            // 1. USD 
            float currentUsd = ResourceManager.Instance.GetUSD();
            if (currentUsd != lastKnownUsd)
            {
                lastKnownUsd = currentUsd;
                if (usdText != null) usdText.text = $"{currentUsd:F3}"; // Formatting from ResourceManager[cite: 2]
            }

            // 2. BTC
            float currentBtc = ResourceManager.Instance.GetBTC(); 
            if (currentBtc != lastKnownBtc)
            {
                lastKnownBtc = currentBtc;
                if (btcText != null) btcText.text = $"{currentBtc:F5}"; // Formatting from ResourceManager[cite: 2]
            }
        }
    }
}