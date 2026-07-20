using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerminalController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text outputText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Settings")]
    [SerializeField] private string userPrefix = "<color=#00FF00>user@os:~$</color> ";

    public Dictionary<string, ITerminalCommand> CommandRegistry { get; private set; } = new();

    private void Awake()
    {
        RegisterCommands();
    }

    private void Start()
    {
        outputText.text = "System Initialized. Type 'help' for available commands.\n";
        inputField.onSubmit.AddListener(ProcessCommand);
        FocusInput();
    }

    private void RegisterCommands()
    {
        // Simply register command instances here!
        Register(new HelpCommand());
        Register(new ClearCommand());
        Register(new EchoCommand());
    }

    private void Register(ITerminalCommand command)
    {
        string key = command.Name.ToLower();
        if (!CommandRegistry.ContainsKey(key))
        {
            CommandRegistry.Add(key, command);
        }
    }

    private void ProcessCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            FocusInput();
            return;
        }

        PrintLine(userPrefix + input);

        string[] parts = input.Trim().Split(' ', 2);
        string commandName = parts[0].ToLower();
        string args = parts.Length > 1 ? parts[1] : "";

        if (CommandRegistry.TryGetValue(commandName, out ITerminalCommand command))
        {
            command.Execute(this, args);
        }
        else
        {
            PrintLine($"<color=#FF0000>Error:</color> Command '{commandName}' not recognized.");
        }

        inputField.text = "";
        FocusInput();
    }

    public void PrintLine(string text)
    {
        if (outputText.text.Length > 0) outputText.text += "\n";
        outputText.text += text;
        StartCoroutine(ScrollToBottom());
    }

    public void ClearScreen()
    {
        outputText.text = "";
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;
    }

    public void FocusInput()
    {
        if (inputField == null) return;
        inputField.ActivateInputField();
        inputField.Select();
    }
}