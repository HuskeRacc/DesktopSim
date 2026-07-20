using UnityEngine;
using TMPro;

public class DateTime : MonoBehaviour
{
    public TextMeshProUGUI dateTimeText;

    void Update()
    {
        dateTimeText.text = 
        System.DateTime.Now.ToString("HH:mm:ss\ndd-MM-yyyy");
    }
}
