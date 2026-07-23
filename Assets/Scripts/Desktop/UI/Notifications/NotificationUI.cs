using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;

    [Header("Settings")]
    public float displayTime = 5f;

    public void Setup(string title, string message)
    {
        if (titleText != null) titleText.text = title;
        if (messageText != null) messageText.text = message;

        // Start the countdown to destroy the notification
        StartCoroutine(AutoDismiss());
    }

    private IEnumerator AutoDismiss()
    {
        yield return new WaitForSeconds(displayTime);
        Dismiss();
    }

    // Call this if you add a little "X" close button to the prefab
    public void Dismiss()
    {
        Destroy(gameObject);
    }
}