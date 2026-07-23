using UnityEngine;
using UnityEngine.UI;

public class PausePopup : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button quitButton;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueGame);
        }
    }

    public void ShowPopup()
    {
        TimeManager.Instance.PauseTime();
        gameObject.SetActive(true);
        transform.SetAsLastSibling(); // Ensure it sits on top of all open app windows
    }

    public void ContinueGame()
    {
        TimeManager.Instance.ResumeTime();
        gameObject.SetActive(false);
    }

    public void TogglePopup()
    {
        if (gameObject.activeSelf)
        {
            ContinueGame();
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