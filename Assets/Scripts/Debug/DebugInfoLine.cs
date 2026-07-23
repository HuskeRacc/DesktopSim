using UnityEngine;
using TMPro;

public class DebugInfoLine : MonoBehaviour
{
    public TextMeshProUGUI infoText;

    // Call this to update the line (e.g., "Power Usage: 120.5")
    public void SetText(string label, string value)
    {
        if (infoText != null)
        {
            infoText.text = $"{label}: {value}";
        }
    }
}