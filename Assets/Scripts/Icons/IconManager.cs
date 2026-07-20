using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IconManager : MonoBehaviour
{
    public static IconManager Instance { get; private set; }

    [Header("Icon Prefabs")]
    [SerializeField] private GameObject desktopIconPrefab;

    [Header("Icon Positions")]
    [SerializeField] private GameObject[] desktopIconPositions;

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
        // Hand off window management to WindowManager!
        if (WindowManager.Instance != null)
        {
            WindowManager.Instance.OpenAppWindow(clickedApp);
        }
    }

    public AppData GetAppData(AppType appType)
    {
        return System.Array.Find(availableApps, a => a.appType == appType);
    }
}