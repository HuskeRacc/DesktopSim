using UnityEngine;

public class DisableOnPlay : MonoBehaviour
{
    public GameObject[] objectsToDisable;

    private void Start()
    {
        objectsToDisable = GameObject.FindGameObjectsWithTag("DisableOnPlay");

        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
        }
    }
}
