// GridSystem.cs
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }

    [Header("Grid dimensions")]
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 20;
    [SerializeField] private float cellSize = 1f;

    [Header("Gizmo display")]
    [SerializeField] private Color gridColor = new Color(1f, 1f, 1f, 0.15f);
    [SerializeField] private Color originColor = new Color(0f, 1f, 0.5f, 0.8f);
    [SerializeField] private bool showCoords = false;

    private Building[,] _grid;

    // World origin comes from the GameObject's position,
    // so you can move the grid by moving the GameObject in the scene
    private Vector3 Origin => transform.position;

    void Awake()
    {
        Instance = this;
        _grid = new Building[width, height];
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

    public Vector3 SnapToGrid(Vector3 worldPos) =>
        CellToWorld(WorldToCell(worldPos));

    public bool IsInBounds(Vector2Int cell) =>
        cell.x >= 0 && cell.y >= 0 && cell.x < width && cell.y < height;

    public bool IsAreaFree(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                var c = new Vector2Int(origin.x + x, origin.y + y);
                if (!IsInBounds(c) || _grid[c.x, c.y] != null) return false;
            }
        return true;
    }

    public void Occupy(Vector2Int origin, Vector2Int size, Building building)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                _grid[origin.x + x, origin.y + y] = building;
    }

    public void Free(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                _grid[origin.x + x, origin.y + y] = null;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        var o = transform.position;
        float w = width * cellSize;
        float h = height * cellSize;

        // Grid lines
        Gizmos.color = gridColor;
        for (int x = 0; x <= width; x++)
        {
            var from = o + new Vector3(x * cellSize, 0, 0);
            var to = o + new Vector3(x * cellSize, 0, h);
            Gizmos.DrawLine(from, to);
        }
        for (int y = 0; y <= height; y++)
        {
            var from = o + new Vector3(0, 0, y * cellSize);
            var to = o + new Vector3(w, 0, y * cellSize);
            Gizmos.DrawLine(from, to);
        }

        // Outer border (slightly brighter)
        Gizmos.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridColor.a * 4f);
        Gizmos.DrawLine(o, o + new Vector3(w, 0, 0));
        Gizmos.DrawLine(o + new Vector3(w, 0, 0), o + new Vector3(w, 0, h));
        Gizmos.DrawLine(o + new Vector3(w, 0, h), o + new Vector3(0, 0, h));
        Gizmos.DrawLine(o + new Vector3(0, 0, h), o);

        // Origin marker (corner dot)
        Gizmos.color = originColor;
        Gizmos.DrawSphere(o, cellSize * 0.08f);

        // Optional: draw (x,y) label at each cell centre in the editor
        if (showCoords)
        {
            UnityEditor.Handles.color = new Color(1f, 1f, 1f, 0.4f);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    var centre = o + new Vector3((x + 0.5f) * cellSize, 0, (y + 0.5f) * cellSize);
                    UnityEditor.Handles.Label(centre, $"{x},{y}");
                }
        }
    }
#endif
}