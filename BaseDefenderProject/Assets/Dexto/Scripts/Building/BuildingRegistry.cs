// BuildingRegistry.cs
using System.Collections.Generic;
using UnityEngine;

public class BuildingRegistry : MonoBehaviour
{
    public static BuildingRegistry Instance { get; private set; }

    [SerializeField] private BuildingData[] allBuildings;

    private readonly Dictionary<string, BuildingData> _map = new();

    void Awake()
    {
        Instance = this;
        foreach (var b in allBuildings)
            _map[b.buildingId] = b;
    }

    public BuildingData Get(string id) =>
        _map.TryGetValue(id, out var d) ? d : null;

    public BuildingData[] GetAll() => allBuildings;
}