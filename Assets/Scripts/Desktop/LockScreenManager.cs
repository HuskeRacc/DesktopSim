using UnityEngine;

public class LockScreenManager : MonoBehaviour
{
    public void Start()
    {
        TimeManager.Instance.PauseTime();
    }

    public void Login()
    {
        TimeManager.Instance.ResumeTime();
        this.gameObject.SetActive(false);
    }
}
