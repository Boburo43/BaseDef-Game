using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyTester : MonoBehaviour

{
    [SerializeField] private Enemy enemy;
    [SerializeField] private EnemyGridManager gridManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnemyCell targetCell = gridManager.GetCell(5, 8);

        if (targetCell != null && targetCell.walkable)
        {
            enemy.SetTargetPosition(targetCell.worldPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
