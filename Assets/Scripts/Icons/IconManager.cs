using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class IconManager : MonoBehaviour
{
    // Simple mapping struct so you can pair AppTypes to Window Prefabs in the Inspector
    [System.Serializable]
    public struct AppWindowConfig
    {
        public AppType appType;
        public GameObject windowPrefab;
    }

    [Header("Icon Prefabs")]
    [SerializeField] private GameObject desktopIconPrefab; //[cite: 7]
    [SerializeField] private GameObject taskbarIconPrefab; //[cite: 7]

    [Header("UI Canvas & App Configurations")]
    [SerializeField] private Transform canvasTransform; //[cite: 7]
    [SerializeField] private AppWindowConfig[] appWindowConfigs; // Link each AppType to its specific Window Prefab here!

    [Header("Icon Positions")]
    [SerializeField] private GameObject[] desktopIconPositions; //[cite: 7]
    [SerializeField] private GameObject[] taskbarIconPositions; //[cite: 7]

    [Header("Available Apps")]
    [SerializeField] private AppData[] availableApps; //[cite: 7]

    // Track active runtime instances mapped by their AppType
    private Dictionary<AppType, WindowController> activeWindows = new Dictionary<AppType, WindowController>();
    private Dictionary<AppType, GameObject> activeTaskbarIcons = new Dictionary<AppType, GameObject>();

    private void Start()
    {
        Initialise(); //[cite: 7]
    }

    private void Initialise()
    {
        for (int i = 0; i < availableApps.Length; i++)
        {
            if (i >= desktopIconPositions.Length) break; //[cite: 7]

            GameObject spawnedIcon = Instantiate(desktopIconPrefab, desktopIconPositions[i].transform.position, Quaternion.identity, desktopIconPositions[i].transform); //[cite: 7]

            DesktopIconItem iconItem = spawnedIcon.GetComponent<DesktopIconItem>(); //[cite: 7]
            if (iconItem != null)
            {
                iconItem.SetupIcon(availableApps[i], OnIconClicked); //[cite: 7]
            }

            Image iconImage = spawnedIcon.GetComponentInChildren<Image>(); //[cite: 7]
            if (iconImage != null) iconImage.sprite = availableApps[i].iconSprite; //[cite: 7]

            TextMeshProUGUI appText = spawnedIcon.GetComponentInChildren<TextMeshProUGUI>(); //[cite: 7]
            if (appText != null) appText.text = availableApps[i].appName; //[cite: 7]
        }
    }

    private void OnIconClicked(AppType clickedApp)
    {
        OpenAppWindow(clickedApp);
    }

    private void OpenAppWindow(AppType appType)
    {
        // 1. If the window already exists in memory, bring it to front and ensure it's open[cite: 7]
        if (activeWindows.TryGetValue(appType, out WindowController existingWindow) && existingWindow != null)
        {
            existingWindow.OpenWindow(); //[cite: 5, 7]
            existingWindow.transform.SetAsLastSibling(); // Focus window on top of UI[cite: 7]
            return;
        }

        // 2. Locate the prefab configured for this specific AppType
        GameObject prefabToSpawn = GetPrefabForApp(appType);
        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"No window prefab configured in IconManager for app type: {appType}");
            return;
        }

        // 3. Instantiate and setup the window[cite: 7]
        GameObject windowInstance = Instantiate(prefabToSpawn, canvasTransform); //[cite: 7]
        windowInstance.transform.localScale = Vector3.one;

        WindowController windowController = windowInstance.GetComponent<WindowController>(); //[cite: 7]
        if (windowController != null)
        {
            activeWindows[appType] = windowController;

            // Subscribe to close event using a closure to pass the specific appType[cite: 7]
            windowController.OnWindowClosed += () => OnWindowClosed(appType);
            windowController.OpenWindow(); //[cite: 5]
        }

        // 4. Spawn taskbar icon in the next available taskbar slot[cite: 7]
        int currentTaskbarIndex = activeTaskbarIcons.Count;
        if (currentTaskbarIndex < taskbarIconPositions.Length) //[cite: 7]
        {
            Transform slot = taskbarIconPositions[currentTaskbarIndex].transform; //[cite: 7]
            GameObject taskbarIcon = Instantiate(taskbarIconPrefab, slot.position, Quaternion.identity, slot); //[cite: 7]

            // Apply app sprite to taskbar icon if present
            Image img = taskbarIcon.GetComponentInChildren<Image>();
            AppData appData = System.Array.Find(availableApps, a => a.appType == appType);
            if (img != null && appData.iconSprite != null)
            {
                img.sprite = appData.iconSprite;
            }

            // Taskbar icon click toggles window visibility[cite: 7]
            Button btn = taskbarIcon.GetComponent<Button>(); //[cite: 7]
            if (btn != null && windowController != null) //[cite: 7]
            {
                btn.onClick.AddListener(windowController.ToggleWindow); //[cite: 5, 7]
            }

            activeTaskbarIcons[appType] = taskbarIcon;
        }
    }

    private void OnWindowClosed(AppType appType)
    {
        // Destroy taskbar icon associated with this closed app[cite: 7]
        if (activeTaskbarIcons.TryGetValue(appType, out GameObject taskbarIcon))
        {
            if (taskbarIcon != null) Destroy(taskbarIcon); //[cite: 7]
            activeTaskbarIcons.Remove(appType);
        }

        // Remove window tracking reference[cite: 7]
        if (activeWindows.ContainsKey(appType))
        {
            activeWindows.Remove(appType);
        }
    }

    private GameObject GetPrefabForApp(AppType appType)
    {
        foreach (var config in appWindowConfigs)
        {
            if (config.appType == appType) return config.windowPrefab;
        }
        return null;
    }
}