using UnityEngine;
using UnityEngine.UI;

public class WindowController : MonoBehaviour
{
    [SerializeField] private Button closeButton; // Drag 'X' button here
    [SerializeField] private Button minimizeButton; // Drag minimize button here

    // Event so IconManager knows when this closes without hard-coupling
    public System.Action OnWindowClosed;

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseWindow);
        }

        if (minimizeButton != null)
        {
            minimizeButton.onClick.AddListener(MinimizeWindow);
        }
    }

    public void OpenWindow()
    {
        gameObject.SetActive(true);
    }

    public void CloseWindow()
    {
        OnWindowClosed?.Invoke(); // Notify manager
        Destroy(gameObject); // Destroy the window
    }

    public void MinimizeWindow()
    {
        gameObject.SetActive(false);
    }

    public void ToggleWindow()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}