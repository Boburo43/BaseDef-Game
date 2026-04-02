// GridSystem.cs
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }

    [SerializeField] private int slotSize = 8;
    [SerializeField] private int maxRadius = 4;
    [SerializeField] private float cellSize = 1f;

    [SerializeField] private Color cellColor = new Color(1f, 1f, 1f, 0.12f);
    [SerializeField] private Color slotBorderColor = new Color(0.4f, 0.9f, 1f, 0.5f);
    [SerializeField] private Color availableColor = new Color(0.4f, 0.9f, 1f, 0.15f);

    private readonly Dictionary<Vector2Int, GridSlot> _slots = new();
    private readonly Dictionary<Vector2Int, Building> _cells = new();

    public float CellSize => cellSize;
    public int SlotSize => slotSize;

    private Vector3 Origin => transform.position;

    private static readonly Vector2Int[] CardinalDirs =
    {
        new( 1,  0), new(-1,  0),
        new( 0,  1), new( 0, -1)
    };

    void Awake()
    {
        Instance = this;
        UnlockSlot(Vector2Int.zero);
    }

    public bool IsSlotUnlocked(Vector2Int slotCoord) =>
        _slots.TryGetValue(slotCoord, out var s) && s.isUnlocked;

    public bool CanUnlockSlot(Vector2Int slotCoord)
    {
        if (IsSlotUnlocked(slotCoord)) return false;
        if (Mathf.Abs(slotCoord.x) > maxRadius ||
            Mathf.Abs(slotCoord.y) > maxRadius) return false;

        foreach (var dir in CardinalDirs)
            if (IsSlotUnlocked(slotCoord + dir)) return true;

        return false;
    }

    public void UnlockSlot(Vector2Int slotCoord)
    {
        if (!_slots.ContainsKey(slotCoord))
            _slots[slotCoord] = new GridSlot { slotCoord = slotCoord };

        _slots[slotCoord].isUnlocked = true;
    }

    public bool IsCellUnlocked(Vector2Int cell) =>
        IsSlotUnlocked(CellToSlot(cell));

    public bool IsCellFree(Vector2Int cell) =>
        IsCellUnlocked(cell) && !_cells.ContainsKey(cell);

    public Building GetBuilding(Vector2Int cell) =>
        _cells.TryGetValue(cell, out var b) ? b : null;

    public bool IsAreaFree(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                if (!IsCellFree(new Vector2Int(origin.x + x, origin.y + y)))
                    return false;
        return true;
    }

    public bool IsAreaUnlocked(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                if (!IsCellUnlocked(new Vector2Int(origin.x + x, origin.y + y)))
                    return false;
        return true;
    }

    public void Occupy(Vector2Int origin, Vector2Int size, Building b)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                _cells[new Vector2Int(origin.x + x, origin.y + y)] = b;
    }

    public void Free(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                _cells.Remove(new Vector2Int(origin.x + x, origin.y + y));
    }

    public Vector3 CellToWorld(Vector2Int cell) =>
        Origin + new Vector3((cell.x + 0.5f) * cellSize, 0f, (cell.y + 0.5f) * cellSize);

    public Vector2Int WorldToCell(Vector3 world)
    {
        var local = world - Origin;
        return new Vector2Int(
            Mathf.FloorToInt(local.x / cellSize),
            Mathf.FloorToInt(local.z / cellSize));
    }

    public Vector2Int WorldToSlot(Vector3 world) =>
        CellToSlot(WorldToCell(world));

    public Vector3 SlotToWorld(Vector2Int slotCoord) =>
        Origin + new Vector3(
            slotCoord.x * slotSize * cellSize, 0f,
            slotCoord.y * slotSize * cellSize);

    private Vector2Int CellToSlot(Vector2Int cell) =>
        new Vector2Int(
            Mathf.FloorToInt((float)cell.x / slotSize),
            Mathf.FloorToInt((float)cell.y / slotSize));

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        foreach (var kvp in _slots)
            DrawSlot(kvp.Value);

        foreach (var kvp in _slots)
        {
            if (!kvp.Value.isUnlocked) continue;
            foreach (var dir in CardinalDirs)
            {
                var candidate = kvp.Key + dir;
                if (!_slots.ContainsKey(candidate) && CanUnlockSlot(candidate))
                    DrawSlotOutline(candidate, availableColor);
            }
        }
    }

    void DrawSlot(GridSlot slot)
    {
        var o = SlotToWorld(slot.slotCoord);
        float sz = slotSize * cellSize;

        if (slot.isUnlocked)
        {
            Gizmos.color = cellColor;
            for (int x = 0; x <= slotSize; x++)
                Gizmos.DrawLine(o + new Vector3(x * cellSize, 0, 0),
                                o + new Vector3(x * cellSize, 0, sz));
            for (int y = 0; y <= slotSize; y++)
                Gizmos.DrawLine(o + new Vector3(0, 0, y * cellSize),
                                o + new Vector3(sz, 0, y * cellSize));

            Gizmos.color = slotBorderColor;
        }
        else
        {
            Gizmos.color = availableColor;
        }

        DrawRect(o, sz);
    }

    void DrawSlotOutline(Vector2Int slotCoord, Color color)
    {
        Gizmos.color = color;
        DrawRect(SlotToWorld(slotCoord), slotSize * cellSize);
    }

    void DrawRect(Vector3 o, float size)
    {
        Gizmos.DrawLine(o, o + new Vector3(size, 0, 0));
        Gizmos.DrawLine(o + new Vector3(size, 0, 0), o + new Vector3(size, 0, size));
        Gizmos.DrawLine(o + new Vector3(size, 0, size), o + new Vector3(0, 0, size));
        Gizmos.DrawLine(o + new Vector3(0, 0, size), o);
    }
#endif
}