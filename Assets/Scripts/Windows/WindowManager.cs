using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowManager : MonoBehaviour
{
    public static WindowManager Instance { get; private set; }

    [System.Serializable]
    public struct AppWindowConfig
    {
        public AppType appType;
        public GameObject windowPrefab;
    }

    [Header("UI Canvas & App Configurations")]
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private AppWindowConfig[] appWindowConfigs;

    [Header("Taskbar Config")]
    [SerializeField] private GameObject taskbarIconPrefab;
    [SerializeField] private GameObject[] taskbarIconPositions;
    [SerializeField] private AppData[] availableApps;

    // Track active runtime instances mapped by AppType
    private Dictionary<AppType, WindowController> activeWindows = new Dictionary<AppType, WindowController>();
    private Dictionary<AppType, GameObject> activeTaskbarIcons = new Dictionary<AppType, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OpenAppWindow(AppType appType)
    {
        // 1. If window exists in memory, bring it to front
        if (activeWindows.TryGetValue(appType, out WindowController existingWindow) && existingWindow != null)
        {
            existingWindow.OpenWindow();
            existingWindow.transform.SetAsLastSibling();
            return;
        }

        // 2. Locate prefab for app type
        GameObject prefabToSpawn = GetPrefabForApp(appType);
        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"No window prefab configured in WindowManager for app type: {appType}");
            return;
        }

        // 3. Instantiate window
        GameObject windowInstance = Instantiate(prefabToSpawn, canvasTransform);
        windowInstance.transform.localScale = Vector3.one;

        WindowController windowController = windowInstance.GetComponent<WindowController>();
        if (windowController != null)
        {
            activeWindows[appType] = windowController;
            windowController.OnWindowClosed += () => OnWindowClosed(appType);
            windowController.OpenWindow();
        }

        // 4. Spawn taskbar icon
        int currentTaskbarIndex = activeTaskbarIcons.Count;
        if (currentTaskbarIndex < taskbarIconPositions.Length)
        {
            Transform slot = taskbarIconPositions[currentTaskbarIndex].transform;
            GameObject taskbarIcon = Instantiate(taskbarIconPrefab, slot.position, Quaternion.identity, slot);

            Image img = taskbarIcon.GetComponentInChildren<Image>();
            AppData appData = System.Array.Find(availableApps, a => a.appType == appType);
            if (img != null && appData.iconSprite != null)
            {
                img.sprite = appData.iconSprite;
            }

            Button btn = taskbarIcon.GetComponent<Button>();
            if (btn != null && windowController != null)
            {
                btn.onClick.AddListener(windowController.ToggleWindow);
            }

            activeTaskbarIcons[appType] = taskbarIcon;
        }
    }

    public void OpenCustomWindow(GameObject windowPrefab)
{
    if (windowPrefab == null)
    {
        Debug.LogWarning("[WindowManager] Custom window prefab is null!");
        return;
    }

    GameObject windowInstance = Instantiate(windowPrefab, canvasTransform);
    windowInstance.transform.localScale = Vector3.one;
    windowInstance.transform.SetAsLastSibling(); // Bring to front

    WindowController windowController = windowInstance.GetComponent<WindowController>();
    if (windowController != null)
    {
        windowController.OpenWindow();
    }
}

    private void OnWindowClosed(AppType appType)
    {
        if (activeTaskbarIcons.TryGetValue(appType, out GameObject taskbarIcon))
        {
            if (taskbarIcon != null) Destroy(taskbarIcon);
            activeTaskbarIcons.Remove(appType);
        }

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