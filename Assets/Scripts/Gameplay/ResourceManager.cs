using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Currencies")]
    [SerializeField] private float currentUSD = 0.00f;
    [SerializeField] private float currentBTC = 0.000f;
    public event Action<float> OnUSDChanged;

    [Header("Power Settings")]
    public float basePowerDrawRate = 5f;
    public float powerPerMiningLevel = 2.5f;
    public float costPerKWh = 0.12f;
    public float unbilledPower = 0f;

    [Header("Network Settings")]
    public float pageLoadTime = 3.0f;

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
    }

    public bool SpendBTC(float amount)
    {
        if (currentBTC >= amount)
        {
            currentBTC -= amount;
            return true;
        }

        Debug.LogWarning("[ResourceManager] Not enough BTC");
        return false;
    }

    public float GetBTC() => currentBTC;
}