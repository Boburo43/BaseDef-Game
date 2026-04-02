using UnityEngine;

public enum ResourceType { Wood, Stone }

[System.Serializable]
public struct ResourceCost
{
    public ResourceType type;
    public int amount;
}