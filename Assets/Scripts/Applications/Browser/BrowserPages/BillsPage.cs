using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BillsPageUI : MonoBehaviour
{
    [Header("Electricity Row")]
    public GameObject electricityRow;
    public TextMeshProUGUI electricityPriceText;
    public TextMeshProUGUI electricityDescText; // NEW: Drag the "%Power%" text here
    public Button electricityPayButton;

    [Header("ISP Row")]
    public GameObject ispRow;
    public TextMeshProUGUI ispPriceText;
    public Button ispPayButton;

    [Header("Fine Row")]
    public GameObject fineRow;
    public TextMeshProUGUI finePriceText;
    public Button finePayButton;

    [Header("Status UI")]
    public GameObject noBillsBanner;

    private void Start()
    {
        electricityPayButton.onClick.AddListener(PayElectricity);
        ispPayButton.onClick.AddListener(PayISP);
        finePayButton.onClick.AddListener(PayFine);
    }

    private void OnEnable()
    {
        RefreshUI(); 
        if (BillingManager.Instance != null)
            BillingManager.Instance.OnBillsChanged += RefreshUI;
    }

    private void OnDisable()
    {
        if (BillingManager.Instance != null)
            BillingManager.Instance.OnBillsChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        if (BillingManager.Instance == null) return;

        // 1. Check if ANY bill is currently active
        bool hasAnyBill = BillingManager.Instance.hasElectricityBill || 
                          BillingManager.Instance.hasISPBill || 
                          BillingManager.Instance.hasFine;

        // 2. Toggle the "No Bills to Pay" banner
        if (noBillsBanner != null)
        {
            // Set active if hasAnyBill is false
            noBillsBanner.SetActive(!hasAnyBill); 
        }

        // Toggle rows
        electricityRow.SetActive(BillingManager.Instance.hasElectricityBill);
        ispRow.SetActive(BillingManager.Instance.hasISPBill);
        fineRow.SetActive(BillingManager.Instance.hasFine);

        // Update Prices with F2 for clean money formatting
        electricityPriceText.text = $"Price: ${BillingManager.Instance.electricityCost.ToString("F2")}";
        ispPriceText.text = $"Price: ${BillingManager.Instance.ispCost.ToString("F2")}";
        finePriceText.text = $"Price: ${BillingManager.Instance.fineCost.ToString("F2")}";

        // Update Dynamic Description
        electricityDescText.text = $"Power used: {BillingManager.Instance.billedPowerAmount.ToString("F2")} kWh";
    }

    private void PayElectricity()
    {
        if (ResourceManager.Instance.GetUSD() >= BillingManager.Instance.electricityCost)
        {
            ResourceManager.Instance.SpendUSD(BillingManager.Instance.electricityCost);
            BillingManager.Instance.hasElectricityBill = false;
            BillingManager.Instance.billedPowerAmount = 0f;
            RefreshUI();
        }
    }

    private void PayISP()
    {
        if (ResourceManager.Instance.GetUSD() >= BillingManager.Instance.ispCost)
        {
            ResourceManager.Instance.SpendUSD(BillingManager.Instance.ispCost);
            BillingManager.Instance.hasISPBill = false;

            BillingManager.Instance.ispCost = 0f; // Reset ISP cost after payment

            RefreshUI();
        }
    }

    private void PayFine()
    {
        if (ResourceManager.Instance.GetUSD() >= BillingManager.Instance.fineCost)
        {
            ResourceManager.Instance.SpendUSD(BillingManager.Instance.fineCost);
            BillingManager.Instance.hasFine = false;
            BillingManager.Instance.fineCost = 0f; 
            RefreshUI();
        }
    }
}