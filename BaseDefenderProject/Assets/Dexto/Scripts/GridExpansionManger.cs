// GridExpansionManager.cs
using UnityEngine;

public class GridExpansionManager : MonoBehaviour
{
    public static GridExpansionManager Instance { get; private set; }

    void Awake() => Instance = this;

    public bool TryUnlockSlot(Vector2Int slotCoord)
    {
        if (!GridSystem.Instance.CanUnlockSlot(slotCoord)) return false;
        GridSystem.Instance.UnlockSlot(slotCoord);
        return true;
    }
}