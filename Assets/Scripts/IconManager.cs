using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IconManager : MonoBehaviour
{
    [Header("Icon Prefabs")]
    [SerializeField] private GameObject desktopIconPrefab;
    [SerializeField] private GameObject taskbarIconPrefab;

    [Header("Window Prefabs")]
    [SerializeField] private GameObject explorerWindowPrefab; // Drag your Window Prefab here
    [SerializeField] private Transform canvasTransform;       // Target Canvas to parent windows under

    [Header("Icon Positions")]
    [SerializeField] private GameObject[] desktopIconPositions;
    [SerializeField] private GameObject[] taskbarIconPositions;

    [Header("Available Apps")]
    [SerializeField] private AppData[] availableApps;

    // Track active runtime instances
    private WindowController activeExplorerWindow;
    private GameObject activeExplorerTaskbarIcon;

    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        for (int i = 0; i < availableApps.Length; i++)
        {
            if (i >= desktopIconPositions.Length) break;

            GameObject spawnedIcon = Instantiate(desktopIconPrefab, desktopIconPositions[i].transform.position, Quaternion.identity, desktopIconPositions[i].transform);

            DesktopIconItem iconItem = spawnedIcon.GetComponent<DesktopIconItem>();
            if (iconItem != null)
            {
                iconItem.SetupIcon(availableApps[i], OnIconClicked);
            }

            Image iconImage = spawnedIcon.GetComponentInChildren<Image>();
            if (iconImage != null) iconImage.sprite = availableApps[i].iconSprite;

            TextMeshProUGUI appText = spawnedIcon.GetComponentInChildren<TextMeshProUGUI>();
            if (appText != null) appText.text = availableApps[i].appName;
        }
    }

    private void OnIconClicked(AppType clickedApp)
    {
        switch (clickedApp)
        {
            case AppType.Explorer:
                OpenExplorer();
                break;

            case AppType.Settings:
                Debug.Log("Opening Settings Window...");
                break;

            case AppType.Browser:
                Debug.Log("Opening Browser Window...");
                break;
        }
    }

    private void OpenExplorer()
    {
        // 1. If window doesn't exist yet, instantiate it from prefab
        if (activeExplorerWindow == null)
        {
            if (explorerWindowPrefab == null) return;

            GameObject windowInstance = Instantiate(explorerWindowPrefab, canvasTransform);
            activeExplorerWindow = windowInstance.GetComponent<WindowController>();

            if (activeExplorerWindow != null)
            {
                // Subscribe to close event
                activeExplorerWindow.OnWindowClosed += OnExplorerClosed;
            }

            // 2. Spawn the taskbar icon
            if (activeExplorerTaskbarIcon == null && taskbarIconPositions.Length > 0)
            {
                Transform slot = taskbarIconPositions[0].transform;
                activeExplorerTaskbarIcon = Instantiate(taskbarIconPrefab, slot.position, Quaternion.identity, slot);

                // Taskbar icon click toggles the active window visibility
                Button btn = activeExplorerTaskbarIcon.GetComponent<Button>();
                if (btn != null && activeExplorerWindow != null)
                {
                    btn.onClick.AddListener(activeExplorerWindow.ToggleWindow);
                }
            }
        }
        else
        {
            // If window already exists, ensure it's visible/focused
            activeExplorerWindow.OpenWindow();
            activeExplorerWindow.transform.SetAsLastSibling(); // Brings to front in UI
        }
    }

    private void OnExplorerClosed()
    {
        // Clean up taskbar icon when the window is closed and destroyed
        if (activeExplorerTaskbarIcon != null)
        {
            Destroy(activeExplorerTaskbarIcon);
            activeExplorerTaskbarIcon = null;
        }

        if (activeExplorerWindow != null)
        {
            activeExplorerWindow.OnWindowClosed -= OnExplorerClosed;
            activeExplorerWindow = null;
        }
    }
}