using UnityEngine;

public class MiningManager : MonoBehaviour
{
    public static MiningManager Instance { get; private set; }

    [Header("Mining Stats")]
    public float miningRatePerSecond = 0.005f; // Speed
    
    [Tooltip("How often in seconds the BTC actually gets deposited/updated")]
    public float updateInterval = 1.0f; 
    
    public bool isMining { get; private set; } = false;
    public float totalMinedThisSession { get; private set; } = 0f;

    private float timer = 0f;

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
        if (isMining)
        {
            timer += Time.deltaTime;

            // Wait until our timer reaches the specified interval (e.g., 1 second)
            if (timer >= updateInterval)
            {
                // Calculate the chunk of BTC mined during this specific interval
                float minedThisTick = miningRatePerSecond * updateInterval;
                
                // Add to our visual tracker
                totalMinedThisSession += minedThisTick;
                
                // Deposit it directly into the bank!
                if (ResourceManager.Instance != null)
                {
                    ResourceManager.Instance.AddBitcoin(minedThisTick);
                }

                // Subtract the interval instead of setting to 0 to prevent time drift (lag loss)
                timer -= updateInterval; 
            }
        }
    }

    public void ToggleMining()
    {
        isMining = !isMining;
        
        // Reset the timer when stopped so the next start is a fresh countdown
        if (!isMining) 
        {
            timer = 0f;
        }
    }

    public void UpgradeMiningRate(float amountToAdd)
    {
        miningRatePerSecond += amountToAdd;
        Debug.Log($"[MiningManager] Rate upgraded! New rate: {miningRatePerSecond}/sec");
    }
}