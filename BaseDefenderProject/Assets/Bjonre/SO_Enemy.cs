using UnityEngine;

[CreateAssetMenu(fileName = "SO_Enemy", menuName = "Scriptable Objects/SO_Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName;
    public Sprite icon;

    [Header("Stats")]
    public int maxHealth = 100;
    public float moveSpeed = 3f;
    public int damage = 10;

    [Header("Combat")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    [Header("Rewards")]
    public int goldReward = 5;
}