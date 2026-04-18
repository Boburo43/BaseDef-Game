using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class DevConsoleUI : MonoBehaviour
{
    // UI references
    private VisualElement _container;
    private TextField _inputField;
    private ScrollView _historyLog;

    // State
    private bool _isOpen = false;

    // Input
    [Tooltip("Drag the same PlayerInput object you use in GameModeManager.")]
    public PlayerInput playerInput;
    private InputAction _toggleAction;

    // Command registry
    private readonly Dictionary<string, Func<string[], string>> _commands = new();

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _container = root.Q<VisualElement>("Root");
        _inputField = root.Q<TextField>("ConsoleInput");
        _historyLog = root.Q<ScrollView>("HistoryLog");

        _container.style.display = DisplayStyle.None;

        // Ensure single-line input (important!)
        _inputField.multiline = false;

        // Input setup
        if (playerInput != null)
        {
            var defaultMap = playerInput.actions.FindActionMap("Default");
            if (defaultMap != null)
            {
                _toggleAction = defaultMap.FindAction("ToggleConsole");
                if (_toggleAction != null)
                    _toggleAction.performed += OnToggleCalled;
                else
                    Debug.LogError("[DevConsoleUI] Could not find 'ToggleConsole'.");
            }
            else
            {
                Debug.LogError("[DevConsoleUI] Could not find 'Default' action map.");
            }
        }
        else
        {
            Debug.LogError("[DevConsoleUI] PlayerInput reference is not assigned.");
        }

       
        _inputField.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);
    }

    private void OnDisable()
    {
        if (_toggleAction != null)
            _toggleAction.performed -= OnToggleCalled;

        _inputField?.UnregisterCallback<NavigationSubmitEvent>(OnSubmit);
        _inputField?.UnregisterCallback<KeyDownEvent>(OnInputKeyDown);
    }

    // Toggle

    private void OnToggleCalled(InputAction.CallbackContext ctx) => SetOpen(!_isOpen);

    private void SetOpen(bool open)
    {
        _isOpen = open;
        _container.style.display = open ? DisplayStyle.Flex : DisplayStyle.None;

        if (open)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            FocusInput();
        }
        else
        {
            _inputField.Blur();

            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }

    private void FocusInput()
    {
        // Cleaner than coroutine
        _container.schedule.Execute(() =>
        {
            _inputField.value = "";
            _inputField.Focus();
        }).ExecuteLater(0);
    }

    // Input

    private void OnSubmit(NavigationSubmitEvent evt)
    {
        SubmitCommand();
        evt.StopPropagation();
    }

    private void OnInputKeyDown(KeyDownEvent evt)
    {
        if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
        {
            SubmitCommand();

            evt.StopImmediatePropagation(); 

            return;
        }

        if (evt.keyCode == KeyCode.Escape)
        {
            SetOpen(false);
            evt.StopPropagation();
        }
    }

    // Commands

    public void Register(string name, Func<string[], string> handler)
    {
        _commands[name.ToLower()] = handler;
    }

    private void SubmitCommand()
    {
        string input = _inputField.value.Trim();

        if (string.IsNullOrWhiteSpace(input))
        {
            _inputField.value = "";
            return;
        }

        AddLogEntry($"> {input}");
        ProcessCommand(input);

        _inputField.value = "";
        _inputField.Focus();
    }

    public void ProcessCommand(string raw)
    {
        string[] parts = raw.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        string name = parts[0].ToLower();
        string[] args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        if (!_commands.TryGetValue(name, out var handler))
        {
            AddLogEntry($"Unknown command: {name}");
            return;
        }

        try
        {
            string result = handler(args);
            if (!string.IsNullOrEmpty(result))
                AddLogEntry(result);
        }
        catch (Exception ex)
        {
            AddLogEntry($"Error: {ex.Message}");
        }
    }

    // Log output

    public void AddLogEntry(string message)
    {
        var label = new Label(message);
        label.style.whiteSpace = WhiteSpace.Normal;
        _historyLog.Add(label);

        // Smooth scroll after layout update
        _historyLog.schedule.Execute(() =>
        {
            _historyLog.scrollOffset = new Vector2(0, float.MaxValue);
        }).ExecuteLater(0);
    }
}