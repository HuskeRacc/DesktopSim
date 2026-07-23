using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BTCMinerWindow : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI rateText;
    [SerializeField] private TextMeshProUGUI minedText;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button toggleButton;

    private void Start()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(OnToggleClicked);
        }
    }

    private void Update()
    {
        // Constantly refresh the UI to match the background manager's state
        if (MiningManager.Instance != null)
        {
            // Display the rate
            rateText.text = $"{ResourceManager.FormatHashrate(ResourceManager.Instance.miningRatePerSecond)}";
            
            // Display the total mined by this app
            minedText.text = $"{ResourceManager.Instance.totalMinedThisSession.ToString("F6")}";

            // Update button text depending on if it's currently running
            if (buttonText != null)
            {
                buttonText.text = MiningManager.Instance.isMining ? "Stop Mining" : "Start Mining";
            }
        }
    }

    private void OnToggleClicked()
    {
        if (MiningManager.Instance != null)
        {
            MiningManager.Instance.ToggleMining();
        }
    }
}