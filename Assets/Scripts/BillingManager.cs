using UnityEngine;
using System;

public class BillingManager : MonoBehaviour
{
    public static BillingManager Instance;
    public event Action OnBillsChanged;

    [Header("Billing Cycle")]
    public int hoursBetweenBills = 4;
    private int hoursePassed = 0;

    [Header("Electricity")]
    public bool hasElectricityBill = false;
    public float electricityCost = 0f;
    public float billedPowerAmount = 0f;
    

    [Header("ISP")]
    public bool hasISPBill = false;
    public float ispCost = 0f;
    public float baseISPFee = 15f;
    public float costPerInternetTier = 12.5f;

    [Header("Fines")]
    public bool hasFine = false;
    public float fineCost = 0f;

    private void OnEnable()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnHourPassed += HandleHourPassed;
        }
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnHourPassed -= HandleHourPassed;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void HandleHourPassed()
    {
        hoursePassed++;
        
        if (hoursePassed >= hoursBetweenBills)
        {
            hoursePassed = 0;

            IssueElectricityBill();
            IssueISPBill();
        }
    }

    public void IssueFineTest() => IssueFine(50f);

    public void IssueFine(float amount)
    {
        hasFine = true;
        fineCost += amount;
        OnBillsChanged?.Invoke(); 
    }

    public void IssueElectricityBill()
    {
        if (ResourceManager.Instance != null)
        {
            // Pull the unbilled power and cost, then add it to any existing unpaid bill
            float cost = ResourceManager.Instance.CalculateAndResetPowerBill(out float power);
            
            billedPowerAmount += power;
            electricityCost += cost;

            hasElectricityBill = true;
            OnBillsChanged?.Invoke(); 
        }
    }

    public void IssueISPBill()
    {
        int currentInternetLevel = 0;

        if (UpgradeManager.Instance != null)
        {
          currentInternetLevel = UpgradeManager.Instance.GetUpgradeLevel(UpgradeType.InternetSpeed);
        }

        float cycleCharge = baseISPFee + (currentInternetLevel * costPerInternetTier);

        ispCost += cycleCharge;
        hasISPBill = true;

        OnBillsChanged?.Invoke();
    }
}