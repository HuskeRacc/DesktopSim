using UnityEngine;
using System;

public class BillingManager : MonoBehaviour
{
    public static BillingManager Instance;
    public event Action OnBillsChanged;

    [Header("Billing Cycle")]
    public int hoursBetweenBills = 4;
    public int gracePeriodHours = 4;

    private int hoursPassed = 0;
    private int totalHoursPlayed = 0;
    private bool isGracePeriodOver = false;

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
        // if grace period is not over
        if (!isGracePeriodOver)
        {
            totalHoursPlayed++;
            
            if (totalHoursPlayed >= gracePeriodHours)
            {
                isGracePeriodOver = true;
                hoursPassed = 0; // Reset the standard cycle tracker
                
                //Issue the very first bill immediately when the grace period ends?
                IssueElectricityBill();
                IssueISPBill();
            }
            

            Debug.Log("Bills Grace Period hour: " + totalHoursPlayed  + " / " + gracePeriodHours);
            return; // Exit out so the standard cycle doesn't tick yet
        }

        //After Grace period ends
        hoursPassed++;
        
        if (hoursPassed >= hoursBetweenBills)
        {
            hoursPassed = 0; 
            Debug.Log("Bill(s) issued.");
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