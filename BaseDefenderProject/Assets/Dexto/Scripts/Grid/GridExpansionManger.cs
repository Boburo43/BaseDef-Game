using UnityEngine;
using UnityEngine.InputSystem; // New Input System

public class GridExpansionManager : MonoBehaviour
{
    public static GridExpansionManager Instance { get; private set; }

    void Awake() => Instance = this;

    void Update()
    {
        // Equivalent to GetMouseButtonDown(0)
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // Use the new Mouse position API
        Vector2 mousePos = Mouse.current.position.ReadValue();
        var ray = Camera.main.ScreenPointToRay(mousePos);

        if (!Physics.Raycast(ray, out var hit)) return;

        // The logic you requested:
        var slotCoord = GridSystem.Instance.WorldToSlot(hit.point);
        var unlocked = TryUnlockSlot(slotCoord);

        Debug.Log(unlocked
            ? $"<color=green>Unlocked slot {slotCoord}</color>"
            : $"<color=red>Cannot unlock slot {slotCoord}</color>");
    }

    public bool TryUnlockSlot(Vector2Int slotCoord)
    {
        // Check if the slot is a valid neighbor and within max radius
        if (!GridSystem.Instance.CanUnlockSlot(slotCoord)) return false;

        GridSystem.Instance.UnlockSlot(slotCoord);
        return true;
    }
}