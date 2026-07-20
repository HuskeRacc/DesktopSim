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

    [System.Serializable]
    public struct PopupConfig
    {
        public string popupID; // e.g., "rajii"
        public GameObject popupPrefab;
        public Sprite popupIcon; // Optional: For taskbar representation
    }

    [Header("Popup Configurations")]
    [SerializeField] private List<PopupConfig> popupConfigs = new List<PopupConfig>();

    [Header("UI Canvas & App Configurations")]
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private AppWindowConfig[] appWindowConfigs;

    [Header("Taskbar Config")]
    [SerializeField] private GameObject taskbarIconPrefab;
    [SerializeField] private GameObject[] taskbarIconPositions;

    // Track active runtime instances
    private Dictionary<AppType, WindowController> activeWindows = new Dictionary<AppType, WindowController>();
    private Dictionary<AppType, GameObject> activeTaskbarIcons = new Dictionary<AppType, GameObject>();

    // Tracking active popups and their taskbar icons by ID
    private Dictionary<string, GameObject> activePopupTaskbarIcons = new Dictionary<string, GameObject>();

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
        Debug.LogWarning($"[WindowManager] No window prefab configured for AppType: {appType}");
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
    int currentTaskbarIndex = GetTotalActiveTaskbarCount();
    if (currentTaskbarIndex < taskbarIconPositions.Length)
    {
        Transform slot = taskbarIconPositions[currentTaskbarIndex].transform;
        GameObject taskbarIcon = Instantiate(taskbarIconPrefab, slot.position, Quaternion.identity, slot);

        // Fetch AppData directly from IconManager
        if (IconManager.Instance != null)
        {
            AppData appData = IconManager.Instance.GetAppData(appType);
            
            if (appData.iconSprite != null)
            {
                ApplySpriteToTaskbarIcon(taskbarIcon, appData.iconSprite);
            }
            else
            {
                Debug.LogWarning($"[WindowManager] Found AppData for {appType}, but its iconSprite is NULL! Check IconManager Inspector.");
            }
        }
        else
        {
            Debug.LogError("[WindowManager] IconManager.Instance is NULL!");
        }

        // Taskbar icon click toggles window visibility
        Button btn = taskbarIcon.GetComponent<Button>();
        if (btn != null && windowController != null)
        {
            btn.onClick.AddListener(windowController.ToggleWindow);
        }

        activeTaskbarIcons[appType] = taskbarIcon;
    }
}

    public void OpenPopup(string popupID)
    {
        foreach (var config in popupConfigs)
        {
            if (config.popupID.Equals(popupID, System.StringComparison.OrdinalIgnoreCase))
            {
                // If popup taskbar icon already exists, skip reopening
                if (activePopupTaskbarIcons.ContainsKey(popupID))
                {
                    return;
                }

                GameObject windowInstance = Instantiate(config.popupPrefab, canvasTransform);
                windowInstance.transform.localScale = Vector3.one;

                WindowController windowController = windowInstance.GetComponent<WindowController>();
                if (windowController != null)
                {
                    windowController.OnWindowClosed += () => OnPopupClosed(popupID);
                    windowController.OpenWindow();
                }

                // Spawn taskbar icon for popup
                if (taskbarIconPrefab != null && taskbarIconPositions.Length > 0)
                {
                    int currentTaskbarIndex = GetTotalActiveTaskbarCount();
                    if (currentTaskbarIndex < taskbarIconPositions.Length)
                    {
                        Transform slot = taskbarIconPositions[currentTaskbarIndex].transform;
                        GameObject taskbarIcon = Instantiate(taskbarIconPrefab, slot.position, Quaternion.identity, slot);

                        ApplySpriteToTaskbarIcon(taskbarIcon, config.popupIcon);

                        Button btn = taskbarIcon.GetComponent<Button>();
                        if (btn != null && windowController != null)
                        {
                            btn.onClick.AddListener(windowController.ToggleWindow);
                        }

                        activePopupTaskbarIcons[popupID] = taskbarIcon;
                    }
                }
                return;
            }
        }
        Debug.LogWarning($"[WindowManager] No popup found with ID: {popupID}");
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
        windowInstance.transform.SetAsLastSibling();

        WindowController windowController = windowInstance.GetComponent<WindowController>();
        if (windowController != null)
        {
            windowController.OpenWindow();
        }
    }

    private void ApplySpriteToTaskbarIcon(GameObject taskbarIcon, Sprite iconSprite)
{
    if (iconSprite == null) return;

    // Look for an Image component on a child object first (e.g. "Icon" image inside the button)
    Image[] images = taskbarIcon.GetComponentsInChildren<Image>();
    
    foreach (var img in images)
    {
        // Target child Image objects rather than the main background container
        if (img.gameObject != taskbarIcon)
        {
            img.sprite = iconSprite;
            img.color = Color.white; // Ensure tint color isn't hiding the sprite
            return;
        }
    }

    // If taskbarIconPrefab only has a single Image on the root object:
    if (images.Length > 0)
    {
        images[0].sprite = iconSprite;
        images[0].color = Color.white;
    }
}

    private int GetTotalActiveTaskbarCount()
    {
        return activeTaskbarIcons.Count + activePopupTaskbarIcons.Count;
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

    private void OnPopupClosed(string popupID)
    {
        if (activePopupTaskbarIcons.TryGetValue(popupID, out GameObject taskbarIcon))
        {
            if (taskbarIcon != null) Destroy(taskbarIcon);
            activePopupTaskbarIcons.Remove(popupID);
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