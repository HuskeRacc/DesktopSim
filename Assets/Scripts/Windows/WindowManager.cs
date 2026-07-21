using System.Collections.Generic;
using UnityEngine;

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
        public string popupID;
        public GameObject popupPrefab;
        public Sprite popupIcon;
    }

    [Header("Popup Configurations")]
    [SerializeField] private List<PopupConfig> popupConfigs = new List<PopupConfig>();

    [Header("UI Canvas & App Configurations")]
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private AppWindowConfig[] appWindowConfigs;

    // Track active runtime instances
    private Dictionary<AppType, WindowController> activeWindows = new Dictionary<AppType, WindowController>();
    private Dictionary<AppType, GameObject> activeTaskbarIcons = new Dictionary<AppType, GameObject>();
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
        if (activeWindows.TryGetValue(appType, out WindowController existingWindow) && existingWindow != null)
        {
            existingWindow.OpenWindow();
            existingWindow.transform.SetAsLastSibling();
            return;
        }

        GameObject prefabToSpawn = GetPrefabForApp(appType);
        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"[WindowManager] No window prefab configured for AppType: {appType}");
            return;
        }

        GameObject windowInstance = Instantiate(prefabToSpawn, canvasTransform);
        windowInstance.transform.localScale = Vector3.one;

        WindowController windowController = windowInstance.GetComponent<WindowController>();
        if (windowController != null)
        {
            activeWindows[appType] = windowController;
            windowController.OnWindowClosed += () => OnWindowClosed(appType);
            windowController.OpenWindow();
        }

        // Delegate taskbar icon creation to IconManager
        if (IconManager.Instance != null)
        {
            int currentTaskbarIndex = GetTotalActiveTaskbarCount();
            GameObject taskbarIcon = IconManager.Instance.CreateTaskbarIcon(appType, currentTaskbarIndex, windowController);
            if (taskbarIcon != null)
            {
                activeTaskbarIcons[appType] = taskbarIcon;
            }
        }
    }

    public void OpenPopup(string popupID)
    {
        foreach (var config in popupConfigs)
        {
            if (config.popupID.Equals(popupID, System.StringComparison.OrdinalIgnoreCase))
            {
                if (activePopupTaskbarIcons.ContainsKey(popupID)) return;

                GameObject windowInstance = Instantiate(config.popupPrefab, canvasTransform);
                windowInstance.transform.localScale = Vector3.one;

                WindowController windowController = windowInstance.GetComponent<WindowController>();
                if (windowController != null)
                {
                    windowController.OnWindowClosed += () => OnPopupClosed(popupID);
                    windowController.OpenWindow();
                }

                // Delegate custom popup taskbar creation to IconManager
                if (IconManager.Instance != null)
                {
                    int currentTaskbarIndex = GetTotalActiveTaskbarCount();
                    GameObject taskbarIcon = IconManager.Instance.CreateCustomTaskbarIcon(config.popupIcon, currentTaskbarIndex, windowController);
                    if (taskbarIcon != null)
                    {
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
        if (windowPrefab == null) return;

        GameObject windowInstance = Instantiate(windowPrefab, canvasTransform);
        windowInstance.transform.localScale = Vector3.one;
        windowInstance.transform.SetAsLastSibling();

        WindowController windowController = windowInstance.GetComponent<WindowController>();
        if (windowController != null)
        {
            windowController.OpenWindow();
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

        // Let IconManager recalculate and clean up the remaining taskbar slots automatically
        if (IconManager.Instance != null)
        {
            IconManager.Instance.RefreshTaskbarSlots();
        }
    }

    private void OnPopupClosed(string popupID)
    {
        if (activePopupTaskbarIcons.TryGetValue(popupID, out GameObject taskbarIcon))
        {
            if (taskbarIcon != null) Destroy(taskbarIcon);
            activePopupTaskbarIcons.Remove(popupID);
        }

        // This calls the parameterless version so it safely recounts the remaining icons
        if (IconManager.Instance != null)
        {
            IconManager.Instance.RefreshTaskbarSlots();
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