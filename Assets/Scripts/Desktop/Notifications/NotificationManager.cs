using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [Header("Setup")]
    [Tooltip("The notification prefab to spawn")]
    public GameObject notificationPrefab;
    
    [Tooltip("An empty UI Panel with a Vertical Layout Group component")]
    public Transform notificationContainer; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Any script can call this exact method!
    public void SendNotification(string title, string message)
    {
        if (notificationPrefab == null || notificationContainer == null) return;

        // Spawn the notification inside the container
        GameObject newNotif = Instantiate(notificationPrefab, notificationContainer);
        
        // Pass the text to the UI script
        NotificationUI uiScript = newNotif.GetComponent<NotificationUI>();
        if (uiScript != null)
        {
            uiScript.Setup(title, message);
        }
    }
}