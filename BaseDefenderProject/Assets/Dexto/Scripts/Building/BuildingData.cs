using UnityEngine;

public enum BuildingCategory { Defense, ResourceGatherer, Wall }

[CreateAssetMenu(menuName = "Buildings/BuildingData")]
public class BuildingData : ScriptableObject
{
    [Header("Identity")]
    public string buildingId;
    public string displayName;
    public GameObject prefab;
    public Sprite icon;

    [Header("Grid")]
    public Vector2Int size = Vector2Int.one;

    [Header("Category")]
    public BuildingCategory category;

    [Header("Cost")]
    public ResourceCost[] buildCost;

    [Header("Stats")]
    public int maxHealth = 100;
}