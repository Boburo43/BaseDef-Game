using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using TMPro;

public class GridBehaviour : MonoBehaviour
{
    public int rows = 10;
    public int columns = 10;
    public int scale = 1;
    public GameObject gridPrefab;
    public TextMeshPro label;
    public Vector3 leftBottomLocation = new Vector3(0,0,0);
    public GameObject[,] gridArray;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gridArray = new GameObject[columns, rows];
        if (gridPrefab)
        {
            GenerateGrid();
        }
        else print("missing gridprefab, please assign.");
    }

    // Update is called once per frame
    void Update()
    {
    }
    void GenerateGrid()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject obj = Instantiate(gridPrefab, new Vector3(leftBottomLocation.x + scale * i, leftBottomLocation.y, leftBottomLocation.z+scale*j), Quaternion.identity);
                obj.transform.SetParent(gameObject.transform);
                obj.GetComponent<GridStat>().x = i;
                obj.GetComponent<GridStat>().y = j;
                gridArray[i, j] = obj;
                obj.name = (i,j).ToString();
                label = obj.GetComponentInChildren<TextMeshPro>();
                label.text = obj.name;
            }
        }
    }
}
