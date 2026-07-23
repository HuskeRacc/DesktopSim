using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [Tooltip("The main root object of the window to move.")]
    [SerializeField] private RectTransform windowRectTransform;
    
    [Tooltip("The main UI Canvas.")]
    [SerializeField] private Canvas canvas;

    private RectTransform canvasRectTransform;

    private void Awake()
    {
        // Auto-fetch references if you forget to drag them in the inspector
        if (windowRectTransform == null)
        {
            windowRectTransform = transform.parent.GetComponent<RectTransform>(); 
        }
        
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Optional: Ensure the window pops to the front the moment you start dragging it!
        if (windowRectTransform != null)
        {
            windowRectTransform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null || windowRectTransform == null) return;

        // 1. Move the window based on mouse movement
        windowRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // 2. Immediately force it back inside the screen if it crossed a border
        ClampWindowToCanvas();
    }

    private void ClampWindowToCanvas()
    {
        // Get the exact physical boundaries of the canvas and the window
        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetWorldCorners(canvasCorners);

        Vector3[] windowCorners = new Vector3[4];
        windowRectTransform.GetWorldCorners(windowCorners);

        float xOffset = 0f;
        float yOffset = 0f;

        // Check Left and Right boundaries
        if (windowCorners[0].x < canvasCorners[0].x) 
        {
            xOffset = canvasCorners[0].x - windowCorners[0].x;
        }
        else if (windowCorners[2].x > canvasCorners[2].x) 
        {
            xOffset = canvasCorners[2].x - windowCorners[2].x;
        }

        // Check Bottom and Top boundaries
        if (windowCorners[0].y < canvasCorners[0].y) 
        {
            yOffset = canvasCorners[0].y - windowCorners[0].y;
        }
        else if (windowCorners[2].y > canvasCorners[2].y) 
        {
            yOffset = canvasCorners[2].y - windowCorners[2].y;
        }

        // Apply the correction if it tried to escape
        if (xOffset != 0f || yOffset != 0f)
        {
            windowRectTransform.position += new Vector3(xOffset, yOffset, 0);
        }
    }
}