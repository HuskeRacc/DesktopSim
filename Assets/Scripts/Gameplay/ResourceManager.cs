using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Currencies")]
    [SerializeField] private float currentUSD = 0.00f;
    [SerializeField] private float currentBTC = 0.000f;

    [Header("Economy & Market")]
    public float btcBuyRate = 1050.00f; 
    public float btcSellRate = 950.00f;

    [Tooltip("The total USD value of the player's currently held BTC (calculated at sell rate)")]
    public float currentBTCValueInUSD { get; private set; }

    [Header("Power Settings")]
    public float basePowerDrawRate = 5f;
    public float powerPerMiningLevel = 2.5f;
    public float costPerKWh = 0.12f;

    [Header("Billing")]
    public float unbilledPower = 0f;

    [Header("Mining Data")]
    public float miningRatePerSecond = 0.005f;
    public float totalMinedThisSession = 0f;

    [Header("Network Settings")]
    public float pageLoadTime = 3.0f;

    public event Action<float> OnUSDChanged;
    public event Action<float> OnBTCChanged;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        float currentPowerDraw = basePowerDrawRate;

        // Ask the backend PlayerUpgradeManager for the level, not the UI!
        if (PlayerUpgradeManager.Instance != null)
        {
            int miningLevel = PlayerUpgradeManager.Instance.GetUpgradeLevel(UpgradeType.MiningSpeed);
            currentPowerDraw += (miningLevel * powerPerMiningLevel);
        }

        // Continuously accumulate power usage over time
        unbilledPower += currentPowerDraw * Time.deltaTime;
    }

    // -- MINE METHODS --

    public void AddMinedTotal(float amount)
    {
        totalMinedThisSession += amount;
    }

    public void UpgradeMiningRate(float amountToAdd)
    {
        miningRatePerSecond += amountToAdd;
        Debug.Log($"[ResourceManager] Mining rate upgraded! New rate: {miningRatePerSecond}/sec");
    }

    public static string FormatHashrate(float hashesPerSecond)
    {
        // The big leagues
        if (hashesPerSecond >= 1000000f)
            return $"{hashesPerSecond / 1000000f:F2} MH/s"; // Megahash
            
        if (hashesPerSecond >= 1000f)
            return $"{hashesPerSecond / 1000f:F2} kH/s"; // Kilohash
            
        // Standard baseline
        if (hashesPerSecond >= 1f)
            return $"{hashesPerSecond:F2} H/s"; // Hash
            
        // The micro-realm (Idle Game Magic!)
        if (hashesPerSecond >= 0.001f)
            return $"{hashesPerSecond * 1000f:F1} mH/s"; // Millihash
            
        return $"{hashesPerSecond * 1000000f:F0} uH/s"; // Microhash
    }

    // -- BILL METHODS --

    public float CalculateAndResetPowerBill(out float powerUsed)
    {
        powerUsed = unbilledPower;
        float totalCost = unbilledPower * costPerKWh;

        unbilledPower = 0f; // Reset after billing
        return totalCost;
    }

    // -- NETWORK METHODS --
    public float GetSpeedScoreMBps()
    {
        return Mathf.Round(10f / pageLoadTime * 10f) / 10f;
    }

    public void UpgradeInternetSpeed(float amountToSubtract)
    {
        pageLoadTime = Mathf.Max(0.1f, pageLoadTime - amountToSubtract);
        Debug.Log($"[OS Network] Internet Upgraded! New load time: {pageLoadTime}s");
    }

    // -- USD METHODS --

    public void AddUSD(float amount)
    {
        currentUSD += amount;

        OnUSDChanged?.Invoke(currentUSD);
    }

    public bool SpendUSD(float amount)
    {
        if (currentUSD >= amount)
        {
            currentUSD -= amount;

            OnUSDChanged?.Invoke(currentUSD);
            
            return true;
        }

        Debug.LogWarning("[ResourceManager] Not enough USD");
        return false;
    }

    public float GetUSD() => currentUSD;

    // -- BTC METHODS --

    public void AddBitcoin(float amount)
    {
        currentBTC += amount;
        UpdateBTCValueInUSD();
        OnBTCChanged?.Invoke(currentBTC);
    }

    public bool SpendBitcoin(float amount)
    {
        if (currentBTC >= amount)
        {
            currentBTC -= amount;
            UpdateBTCValueInUSD();
            OnBTCChanged?.Invoke(currentBTC);
            return true;
        }
        return false;
    }

    public float GetBTC() => currentBTC;

    private void UpdateBTCValueInUSD()
    {
        currentBTCValueInUSD = currentBTC * btcSellRate;
    }
}