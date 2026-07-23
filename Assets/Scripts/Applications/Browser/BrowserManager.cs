using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BrowserManager : MonoBehaviour
{
    // Singleton instance so other scripts can access it globally
    public static BrowserManager Instance { get; private set; }

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

    [Header("Internet Settings")]
    [SerializeField] private Image loadingProgressBar;

    [Header("Websites")]
    [SerializeField] private string homeUrl = "www.os.home";
    [SerializeField] private WebPage[] availablePages;

    // History & Loading tracking
    private List<string> history = new List<string>();
    private int historyIndex = -1;
    private Coroutine currentLoadRoutine;

    private void Awake()
    {
        // Standard Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (loadingProgressBar != null) loadingProgressBar.gameObject.SetActive(false);
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

        StartLoadingPage(targetUrl);
    }

    public void GoBack()
    {
        if (historyIndex > 0)
        {
            historyIndex--;
            StartLoadingPage(history[historyIndex]);
        }
    }

    public void GoForward()
    {
        if (historyIndex < history.Count - 1)
        {
            historyIndex++;
            StartLoadingPage(history[historyIndex]);
        }
    }

    public void GoHome()
    {
        if (historyIndex >= 0 && history[historyIndex] == homeUrl) return;
        
        LoadNewPage(homeUrl);
    }

    private void StartLoadingPage(string targetUrl)
    {
        // Stop any current loading if the player rapidly clicks links
        if (currentLoadRoutine != null)
        {
            StopCoroutine(currentLoadRoutine);
        }

        // Update the text box immediately
        if (urlInputField != null) urlInputField.text = targetUrl;

        // Check if the target is the local home page (or any other local system app)
        if (targetUrl.Equals(homeUrl, System.StringComparison.OrdinalIgnoreCase))
        {
            // Hide the progress bar just in case we interrupted an external page load
            if (loadingProgressBar != null)
            {
                loadingProgressBar.gameObject.SetActive(false);
            }
            
            // Instantly snap the local page into place!
            DisplayPage(targetUrl);
        }
        else
        {
            // It's an external internet page, start the loading delay
            currentLoadRoutine = StartCoroutine(PageLoadRoutine(targetUrl));
        }
    }

    private IEnumerator PageLoadRoutine(string targetUrl)
    {
        if (loadingProgressBar != null)
        {
            loadingProgressBar.fillAmount = 0f;
            loadingProgressBar.gameObject.SetActive(true);
        }

        float elapsedTime = 0f;
        
        // Pull the live speed from the persistent manager
        float currentLoadTime = ResourceManager.Instance != null ? ResourceManager.Instance.pageLoadTime : 3.0f;

        while (elapsedTime < currentLoadTime)
        {
            elapsedTime += Time.deltaTime;
            
            if (loadingProgressBar != null)
            {
                // Use the new variable for the fill math
                loadingProgressBar.fillAmount = elapsedTime / currentLoadTime;
            }
            
            yield return null; 
        }

        if (loadingProgressBar != null)
        {
            loadingProgressBar.gameObject.SetActive(false);
        }

        DisplayPage(targetUrl);
    }

    private void DisplayPage(string targetUrl)
    {
        bool pageFound = false;

        foreach (var page in availablePages)
        {
            if (page.url.Equals(targetUrl, System.StringComparison.OrdinalIgnoreCase))
            {
                page.pagePanel.SetActive(true);
                pageFound = true;
            }
            else
            {
                page.pagePanel.SetActive(false);
            }
        }

        if (!pageFound)
        {
            Debug.LogWarning($"[Browser] 404 - Page not found: {targetUrl}");
        }

        UpdateNavigationButtons();
    }

    private void UpdateNavigationButtons()
    {
        if (backButton != null) backButton.interactable = historyIndex > 0;
        if (forwardButton != null) forwardButton.interactable = historyIndex < history.Count - 1;
    }
}