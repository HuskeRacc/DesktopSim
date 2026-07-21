using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IconManager : MonoBehaviour
{
    public static IconManager Instance { get; private set; }

    [Header("Icon Prefabs")]
    [SerializeField] private GameObject desktopIconPrefab;
    [SerializeField] private GameObject taskbarIconPrefab;

    [Header("Desktop Icon Positions")]
    [SerializeField] private GameObject[] desktopIconPositions;

    [Header("Taskbar Icon Positions")]
    [SerializeField] private GameObject[] taskbarIconPositions;

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
        Initialise();
        RefreshTaskbarSlots(0);
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
        if (WindowManager.Instance != null)
        {
            WindowManager.Instance.OpenAppWindow(clickedApp);
        }
    }

    public AppData GetAppData(AppType appType)
    {
        return System.Array.Find(availableApps, a => a.appType == appType);
    }

    // --- TASKBAR MANAGEMENT ---

    // Standard App Taskbar Creation
    public GameObject CreateTaskbarIcon(AppType appType, int slotIndex, WindowController windowController)
    {
        AppData appData = GetAppData(appType);
        Sprite iconSprite = appData.iconSprite; // No null check needed on appData itself!
        
        return SpawnTaskbarIconAtSlot(iconSprite, slotIndex, windowController);
    }

    // Custom Window / Popup Taskbar Creation (Safe for non-standard windows!)
    public GameObject CreateCustomTaskbarIcon(Sprite customSprite, int slotIndex, WindowController windowController)
    {
        return SpawnTaskbarIconAtSlot(customSprite, slotIndex, windowController);
    }

    private GameObject SpawnTaskbarIconAtSlot(Sprite sprite, int slotIndex, WindowController windowController)
    {
        if (slotIndex >= taskbarIconPositions.Length)
        {
            Debug.LogWarning("[IconManager] Not enough taskbar slots available!");
            return null;
        }

        GameObject slotObject = taskbarIconPositions[slotIndex];
        slotObject.SetActive(true);

        GameObject taskbarIcon = Instantiate(taskbarIconPrefab, slotObject.transform.position, Quaternion.identity, slotObject.transform);

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

    public void RefreshTaskbarSlots(int activeCount = -1)
    {
        // If no count is specified, dynamically count how many slots actually have children (icons) inside them!
        if (activeCount < 0)
        {
            activeCount = 0;
            foreach (var slot in taskbarIconPositions)
            {
                if (slot != null && slot.transform.childCount > 0)
                {
                    activeCount++;
                }
            }
        }

        // Toggle the slot GameObjects on or off based on active count
        for (int i = 0; i < taskbarIconPositions.Length; i++)
        {
            if (taskbarIconPositions[i] != null)
            {
                taskbarIconPositions[i].SetActive(i < activeCount);
            }
        }
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