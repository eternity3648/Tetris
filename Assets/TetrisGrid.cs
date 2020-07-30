using System;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    public int sizeX, sizeY;
    public Cell[,] cells;
    public GameObject BorderLinePrefab;
    public Vector3 cellSize;
    private enum Sides
    {
        Left = 1,
        Top = 2,
        Right = 3,
        Bottom = 4
    }

    public void Start()
    {
        cells = new Cell[sizeX, sizeY];

        Vector3 startPositon = new Vector3(-cellSize.x * (sizeX / 2), cellSize.y * (sizeX / 2));

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                cells[x, y] = new Cell();
                Vector3 cellCenterPosition = startPositon + new Vector3(cellSize.x * (x + 0.5f), - cellSize.y * (y + 0.5f));

                if (x == sizeX - 1)
                {
                    DrawBorder(Sides.Right, cellCenterPosition);
                }
                if (y == 0)
                {
                    DrawBorder(Sides.Top, cellCenterPosition);
                }
                if (x == 0)
                {
                    DrawBorder(Sides.Left, cellCenterPosition);
                }
                if (y == sizeY - 1)
                {
                    DrawBorder(Sides.Bottom, cellCenterPosition);
                }
            }
        }
    }

    private void DrawBorder(Sides side, Vector3 centerPosition)
    {
        GameObject borderLine = Instantiate(BorderLinePrefab, this.transform);
        Transform transform = borderLine.transform;
        Vector3 borderPosition = new Vector3();
        Vector3 angles = transform.rotation.eulerAngles;

        if (side == Sides.Top)
        {
            borderPosition = centerPosition + new Vector3(0, cellSize.y * 0.5f);
        } 
        else if (side == Sides.Right)
        {
            borderPosition = centerPosition + new Vector3(cellSize.x * 0.5f, 0);
            angles.z += 90;
        }
        else if (side == Sides.Bottom)
        {
            borderPosition = centerPosition - new Vector3(0, cellSize.y * 0.5f);
        }
        else if (side == Sides.Left)
        {
            borderPosition = centerPosition - new Vector3(cellSize.x * 0.5f, 0);
            angles.z += 90;
        }

        transform.rotation = Quaternion.Euler(angles);
        transform.localPosition = borderPosition;
        borderLine.SetActive(true);
    }
}

public class Cell
{
    public GameObject occupyingCube;
}
