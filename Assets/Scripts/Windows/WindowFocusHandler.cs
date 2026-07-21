using UnityEngine;
using UnityEngine.EventSystems;

public class WindowFocusHandler : MonoBehaviour, IPointerDownHandler
{
    [Tooltip("Drag the main root object of the window here. If left blank, it assumes this script is on the root object.")]
    [SerializeField] private RectTransform rootWindowTransform;

    private void Awake()
    {
        if (rootWindowTransform == null)
        {
            rootWindowTransform = GetComponent<RectTransform>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (rootWindowTransform != null)
        {
            rootWindowTransform.SetAsLastSibling();
        }
    }
}
