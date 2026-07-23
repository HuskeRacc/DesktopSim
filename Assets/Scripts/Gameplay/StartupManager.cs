using UnityEngine;

public class StartupManager : MonoBehaviour
{
    public GameObject lockScreen;

    void Start()
    {
        lockScreen.SetActive(true);
    }
}
