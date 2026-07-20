using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DesktopIconItem : MonoBehaviour
{
    [HideInInspector] public AppType assignedAppType;
    
    [SerializeField] private Button iconButton;

    // A method to initialize its look and behavior when spawned
    public void SetupIcon(AppData data, System.Action<AppType> onClickAction)
    {
        assignedAppType = data.appType;

        if (iconButton != null)
        {
            iconButton.onClick.AddListener(() => onClickAction(assignedAppType));
        }
    }
}