using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IconManager : MonoBehaviour
{
    [Header("Icon Prefabs")]
    [SerializeField] private GameObject desktopIconPrefab;
    [SerializeField] private GameObject taskbarIconPrefab;

    [Header("Icon Positions")]
    [SerializeField] private GameObject[] desktopIconPositions;
    [SerializeField] private GameObject[] taskbarIconPositions;

    [Header("Available Apps")]
    [SerializeField] private AppData[] availableApps;

    [Header("Window Panels")]
    [SerializeField] private WindowController explorerWindow;

    private GameObject activeExplorerTaskbarIcon;

    private void Start()
    {
        Initialise();
        
        // Listen for when the window closes so we can destroy the taskbar icon
        if (explorerWindow != null)
        {
            explorerWindow.OnWindowClosed += OnExplorerClosed;
        }
    }

    private void OnDestroy()
    {
        if (explorerWindow != null)
        {
            explorerWindow.OnWindowClosed -= OnExplorerClosed;
        }
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
        if (explorerWindow == null) return;

        explorerWindow.OpenWindow();

        // Spawn taskbar icon if it isn't already there
        if (activeExplorerTaskbarIcon == null && taskbarIconPositions.Length > 0)
        {
            Transform slot = taskbarIconPositions[0].transform;
            activeExplorerTaskbarIcon = Instantiate(taskbarIconPrefab, slot.position, Quaternion.identity, slot);

            // Clicking taskbar icon toggles the window
            Button btn = activeExplorerTaskbarIcon.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(explorerWindow.ToggleWindow);
            }
        }
    }

    private void OnExplorerClosed()
    {
        if (activeExplorerTaskbarIcon != null)
        {
            Destroy(activeExplorerTaskbarIcon);
            activeExplorerTaskbarIcon = null;
        }
    }
}