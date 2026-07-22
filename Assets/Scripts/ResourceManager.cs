using UnityEngine;

// Notice we no longer need 'using TMPro;' up here!
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Currencies")]
    [SerializeField] private float currentUSD = 0.00f;
    [SerializeField] private float currentBTC = 0.000f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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