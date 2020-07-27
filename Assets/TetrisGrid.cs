using System;
using UnityEngine;

public class TetrisGrid
{
    public Cell[,] cells;

    public TetrisGrid(int sizeX, int sizeY)
    {
        cells = new Cell[sizeX, sizeY];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                cells[x, y] = new Cell();
            }
        }
    }
}

public class Cell
{
    public GameObject occupyingCube;
}
