using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DownloadButton : MonoBehaviour
{
    [SerializeField] private AppType appToDownload;
    [SerializeField] private TextMeshProUGUI buttonText; // Reference to the text on the button
    
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnDownloadClicked);

        // Auto-grab the text component if you forget to assign it
        if (buttonText == null) 
        {
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        // When the page opens, check if we already own this app
        if (IconManager.Instance != null)
        {
            AppData appData = IconManager.Instance.GetAppData(appToDownload);
            if (appData.isInstalled)
            {
                SetToPurchasedState();
            }
        }
    }

    private void OnDownloadClicked()
    {
        if (IconManager.Instance != null)
        {
            // Try to buy/install. Only change the UI if it returns true!
            bool wasSuccessful = IconManager.Instance.InstallApp(appToDownload);
            
            if (wasSuccessful)
            {
                SetToPurchasedState();
            }
        }
    }

    private void SetToPurchasedState()
    {
        button.interactable = false; // Grays out the button and stops clicks
        
        if (buttonText != null)
        {
            buttonText.text = "Purchased";
        }
    }
}