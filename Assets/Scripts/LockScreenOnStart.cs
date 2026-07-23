using UnityEngine;

public class LockScreenOnStart : MonoBehaviour
{
    public GameObject lockScreen;

    void Start()
    {
        lockScreen.SetActive(true);
    }
}
