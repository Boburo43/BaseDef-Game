using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingData Data { get; private set; }
    public Vector2Int GridOrigin { get; private set; }

    public void Initialize(BuildingData data, Vector2Int origin)
    {
        Data = data;
        GridOrigin = origin;
    }

    public void Demolish()
    {
        GridSystem.Instance.Free(GridOrigin, Data.size);
        Destroy(gameObject);
    }
}