using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BrowserManager : MonoBehaviour
{
    [System.Serializable]
    public struct WebPage
    {
        public string url;
        public GameObject pagePanel;
    }

    [Header("Browser UI")]
    [SerializeField] private TMP_InputField urlInputField;
    [SerializeField] private Button backButton;
    [SerializeField] private Button forwardButton;

    [Header("Websites")]
    [SerializeField] private string homeUrl = "www.os.home";
    [SerializeField] private WebPage[] availablePages;

    // History tracking
    private List<string> history = new List<string>();
    private int historyIndex = -1;

    private void Start()
    {
        LoadNewPage(homeUrl);
    }

    public void LoadNewPage(string targetUrl)
    {
        if (historyIndex < history.Count - 1)
        {
            history.RemoveRange(historyIndex + 1, history.Count - (historyIndex + 1));
        }

        history.Add(targetUrl);
        historyIndex++;

        DisplayPage(targetUrl);
    }

    public void GoBack()
    {
        if (historyIndex > 0)
        {
            historyIndex--;
            DisplayPage(history[historyIndex]);
        }
    }

    public void GoForward()
    {
        if (historyIndex < history.Count - 1)
        {
            historyIndex++;
            DisplayPage(history[historyIndex]);
        }
    }

    public void GoHome()
    {
        if (historyIndex >= 0 && history[historyIndex] == homeUrl) return;
        
        LoadNewPage(homeUrl);
    }

    private void DisplayPage(string targetUrl)
    {
        bool pageFound = false;

        foreach (var page in availablePages)
        {
            if (page.url.Equals(targetUrl, System.StringComparison.OrdinalIgnoreCase))
            {
                page.pagePanel.SetActive(true);
                if (urlInputField != null) urlInputField.text = page.url;
                pageFound = true;
            }
            else
            {
                // Hide all other pages
                page.pagePanel.SetActive(false);
            }
        }

        if (!pageFound)
        {
            Debug.LogWarning($"[Browser] 404 - Page not found: {targetUrl}");
        }

        UpdateNavigationButtons();
    }

    // Automatically disables the back/forward buttons when at the end of the history
    private void UpdateNavigationButtons()
    {
        if (backButton != null) backButton.interactable = historyIndex > 0;
        if (forwardButton != null) forwardButton.interactable = historyIndex < history.Count - 1;
    }
}
 