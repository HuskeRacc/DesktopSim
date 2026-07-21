using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IconManager : MonoBehaviour
{
    public static IconManager Instance { get; private set; }

    [Header("Icon Prefabs")]
    [SerializeField] private GameObject desktopIconPrefab;
    [SerializeField] private GameObject taskbarIconPrefab;

    [Header("Containers & Positions")]
    [SerializeField] private GameObject[] desktopIconPositions;
    [SerializeField] private Transform taskbarContainer; // Drag your 'IconSlots' parent object here!

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

    // --- TASKBAR MANAGEMENT (No Slots Needed!) ---

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