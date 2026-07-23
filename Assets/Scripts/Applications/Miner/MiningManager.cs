using UnityEngine;

public class MiningManager : MonoBehaviour
{
    public static MiningManager Instance { get; private set; }

    [Header("Mining Settings")]
    [Tooltip("How often in seconds the BTC actually gets deposited/updated")]
    public float updateInterval = 1.0f; 
    
    public bool isMining { get; private set; } = false;
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
        if (isMining && ResourceManager.Instance != null)
        {
            timer += Time.deltaTime;

            // Wait until our timer reaches the specified interval (e.g., 1 second)
            if (timer >= updateInterval)
            {
                // Grab the live rate dynamically from ResourceManager!
                float currentRate = ResourceManager.Instance.miningRatePerSecond;
                
                // Calculate the chunk of BTC mined during this specific interval
                float minedThisTick = currentRate * updateInterval;
                
                // Send data to ResourceManager
                ResourceManager.Instance.AddMinedTotal(minedThisTick);
                ResourceManager.Instance.AddBitcoin(minedThisTick);

                // Subtract the interval instead of setting to 0 to prevent time drift
                timer -= updateInterval; 
            }
        }
    }

    public void ToggleMining()
    {
        isMining = !isMining;
        
        if (!isMining) 
        {
            timer = 0f;
        }
    }
}