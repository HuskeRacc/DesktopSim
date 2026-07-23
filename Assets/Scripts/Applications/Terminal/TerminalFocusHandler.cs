using UnityEngine;
using UnityEngine.EventSystems;

public class TerminalFocusHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TerminalManager terminalController;

    private void Reset()
    {
        terminalController = GetComponentInParent<TerminalManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (terminalController != null)
        {
            terminalController.FocusInput();
        }
    }
}