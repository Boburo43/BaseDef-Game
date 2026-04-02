// GridSlot.cs
using UnityEngine;

[System.Serializable]
public class GridSlot
{
    public Vector2Int slotCoord;
    public bool isUnlocked;

    public Vector2Int CellOrigin(int slotSize) =>
        new Vector2Int(slotCoord.x * slotSize, slotCoord.y * slotSize);
}