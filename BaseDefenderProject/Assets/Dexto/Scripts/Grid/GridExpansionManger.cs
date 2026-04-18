using UnityEngine;
using UnityEngine.InputSystem; 

public class GridExpansionManager : MonoBehaviour
{
    public static GridExpansionManager Instance { get; private set; }

    public bool isExpanding { get; set; }

    void Awake() => Instance = this;


    void Update()
    {
        if (GameModeManager.Instance.CurrentMode != GameMode.Build) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (!isExpanding) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        var ray = Camera.main.ScreenPointToRay(mousePos);


        if (Physics.Raycast(ray, out var hit))
        {
            var slotCoord = GridSystem.Instance.WorldToSlot(hit.point);
            TryUnlockSlot(slotCoord);
        }
    }

    public bool TryUnlockSlot(Vector2Int slotCoord)
    {
        // Check if the slot is a valid neighbor and within max radius
        if (!GridSystem.Instance.CanUnlockSlot(slotCoord)) return false;
        if (isExpanding)
        {
            GridSystem.Instance.UnlockSlot(slotCoord);
        }
        return false;
    }
}