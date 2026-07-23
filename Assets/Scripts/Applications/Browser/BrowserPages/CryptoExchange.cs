using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CryptoExchange : MonoBehaviour
{
    [Header("Global UI")]
    [SerializeField] private TextMeshProUGUI exchangeRateText;

    [Header("Buy BTC Panel")]
    [SerializeField] private Slider buySlider;
    [SerializeField] private TextMeshProUGUI buyText; // The $0.00 text under the buy slider
    [SerializeField] private Button buyButton;

    [Header("Sell BTC Panel")]
    [SerializeField] private Slider sellSlider;
    [SerializeField] private TextMeshProUGUI sellText; // The $0.00 text under the sell slider
    [SerializeField] private Button sellButton;

    [Header("Tracking Variables")]
    private float lastKnownBtc = -1f;
    private float lastKnownUsd = -1f;

    private void OnEnable()
    {
        // Reset sliders to 0 whenever the player opens this page
        if (buySlider != null) buySlider.value = 0;
        if (sellSlider != null) sellSlider.value = 0;
        
        UpdateExchangeRateUI();
        UpdateBuyUI(0);
        UpdateSellUI(0);
    }

    private void Start()
    {
        // Tell the sliders to run our update methods whenever they are moved
        if (buySlider != null) buySlider.onValueChanged.AddListener(UpdateBuyUI);
        if (sellSlider != null) sellSlider.onValueChanged.AddListener(UpdateSellUI);

        // Hook up the buttons
        if (buyButton != null) buyButton.onClick.AddListener(ExecuteBuy);
        if (sellButton != null) sellButton.onClick.AddListener(ExecuteSell);
    }

    private void Update()
    {
        if (ResourceManager.Instance == null) return;

        float currentBtc = ResourceManager.Instance.GetBTC();
        float currentUsd = ResourceManager.Instance.GetUSD();

        // If the player's wallet balance changes (like from background mining), 
        // force the UI to recalculate based on the new totals!
        if (currentBtc != lastKnownBtc || currentUsd != lastKnownUsd)
        {
            lastKnownBtc = currentBtc;
            lastKnownUsd = currentUsd;

            // Recalculate using whatever the sliders are currently set to
            UpdateBuyUI(buySlider.value);
            UpdateSellUI(sellSlider.value);
        }
    }

    private void UpdateExchangeRateUI()
    {
        if (ResourceManager.Instance != null && exchangeRateText != null)
        {
            float buyRate = ResourceManager.Instance.btcBuyRate;
            float sellRate = ResourceManager.Instance.btcSellRate;

            exchangeRateText.text = $"Buy: ${buyRate.ToString("F2")} | Sell: ${sellRate.ToString("F2")}";
        }
    }

    // --- BUY LOGIC ---

    private void UpdateBuyUI(float sliderValue)
    {
        if (ResourceManager.Instance == null) return;

        float maxUsd = ResourceManager.Instance.GetUSD();
        float usdToSpend = maxUsd * sliderValue;
        float buyRate = ResourceManager.Instance.btcBuyRate;
        float btcToGet = buyRate > 0 ? usdToSpend / buyRate : 0;

        buyText.text = $"Cost: ${usdToSpend.ToString("F2")}\nGet: {btcToGet.ToString("F4")} BTC";
        
        buyButton.interactable = usdToSpend > 0;
    }

    private void ExecuteBuy()
    {
        if (ResourceManager.Instance == null) return;

        float maxUsd = ResourceManager.Instance.GetUSD();
        float usdToSpend = maxUsd * buySlider.value;
        float buyRate = ResourceManager.Instance.btcBuyRate;
        float btcToGet = buyRate > 0 ? usdToSpend / buyRate : 0;

        if (usdToSpend > 0 && ResourceManager.Instance.SpendUSD(usdToSpend))
        {
            ResourceManager.Instance.AddBitcoin(btcToGet);
            
            buySlider.value = 0; 
            UpdateSellUI(sellSlider.value); 
        }
    }

    // --- SELL LOGIC ---

    private void UpdateSellUI(float sliderValue)
    {
        if (ResourceManager.Instance == null) return;

        float maxBtc = ResourceManager.Instance.GetBTC();
        float btcToSell = maxBtc * sliderValue;
        float sellRate = ResourceManager.Instance.btcSellRate;
        float usdToGet = btcToSell * sellRate;

        sellText.text = $"Sell: {btcToSell.ToString("F4")} BTC\nGet: ${usdToGet.ToString("F2")}";
        
        sellButton.interactable = btcToSell > 0;
    }

    private void ExecuteSell()
    {
        if (ResourceManager.Instance == null) return;

        float maxBtc = ResourceManager.Instance.GetBTC();
        float btcToSell = maxBtc * sellSlider.value;
        float sellRate = ResourceManager.Instance.btcSellRate;
        float usdToGet = btcToSell * sellRate;

        if (btcToSell > 0 && ResourceManager.Instance.SpendBitcoin(btcToSell))
        {
            ResourceManager.Instance.AddUSD(usdToGet);
            
            sellSlider.value = 0; 
            UpdateBuyUI(buySlider.value); 
        }
    }
}