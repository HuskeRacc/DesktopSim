using UnityEngine;

public class LockScreenManager : MonoBehaviour
{
    public static LockScreenManager Instance;

    public GameObject lockScreen;

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
        ShowLockScreen();
    }

    public void ShowLockScreen()
    {
        TimeManager.Instance.PauseTime();
        lockScreen.SetActive(true);
    }

    public void CloseLockScreen()
    {
        TimeManager.Instance.ResumeTime();
        lockScreen.SetActive(false);
    }
}
