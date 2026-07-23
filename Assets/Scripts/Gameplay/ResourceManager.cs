using UnityEngine;

// Notice we no longer need 'using TMPro;' up here!
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Currencies")]
    [SerializeField] private float currentUSD = 0.00f;
    [SerializeField] private float currentBTC = 0.000f;

    [Header("Power Settings")]
    public float basePowerDrawRate = 5f;
    public float powerPerMiningLevel = 2.5f;
    public float costPerKWh = 0.12f;

    public float unbilledPower = 0f;

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

        // Automatically increase power draw if they have upgraded their mining rigs
        if (UpgradeManager.Instance != null)
        {
            int miningLevel = UpgradeManager.Instance.GetUpgradeLevel(UpgradeType.MiningSpeed);
            currentPowerDraw += (miningLevel * powerPerMiningLevel);
        }

        // Continuously accumulate power usage over time
        unbilledPower += currentPowerDraw * Time.deltaTime;
    }

    public float CalculateAndResetPowerBill(out float powerUsed)
    {
        powerUsed = unbilledPower;
        float totalCost = unbilledPower * costPerKWh;

        unbilledPower = 0f; // Reset after billing
        return totalCost;
    }

    // -- USD METHODS --

    public void AddUSD(float amount)
    {
        currentUSD += amount;
    }

    public bool SpendUSD(float amount)
    {
        if (currentUSD >= amount)
        {
            currentUSD -= amount;
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