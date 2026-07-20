using UnityEngine;
using UnityEngine.UI;

public class QuitPopupController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button confirmQuitButton;
    [SerializeField] private Button cancelButton;

    private void Awake()
    {
        if (confirmQuitButton != null)
        {
            confirmQuitButton.onClick.AddListener(QuitGame);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(HidePopup);
        }
    }

    public void ShowPopup()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling(); // Ensure it sits on top of all open app windows
    }

    public void HidePopup()
    {
        gameObject.SetActive(false);
    }

    public void TogglePopup()
    {
        if (gameObject.activeSelf)
        {
            HidePopup();
        }
        else
        {
            ShowPopup();
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}