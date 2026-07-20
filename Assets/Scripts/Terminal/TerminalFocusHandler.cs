using UnityEngine;
using UnityEngine.EventSystems;

public class TerminalFocusHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TerminalController terminalController;

    private void Reset()
    {
        terminalController = GetComponentInParent<TerminalController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (terminalController != null)
        {
            terminalController.FocusInput();
        }
    }
}