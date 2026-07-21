using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Currencies")]
    [SerializeField] private float currentUSD = 0.00f;
    [SerializeField] private float currentBTC = 0.000f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI usdText;
    [SerializeField] private TextMeshProUGUI btcText;

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
        UpdateUI();
    }

    // -- USD METHODS --
    public bool SpendUSD(float amount)
    {
        if (currentUSD >= amount)
        {
            currentUSD -= amount;
            UpdateUI();
            return true;
        }

        Debug.LogWarning("[ResourceManager] Not enough USD");
        return false;
    }

    public float GetUSD() => currentUSD;

    // -- BTC METHODS --
    public bool SpendBTC(float amount)
    {
        if (currentBTC >= amount)
        {
            currentBTC -= amount;
            UpdateUI();
            return true;
        }

        Debug.LogWarning("[ResourceManager] Not enough BTC");
        return false;
    }

    public float GetBTC() => currentBTC;

    private void UpdateUI()
    {
        if (usdText != null)
            usdText.text = $"{currentUSD:F2}";

        if (btcText != null)
            btcText.text = $"{currentBTC:F3}";
    }
}
