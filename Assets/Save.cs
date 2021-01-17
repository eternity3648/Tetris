using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CreateCubeInCell(Vector2 coord, int figureIndex);

[System.Serializable]
public class Save
{
    public int score = 0;
    public CellInfo[,] cells;
    public int figureIndex;
    public int nextFigureIndex;
    public SerializableVector figurePosition;
    // how many times figure was rotated
    public int rotationCount;
    public float figureFallDelay;
    public float figureFallDelayDecreaseCurrentTime;

    [System.Serializable]
    public struct CellInfo
    {
        public bool isFree;
        public int cubeIndex;
    }

    [System.Serializable]
    public struct SerializableVector
    {
        public float x;
        public float y;
    }

    public void SaveGridState(Cell[,] cells1)
    {
        cells = new CellInfo[cells1.GetLength(0), cells1.GetLength(1)];
        for (int x = 0; x < cells1.GetLength(0); x++)
        {
            for (int y = 0; y < cells1.GetLength(1); y++)
            {
                cells[x, y].isFree = cells1[x, y].IsFree();
                cells[x, y].cubeIndex = cells1[x, y].cubeIndex;
            }
        }
    }

    public void SetGridState(CreateCubeInCell Create)
    {
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                CellInfo cellInfo = cells[x, y];
                if (!cellInfo.isFree)
                    Create(new Vector2(x, y), cellInfo.cubeIndex);
            }
        }
    }
}
