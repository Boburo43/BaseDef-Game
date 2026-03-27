using UnityEngine;

[System.Serializable]
public class EnemyCell
{
    public int x;
    public int y;
    public bool walkable = true;
    public Vector3 worldPosition;

    public EnemyCell(int x, int y, Vector3 worldPosition, bool walkable = true)
    {
        this.x = x;
        this.y = y;
        this.worldPosition = worldPosition;
        this.walkable = walkable;
    }
}
