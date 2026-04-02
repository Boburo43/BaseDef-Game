// GridDebug.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class GridDebug : MonoBehaviour
{
    void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out var hit)) return;

        var slotCoord = GridSystem.Instance.WorldToSlot(hit.point);
        var unlocked = GridExpansionManager.Instance.TryUnlockSlot(slotCoord);

        Debug.Log(unlocked
            ? $"Unlocked slot {slotCoord}"
            : $"Cannot unlock slot {slotCoord}");
    }
}