using UnityEngine;

public class BootScreenManager : MonoBehaviour
{
    private void Start()
    {
        this.gameObject.SetActive(true);
    }

    public void Login()
    {
        this.gameObject.SetActive(false);
    }
}
