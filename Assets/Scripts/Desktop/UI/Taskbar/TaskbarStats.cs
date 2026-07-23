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

    private string lastKnownTime = "";

    private void Update()
    {
        UpdateClock();
        UpdateSpeeds();
        UpdateWallet();
    }

    private void UpdateClock()
    {
        if (TimeManager.Instance != null && dateTimeText != null)
        {
            // Pull the fake game time from the manager
            string currentTime = TimeManager.Instance.GetFormattedTime();
            
            // Only update the text element if the minute has changed
            if (currentTime != lastKnownTime)
            {
                lastKnownTime = currentTime;
                dateTimeText.text = currentTime;
            }
        }
    }

    private void UpdateSpeeds()
{
    // 1. Internet Speed
    if (ResourceManager.Instance != null)
    {
        float currentNetSpeed = ResourceManager.Instance.GetSpeedScoreMBps(); 
        if (currentNetSpeed != lastKnownNetSpeed)
        {
            lastKnownNetSpeed = currentNetSpeed;
            if (netSpeedText != null) 
                netSpeedText.text = currentNetSpeed.ToString("F0") + " MB/s"; 
        }
    }

    // 2. Mining Speed
    if (ResourceManager.Instance != null)
    {
        float currentMineSpeed = ResourceManager.Instance.miningRatePerSecond; 
        if (currentMineSpeed != lastKnownMineSpeed)
        {
            lastKnownMineSpeed = currentMineSpeed;
            if (mineSpeedText != null) 
            {
                // Replaced .ToString("F4") with the clean hashrate formatter!
                mineSpeedText.text = ResourceManager.FormatHashrate(currentMineSpeed); 
            }
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
                if (usdText != null) usdText.text = $"{currentUsd:F2}"; // Formatting from ResourceManager
            }

            // 2. BTC
            float currentBtc = ResourceManager.Instance.GetBTC(); 
            if (currentBtc != lastKnownBtc)
            {
                lastKnownBtc = currentBtc;
                if (btcText != null) btcText.text = $"{currentBtc:F4}"; // Formatting from ResourceManager
            }
        }
    }
}