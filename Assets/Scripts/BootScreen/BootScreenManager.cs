using UnityEngine;

public class BootScreenManager : MonoBehaviour
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
