using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [Tooltip("The main window RectTransform that should move when this handle is dragged.")]
    [SerializeField] private RectTransform windowRectTransform;

    Canvas parentCanvas;

    private void Awake()
    {
        if(windowRectTransform == null)
        {
            windowRectTransform = GetComponentInParent<WindowController>()?.GetComponent<RectTransform>();
        }

        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(windowRectTransform != null)
        {
            windowRectTransform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (windowRectTransform == null || parentCanvas == null) return;

        // Move the window position based on mouse delta, adjusted for UI Canvas scale
        windowRectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }
}
