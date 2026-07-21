using UnityEngine;

public class MiningManager : MonoBehaviour
{
    public static MiningManager Instance { get; private set; }

    [Header("Mining Stats")]
    public float miningRatePerSecond = 0.005f; // Starting speed
    public bool isMining { get; private set; } = false;
    public float totalMinedThisSession { get; private set; } = 0f;

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
            // Calculate how much BTC was mined in this exact frame
            float minedThisFrame = miningRatePerSecond * Time.deltaTime;
            
            // Add to our visual tracker
            totalMinedThisSession += minedThisFrame;
            
            // Deposit it directly into the bank!
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.AddBitcoin(minedThisFrame);
            }
        }
    }

    public void ToggleMining()
    {
        isMining = !isMining;
    }

    // Your browser upgrade script will call this method!
    public void UpgradeMiningRate(float amountToAdd)
    {
        miningRatePerSecond += amountToAdd;
        Debug.Log($"[MiningManager] Rate upgraded! New rate: {miningRatePerSecond}/sec");
    }
}