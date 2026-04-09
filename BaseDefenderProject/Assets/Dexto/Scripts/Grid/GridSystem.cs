using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }

    [Header("Grid Settings")]
    [SerializeField] private int slotSize = 8;
    [SerializeField] private int maxRadius = 4;
    [SerializeField] private float cellSize = 1f;

    [Header("Visual Colors")]
    [SerializeField] private bool showGrid = true;
    [SerializeField] private Color cellColor = new Color(1f, 1f, 1f, 0.12f);
    [SerializeField] private Color slotBorderColor = new Color(0.4f, 0.9f, 1f, 0.5f);
    [SerializeField] private Color availableColor = new Color(0.4f, 0.9f, 1f, 0.15f);

    private readonly Dictionary<Vector2Int, GridSlot> _slots = new();
    private readonly Dictionary<Vector2Int, Building> _cells = new();

    // Mesh Generation Data
    private MeshFilter _meshFilter;
    private List<Vector3> _vertices = new();
    private List<int> _indices = new();
    private List<Color> _colors = new();

    public float CellSize => cellSize;
    public int SlotSize => slotSize;
    private Vector3 Origin => transform.position;

    private static readonly Vector2Int[] CardinalDirs =
    {
        new( 1,  0), new(-1,  0),
        new( 0,  1), new( 0, -1)
    };

    private void OnEnable()
    {
        GameModeManager.OnModeChanged += OnGameModeChanged;
    }

    private void OnDisable()
    {
        GameModeManager.OnModeChanged -= OnGameModeChanged;
    }

    private void OnGameModeChanged(GameMode newMode)
    {
        GetComponent<MeshRenderer>().enabled = newMode == GameMode.Build;
    }

    void Awake()
    {
        Instance = this;
        _meshFilter = GetComponent<MeshFilter>();
        UnlockSlot(Vector2Int.zero);
    }

    

    #region Slot Logic

    public bool IsSlotUnlocked(Vector2Int slotCoord) =>
        _slots.TryGetValue(slotCoord, out var s) && s.isUnlocked;

    public bool CanUnlockSlot(Vector2Int slotCoord)
    {
        if (IsSlotUnlocked(slotCoord)) return false;
        if (Mathf.Abs(slotCoord.x) > maxRadius || Mathf.Abs(slotCoord.y) > maxRadius) return false;

        foreach (var dir in CardinalDirs)
            if (IsSlotUnlocked(slotCoord + dir)) return true;

        return false;
    }

    public void UnlockSlot(Vector2Int slotCoord)
    {
        if (!_slots.ContainsKey(slotCoord))
            _slots[slotCoord] = new GridSlot { slotCoord = slotCoord };

        _slots[slotCoord].isUnlocked = true;
        UpdateVisualMesh();
    }

    #endregion

    #region Building & Cell Logic

    public bool IsCellUnlocked(Vector2Int cell) => IsSlotUnlocked(CellToSlot(cell));
    public bool IsCellFree(Vector2Int cell) => IsCellUnlocked(cell) && !_cells.ContainsKey(cell);
    public Building GetBuilding(Vector2Int cell) => _cells.TryGetValue(cell, out var b) ? b : null;

    public bool IsAreaFree(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                if (!IsCellFree(new Vector2Int(origin.x + x, origin.y + y))) return false;
        return true;
    }

    public bool IsAreaUnlocked(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                if (!IsCellUnlocked(new Vector2Int(origin.x + x, origin.y + y))) return false;
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

    #endregion

    #region Coordinate Conversions

    public Vector3 CellToWorld(Vector2Int cell) =>
        Origin + new Vector3((cell.x + 0.5f) * cellSize, 0f, (cell.y + 0.5f) * cellSize);

    public Vector2Int WorldToCell(Vector3 world)
    {
        var local = world - Origin;
        return new Vector2Int(Mathf.FloorToInt(local.x / cellSize), Mathf.FloorToInt(local.z / cellSize));
    }

    public Vector3 SlotToWorld(Vector2Int slotCoord) =>
        Origin + new Vector3(slotCoord.x * slotSize * cellSize, 0f, slotCoord.y * slotSize * cellSize);

    private Vector2Int CellToSlot(Vector2Int cell) =>
        new Vector2Int(Mathf.FloorToInt((float)cell.x / slotSize), Mathf.FloorToInt((float)cell.y / slotSize));

    public Vector2Int WorldToSlot(Vector3 world)
    {
        Vector2Int cell = WorldToCell(world);

        // You MUST cast to float here, otherwise -1 / 8 becomes 0
        return new Vector2Int(
            Mathf.FloorToInt((float)cell.x / slotSize),
            Mathf.FloorToInt((float)cell.y / slotSize));
    }

    #endregion

    #region Procedural Mesh Rendering

    [ContextMenu("Update Mesh")]
    public void UpdateVisualMesh()
    {
        if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();

        _vertices.Clear();
        _indices.Clear();
        _colors.Clear();

        foreach (var kvp in _slots)
        {
            if (kvp.Value.isUnlocked)
            {
                AddSlotLines(kvp.Key, cellColor, true);
                AddSlotBorder(kvp.Key, slotBorderColor);
            }
        }

        HashSet<Vector2Int> checkedAvailable = new HashSet<Vector2Int>();
        foreach (var kvp in _slots)
        {
            if (!kvp.Value.isUnlocked) continue;
            foreach (var dir in CardinalDirs)
            {
                Vector2Int neighbor = kvp.Key + dir;
                if (!IsSlotUnlocked(neighbor) && CanUnlockSlot(neighbor) && !checkedAvailable.Contains(neighbor))
                {
                    AddSlotBorder(neighbor, availableColor);
                    checkedAvailable.Add(neighbor);
                }
            }
        }

        Mesh mesh = new Mesh { name = "GridMesh" };
        mesh.vertices = _vertices.ToArray();
        mesh.colors = _colors.ToArray();
        mesh.SetIndices(_indices.ToArray(), MeshTopology.Lines, 0);
        _meshFilter.mesh = mesh;
    }

    private void AddSlotLines(Vector2Int coord, Color color, bool drawInternal)
    {
        Vector3 o = SlotToWorld(coord);
        float sz = slotSize * cellSize;
        if (!drawInternal) return;

        for (int x = 1; x < slotSize; x++)
            AddLine(o + new Vector3(x * cellSize, 0, 0), o + new Vector3(x * cellSize, 0, sz), color);
        for (int y = 1; y < slotSize; y++)
            AddLine(o + new Vector3(0, 0, y * cellSize), o + new Vector3(sz, 0, y * cellSize), color);
    }

    private void AddSlotBorder(Vector2Int coord, Color color)
    {
        Vector3 o = SlotToWorld(coord);
        float sz = slotSize * cellSize;
        AddLine(o, o + new Vector3(sz, 0, 0), color);
        AddLine(o + new Vector3(sz, 0, 0), o + new Vector3(sz, 0, sz), color);
        AddLine(o + new Vector3(sz, 0, sz), o + new Vector3(0, 0, sz), color);
        AddLine(o + new Vector3(0, 0, sz), o, color);
    }

    private void AddLine(Vector3 start, Vector3 end, Color color)
    {
        int index = _vertices.Count;
        _vertices.Add(start); _vertices.Add(end);
        _colors.Add(color); _colors.Add(color);
        _indices.Add(index); _indices.Add(index + 1);
    }
    #endregion
}