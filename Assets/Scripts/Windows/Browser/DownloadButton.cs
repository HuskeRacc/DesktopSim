using UnityEngine;
using UnityEngine.UI;

public class DownloadButton : MonoBehaviour
{
    [SerializeField] private AppType appToDownload;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnDownloadClicked);
    }

    private void OnDownloadClicked()
    {
        if (IconManager.Instance != null)
        {
            IconManager.Instance.InstallApp(appToDownload);
            
            // Optional: Disable the button or change its text to "Downloaded!" here
            button.interactable = false; 
        }
    }
}