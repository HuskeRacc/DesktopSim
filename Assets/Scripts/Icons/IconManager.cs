using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IconManager : MonoBehaviour
{
    public static IconManager Instance { get; private set; }

    [Header("Icon Prefabs")]
    [SerializeField] private GameObject desktopIconPrefab;
    [SerializeField] private GameObject taskbarIconPrefab;

    [Header("Containers")]
    [SerializeField] private Transform desktopIconsContainer; // Drag your DesktopIcons grid object here!
    [SerializeField] private Transform taskbarContainer;      // Drag your taskbar layout object here!

    [Header("Available Apps")]
    [SerializeField] private AppData[] availableApps;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitialiseDesktopIcons();
    }

    private void InitialiseDesktopIcons()
    {
        if (desktopIconsContainer == null)
        {
            Debug.LogWarning("[IconManager] Desktop Icons Container is not assigned!");
            return;
        }

        // Loop through the database and ONLY spawn apps marked as installed
        for (int i = 0; i < availableApps.Length; i++)
        {
            if (availableApps[i].isInstalled)
            {
                SpawnDesktopIcon(availableApps[i]);
            }
        }
    }

    private void SpawnDesktopIcon(AppData app)
    {
        GameObject spawnedIcon = Instantiate(desktopIconPrefab, desktopIconsContainer);
        spawnedIcon.transform.localScale = Vector3.one;

        DesktopIconItem iconItem = spawnedIcon.GetComponent<DesktopIconItem>();
        if (iconItem != null)
        {
            iconItem.SetupIcon(app, OnIconClicked);
        }

        Image iconImage = spawnedIcon.GetComponentInChildren<Image>();
        if (iconImage != null) iconImage.sprite = app.iconSprite;

        TextMeshProUGUI appText = spawnedIcon.GetComponentInChildren<TextMeshProUGUI>();
        if (appText != null) appText.text = app.appName;
    }

    private void OnIconClicked(AppType clickedApp)
    {
        if (WindowManager.Instance != null)
        {
            WindowManager.Instance.OpenAppWindow(clickedApp);
        }
    }

    public void InstallApp(AppType appType)
    {
        for (int i = 0; i < availableApps.Length; i++)
        {
            if (availableApps[i].appType == appType)
            {
                if (!availableApps[i].isInstalled)
                {
                    // Check if the app is free (0 cost) OR if the player has enough money
                    if (availableApps[i].priceUSD <= 0f || ResourceManager.Instance.SpendUSD(availableApps[i].priceUSD))
                    {
                        // Transaction successful (or it was free)! Install it.
                        availableApps[i].isInstalled = true; 
                        SpawnDesktopIcon(availableApps[i]);
                        Debug.Log($"[IconManager] Successfully bought and installed {appType}!");
                    }
                    else
                    {
                        // Transaction failed
                        Debug.LogWarning($"[IconManager] Player cannot afford {appType}! It costs ${availableApps[i].priceUSD}");
                        
                        // Optional: You could trigger a popup window here saying "Not enough funds!"
                    }
                }
                else
                {
                    Debug.Log($"[IconManager] {appType} is already installed.");
                }
                return;
            }
        }
        Debug.LogWarning($"[IconManager] Could not find {appType} in the availableApps database!");
    }

    public AppData GetAppData(AppType appType)
    {
        return System.Array.Find(availableApps, a => a.appType == appType);
    }

    // --- TASKBAR MANAGEMENT ---

    public GameObject CreateTaskbarIcon(AppType appType, WindowController windowController)
    {
        AppData appData = GetAppData(appType);
        return SpawnTaskbarIcon(appData.iconSprite, windowController);
    }

    public GameObject CreateCustomTaskbarIcon(Sprite customSprite, WindowController windowController)
    {
        return SpawnTaskbarIcon(customSprite, windowController);
    }

    private GameObject SpawnTaskbarIcon(Sprite sprite, WindowController windowController)
    {
        if (taskbarContainer == null)
        {
            Debug.LogError("[IconManager] Taskbar Container is not assigned!");
            return null;
        }

        // Instantiating directly into the container lets the Horizontal Layout Group space them automatically
        GameObject taskbarIcon = Instantiate(taskbarIconPrefab, taskbarContainer);
        taskbarIcon.transform.localScale = Vector3.one;

        if (sprite != null)
        {
            ApplySpriteToTaskbarIcon(taskbarIcon, sprite);
        }

        Button btn = taskbarIcon.GetComponent<Button>();
        if (btn != null && windowController != null)
        {
            btn.onClick.AddListener(windowController.ToggleWindow);
        }

        return taskbarIcon;
    }

    private void ApplySpriteToTaskbarIcon(GameObject taskbarIcon, Sprite iconSprite)
    {
        if (iconSprite == null) return;

        Image[] images = taskbarIcon.GetComponentsInChildren<Image>();
        foreach (var img in images)
        {
            if (img.gameObject != taskbarIcon)
            {
                img.sprite = iconSprite;
                img.color = Color.white;
                return;
            }
        }

        if (images.Length > 0)
        {
            images[0].sprite = iconSprite;
            images[0].color = Color.white;
        }
    }
}