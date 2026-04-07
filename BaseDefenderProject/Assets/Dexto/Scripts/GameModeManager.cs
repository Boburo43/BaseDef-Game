using UnityEngine;
using UnityEngine.InputSystem;
using System;

public enum GameMode { Move, Build }

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }

    [Header("References")]
    [Tooltip("Drag the Player object with the PlayerInput component here.")]
    public PlayerInput playerInput;

    public GameMode CurrentMode { get; private set; } = GameMode.Move;
    public static event Action<GameMode> OnModeChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (playerInput == null) return;
        ToggleGlobalBindings(true);
    }

    private void OnDisable()
    {
        if (playerInput == null) return;
        ToggleGlobalBindings(false);
    }

    private void Start()
    {
        // Ensure Default map is always listening
        var defaultMap = playerInput.actions.FindActionMap("Default");
        if (defaultMap != null) defaultMap.Enable();

        SetMode(GameMode.Move);
    }

 
    private void ToggleGlobalBindings(bool shouldBind)
    {
        var map = playerInput.actions.FindActionMap("Default");
        if (map == null) return;

        // Find actions
        var toggleAction = map.FindAction("ToggleMode");
        var pauseAction = map.FindAction("Pause"); 

        if (shouldBind)
        {
            // Subscribe
            if (toggleAction != null) toggleAction.performed += OnToggleInput;
            if (pauseAction != null) pauseAction.performed += OnPauseInput;
        }
        else
        {
            // Unsubscribe 
            if (toggleAction != null) toggleAction.performed -= OnToggleInput;
            if (pauseAction != null) pauseAction.performed -= OnPauseInput;
        }
    }

   

    private void OnToggleInput(InputAction.CallbackContext context)
    {
        GameMode nextMode = (CurrentMode == GameMode.Move) ? GameMode.Build : GameMode.Move;
        SetMode(nextMode);
    }

    private void OnPauseInput(InputAction.CallbackContext context)
    {
        Debug.Log("Pause Menu Toggled!");
        
    }


    public void SetMode(GameMode newMode)
    {
        CurrentMode = newMode;

        // Switch Action Maps
        if (newMode == GameMode.Build)
        {
            playerInput.actions.FindActionMap("MoveMode").Disable();
            playerInput.actions.FindActionMap("BuildMode").Enable();
        }
        else
        {
            playerInput.actions.FindActionMap("BuildMode").Disable();
            playerInput.actions.FindActionMap("MoveMode").Enable();
        }

        OnModeChanged?.Invoke(newMode);
        Debug.Log("Game Mode: " + newMode);
    }
}