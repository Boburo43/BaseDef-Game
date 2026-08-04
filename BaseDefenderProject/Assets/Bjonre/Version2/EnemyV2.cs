using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyV2 : MonoBehaviour
{
    public bool findDistance = false;
    public GridBehaviour gridScript;
    public int startX = 0;
    public int startY = 0;
    public int endX = 2;
    public int endY = 2;
    public List<GameObject> path = new List<GameObject>();

    [Header("Movement Speed")]
    public float moveSpeed = 3f;
    private int pathIndex;
    private bool isMoving;

    //Till spawnern senare: 
    //   EnemyV2 enemy = Instantiate(enemyPrefab);
    //    enemy.gridScript = gridGenerator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(gridScript.leftBottomLocation.x, transform.position.y, gridScript.leftBottomLocation.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (findDistance)
        {
            SetDistance();
            SetPath();

            path.Reverse();
            pathIndex = 0;
            isMoving = path.Count > 0;

            findDistance = false;
        }
        if (isMoving)
        {
            FollowPath();
        }
    }

    void FollowPath()
    {
        if (pathIndex >= path.Count)
        {
            isMoving = false;
            return;
        }

        Vector3 targetPosition = path[pathIndex].transform.position;

        targetPosition.y = transform.position.y;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            pathIndex++;
        }
        startX = endX;
        startY = endY;

    }
    void SetDistance()
    {
        InitialSetup();
        int x = startX;
        int y = startY;
        int[] testArray = new int[gridScript.rows * gridScript.columns];
        for (int step = 1; step < gridScript.rows * gridScript.columns; step++)
        {
            foreach (GameObject obj in gridScript.gridArray)
            {
                if (obj && obj.GetComponent<GridStat>().visited == step - 1)
                {
                    TestFourDirections(obj.GetComponent<GridStat>().x, obj.GetComponent<GridStat>().y, step);
                }
            }
        }
    }
    public void SetPath()
    {
        int step;
        int x = endX;
        int y = endY;
        List<GameObject> tempList = new List<GameObject>();
        path.Clear();
        if (gridScript.gridArray[endX, endY] && gridScript.gridArray[endX, endY].GetComponent<GridStat>().visited > 0)
        {
            path.Add(gridScript.gridArray[x, y]);
            step = gridScript.gridArray[x, y].GetComponent<GridStat>().visited - 1;
        }
        else
        {
            print("Cant reach the desired location.");
            return;
        }
        for (int i = step; step > -1; step--)
        {
            if (TestDirection(x, y, step, 1))
                tempList.Add(gridScript.gridArray[x, y + 1]);
            if (TestDirection(x, y, step, 2))
                tempList.Add(gridScript.gridArray[x + 1, y]);
            if (TestDirection(x, y, step, 3))
                tempList.Add(gridScript.gridArray[x, y - 1]);
            if (TestDirection(x, y, step, 4))
                tempList.Add(gridScript.gridArray[x - 1, y]);
            GameObject tempObj = FindClosest(gridScript.gridArray[endX, endY].transform, tempList);
            path.Add(tempObj);
            x = tempObj.GetComponent<GridStat>().x;
            y = tempObj.GetComponent<GridStat>().y;
            tempList.Clear();
        }
    }
    public void InitialSetup()
    {
        foreach (GameObject obj in gridScript.gridArray)
        {
            obj.GetComponent<GridStat>().visited = -1;
        }
        gridScript.gridArray[startX, startY].GetComponent<GridStat>().visited = 0;
    }
    bool TestDirection(int x, int y, int step, int direction)
    {
        // int direction tells which case to use 1 is up, 2 is right, 3 is down, 4 is left
        switch (direction)
        {
            case 1:
                if (y + 1 < gridScript.rows && gridScript.gridArray[x, y + 1] && gridScript.gridArray[x, y + 1].GetComponent<GridStat>().visited == step)
                    return true;
                else
                    return false;
            case 2:
                if (x + 1 < gridScript.columns && gridScript.gridArray[x + 1, y] && gridScript.gridArray[x + 1, y].GetComponent<GridStat>().visited == step)
                    return true;
                else
                    return false;
            case 3:
                if (y - 1 > -1 && gridScript.gridArray[x, y - 1] && gridScript.gridArray[x, y - 1].GetComponent<GridStat>().visited == step)
                    return true;
                else
                    return false;
            case 4:
                if (x - 1 > -1 && gridScript.gridArray[x - 1, y] && gridScript.gridArray[x - 1, y].GetComponent<GridStat>().visited == step)
                    return true;
                else
                    return false;

        }
        return false;
    }
    void TestFourDirections(int x, int y, int step)
    {
        if (TestDirection(x, y, -1, 1))
        {
            SetVisited(x, y + 1, step);
        }
        if (TestDirection(x, y, -1, 2))
        {
            SetVisited(x + 1, y, step);
        }
        if (TestDirection(x, y, -1, 3))
        {
            SetVisited(x, y - 1, step);
        }
        if (TestDirection(x, y, -1, 4))
        {
            SetVisited(x - 1, y, step);
        }
    }
    void SetVisited(int x, int y, int step)
    {
        if (gridScript.gridArray[x, y])
            gridScript.gridArray[x, y].GetComponent<GridStat>().visited = step;
    }
    GameObject FindClosest(Transform targetLocation, List<GameObject> list)
    {
        float currentDistance = gridScript.scale * gridScript.rows * gridScript.columns;
        int indexNumber = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (Vector3.Distance(targetLocation.position, list[i].transform.position) < currentDistance)
            {
                currentDistance = Vector3.Distance(targetLocation.position, list[i].transform.position);
                indexNumber = i;
            }
        }
        return list[indexNumber];
    }

}
