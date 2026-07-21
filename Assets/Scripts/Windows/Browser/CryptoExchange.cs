using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CryptoExchange : MonoBehaviour
{
    [Header("Market Settings")]
    public float btcToUsdRate = 50.00f; // 1 BTC = $50.00

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
        buySlider.onValueChanged.AddListener(UpdateBuyUI);
        sellSlider.onValueChanged.AddListener(UpdateSellUI);

        // Hook up the buttons
        buyButton.onClick.AddListener(ExecuteBuy);
        sellButton.onClick.AddListener(ExecuteSell);
    }

    private void UpdateExchangeRateUI()
    {
        if (exchangeRateText != null)
        {
            exchangeRateText.text = $"1BTC = ${btcToUsdRate.ToString("F2")}";
        }
    }

    // --- BUY LOGIC ---

    private void UpdateBuyUI(float sliderValue)
    {
        if (ResourceManager.Instance == null) return;

        // sliderValue is between 0 and 1 (0% to 100% of their USD)
        float maxUsd = ResourceManager.Instance.GetUSD();
        float usdToSpend = maxUsd * sliderValue;
        float btcToGet = usdToSpend / btcToUsdRate;

        // Show both the cost and the amount of BTC they will get
        buyText.text = $"Cost: ${usdToSpend.ToString("F2")}\nGet: {btcToGet.ToString("F4")} BTC";
        
        // Only allow clicking "Buy" if they are actually spending money
        buyButton.interactable = usdToSpend > 0;
    }

    private void ExecuteBuy()
    {
        float maxUsd = ResourceManager.Instance.GetUSD();
        float usdToSpend = maxUsd * buySlider.value;
        float btcToGet = usdToSpend / btcToUsdRate;

        if (usdToSpend > 0 && ResourceManager.Instance.SpendUSD(usdToSpend))
        {
            ResourceManager.Instance.AddBitcoin(btcToGet);
            
            // Reset the slider after a successful purchase
            buySlider.value = 0; 
            UpdateSellUI(sellSlider.value); // Update the other side just in case
        }
    }

    // --- SELL LOGIC ---

    private void UpdateSellUI(float sliderValue)
    {
        if (ResourceManager.Instance == null) return;

        // sliderValue is between 0 and 1 (0% to 100% of their BTC)
        float maxBtc = ResourceManager.Instance.GetBTC();
        float btcToSell = maxBtc * sliderValue;
        float usdToGet = btcToSell * btcToUsdRate;

        // Show both the BTC being sold and the USD they will get
        sellText.text = $"Sell: {btcToSell.ToString("F4")} BTC\nGet: ${usdToGet.ToString("F2")}";
        
        // Only allow clicking "Sell" if they are actually selling something
        sellButton.interactable = btcToSell > 0;
    }

    private void ExecuteSell()
    {
        float maxBtc = ResourceManager.Instance.GetBTC();
        float btcToSell = maxBtc * sellSlider.value;
        float usdToGet = btcToSell * btcToUsdRate;

        if (btcToSell > 0 && ResourceManager.Instance.SpendBTC(btcToSell))
        {
            ResourceManager.Instance.AddUSD(usdToGet);
            
            // Reset the slider after a successful sale
            sellSlider.value = 0; 
            UpdateBuyUI(buySlider.value); // Update the other side just in case
        }
    }
}