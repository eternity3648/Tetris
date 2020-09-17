using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class TetrisGrid : MonoBehaviour
{
    public int sizeX, sizeY;
    public Cell[,] cells;
    public GameObject BorderLinePrefab;
    public GameObject CubePrefab;
    public GameObject figurePrefab;
    public Vector3 cellSize;
    public float startFigureSpeed;

    private Vector3 startPositon;
    private Vector2 currentFigureCoord;
    private GameObject currentFigure;
    private Figure figScript;
    private Cell currentCell;
    private float currentFigureSpeed;

    private enum Sides
    {
        Left = 1,
        Top = 2,
        Right = 3,
        Bottom = 4
    }
    
    public void Start()
    {
        FigureTypes.Init();

        cells = new Cell[sizeX, sizeY];
        startPositon = new Vector3(-cellSize.x * (sizeX / 2), cellSize.y * (sizeX / 2));

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                cells[x, y] = new Cell();
                Vector3 cellCenterPosition = GetCellPosition(new Vector2(x, y));

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

        currentFigureSpeed = startFigureSpeed;
        LaunchStartFigure();
    }

    public void Update()
    {
        float delta = Time.deltaTime;
        if (currentFigure != null && delta < 0.2f)
        {
            Vector3 figurePosition = currentFigure.transform.localPosition;
            figurePosition += new Vector3(0, -currentFigureSpeed * Time.deltaTime, 0);
            currentFigure.transform.localPosition = figurePosition;

            Vector2 coord = GetCellСoordByPosition(figurePosition);
            Cell cell = GetCellByCoord(coord);

            if (CheckIfCellIsFreeForBlock(cell))
            {
                currentFigureCoord = coord;
            }
            else if (currentFigureCoord == new Vector2(1000, 1000))
            {
                Destroy(currentFigure);
                currentFigure = null;
            }
            else
            {
                Cell currentCell = GetCellByCoord(currentFigureCoord);
                if (currentCell.IsFree())
                {
                    currentCell.occupyingCube = currentFigure;
                    currentFigure.transform.localPosition = GetCellPosition(currentFigureCoord);
                    LaunchStartFigure();
                }
                else
                {
                    Destroy(currentFigure);
                    currentFigure = null;
                }
            }
        }
    }

    public void LaunchStartFigure()
    {
        Vector3 startPosition = GetFigureStartPosition();


        int[,] figureMatrix = FigureTypes.GetRangdom();

        currentFigure = Instantiate(figurePrefab, this.transform);
        figScript = currentFigure.GetComponent<Figure>();
        figScript.Set(figureMatrix, CubePrefab, cellSize);


        currentCell = GetCellByPosition(startPosition);
        //print("currentCell");
        //print(GetCellСoordByPosition(startPosition));
        if (CheckIfCellIsFreeForBlock(currentCell))
        {
            currentFigure.transform.localPosition = startPosition;
            currentFigureCoord = new Vector2(1000, 1000);
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

    private Vector3 GetCellPosition(Vector2 coord)
    {
        Vector3 position = startPositon + new Vector3(cellSize.x * (coord.x + 0.5f), -cellSize.y * (coord.y + 0.5f));
        return position;
    }

    private Vector2 GetCellСoordByPosition(Vector3 position) 
    {
        position -= startPositon;
        return new Vector2(Convert.ToInt32((position.x / cellSize.x) - 0.5f), -Convert.ToInt32((position.y / cellSize.y + 0.5f)));
    }

    private Cell GetCellByPosition(Vector3 position)
    {
        return GetCellByCoord(GetCellСoordByPosition(position));
    }

    private Cell GetCellByCoord(Vector2 coord)
    {
        if (coord.x >= sizeX || coord.y >= sizeY) { return null; }
        return cells[Convert.ToInt32(coord.x), Convert.ToInt32(coord.y)];
    }

    private Vector3 GetFigureStartPosition()
    {
        Vector2 startCoord = new Vector2(Math.Abs(sizeX /2) - 2, 0);
        return GetCellPosition(startCoord);
    }

    private bool CheckIfCellIsFreeForBlock(Cell cell)
    {
        return cell != null && cell.IsFree();
    }
}

public class Cell
{
    public GameObject occupyingCube;

    public bool IsFree()
    {
        return (occupyingCube == null);
    }
}
