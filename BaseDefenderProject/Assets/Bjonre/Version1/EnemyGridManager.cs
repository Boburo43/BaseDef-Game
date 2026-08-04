using UnityEngine;

[ExecuteAlways]
public class EnemyGridManager : MonoBehaviour
{
    public int width = 30;
    public int height = 30;
    [SerializeField] float cellSize = 1f;
    public Vector3 originPosition = Vector3.zero;

    public EnemyCell[,] grid;

    private void Awake()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        grid = new EnemyCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = originPosition + new Vector3(x * cellSize + cellSize * 0.5f, 0f, y * cellSize + cellSize * 0.5f);

                grid[x, y] = new EnemyCell(x, y, pos);
            }
        }
    }

    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public EnemyCell GetCell(int x, int y)
    {
        if (!IsInsideGrid(x, y))
            return null;

        return grid[x, y];
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - originPosition.x) / cellSize);
        int y = Mathf.FloorToInt((worldPosition.z - originPosition.z) / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(int x, int y)
    {
        return originPosition + new Vector3(x * cellSize + cellSize * 0.5f, 0f, y * cellSize + cellSize * 0.5f);
    }

    public void SetWalkable(int x, int y, bool walkable)
    {
        if (IsInsideGrid(x, y))
        {
            grid[x, y].walkable = walkable;
        }
    }

    private void OnDrawGizmos()
    {
        if (grid == null)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                EnemyCell cell = grid[x, y];
                Gizmos.color = cell.walkable ? Color.white : Color.red;
                Gizmos.DrawWireCube(cell.worldPosition, new Vector3(cellSize, 0.05f, cellSize));
            }
        }
    }
}