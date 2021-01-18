using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figure : MonoBehaviour
{
    public int[,] blockMatrix;
    public GameObject cubePrefab;
    public Vector3 figureCellSize;
    public const int blocksCount = 4;
    public GameObject[] blocks;
    public Vector2 centerCoord = new Vector2(0, 0);
    public Vector3 centerPosition;
    public int index;
    public int rotationCount = 0;

    public void Set(int[,] matrix, GameObject cube, Vector3 cellSize, int index1)
    {
        centerPosition = -figureCellSize;
        blockMatrix = matrix;
        cubePrefab = cube;
        figureCellSize = cellSize;
        blocks = new GameObject[blocksCount + 1];
        index = index1;
        Render();
    }

    public void Rotate()
    {
        int[,] newMatrix = new int[blockMatrix.GetLength(1), blockMatrix.GetLength(0)];
        int newColumn, newRow = 0;

        //if (direction)
        //{
            for (int oldColumn = blockMatrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
            {
                newColumn = 0;
                for (int oldRow = 0; oldRow < blockMatrix.GetLength(0); oldRow++)
                {
                    newMatrix[newRow, newColumn] = blockMatrix[oldRow, oldColumn];
                    newColumn++;
                }
                newRow++;
            }
        //} 
        //else
        //{
        //    for (int oldColumn = 0; oldColumn < blockMatrix.GetLength(0) - 1; oldColumn++)
        //    {
        //        newColumn = 0;
        //        for (int oldRow = blockMatrix.GetLength(1); oldRow >= 0; oldRow--)
        //        {
        //            newMatrix[newRow, newColumn] = blockMatrix[oldRow, oldColumn];
        //            newColumn++;
        //        }
        //        newRow++;
        //    }
        //}

        blockMatrix = newMatrix;
        Render();

        rotationCount++;
        if (rotationCount > 3) rotationCount = 0;
    }

    public List<Vector2> GetBlockCoordsRelativeToCoord(Vector2 coord)
    {
        List<Vector2> coords = new List<Vector2>();
        for (int i = 1; i <= blocksCount; i++)
        {
            coords.Add(coord + GetBlockCoord(i));
        }

        return coords;
    }

    public int [,] GetMatrix()
    {
        return blockMatrix;
    }
    public int GetIndex()
    {
        return index;
    }

    public Vector2 GetBlockCoord(int blockIndex)
    {
        for (int i = 0; i < blockMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < blockMatrix.GetLength(1); j++)
            {
                if (blockMatrix[i, j] == blockIndex)
                {
                    //print("Coord");
                    //print(new Vector2(i, j));
                    return new Vector2(i, j);
                }
            }
        }

        return new Vector2(-1, -1); // invalid index
    }

    private void Render()
    {
        for (int i = 1; i <= blocksCount; i++)
        {
            if (blocks[i] != null) 
            {
                Destroy(blocks[i]);
                blocks[i] = null;
            }

            GameObject figureCube = Instantiate(cubePrefab, this.transform);
            blocks[i] = figureCube;
            Vector2 blockCoord = GetBlockCoord(i);
            Vector3 position = GetBlockPositionByCoord(blockCoord);
            figureCube.transform.localPosition = GetBlockPositionByCoord(blockCoord);
            figureCube.SetActive(true);
            Color color = FigureTypes.GetСolor(index);
            figureCube.GetComponent<SpriteRenderer>().color = color;
            figureCube.GetComponent<SpriteRenderer>().material.color = color;
        }
    }

    private Vector3 GetBlockPositionByCoord(Vector2 coord)
    {
        Vector3 position = new Vector3(figureCellSize.x * coord.x - centerPosition.x, -figureCellSize.y * coord.y - centerPosition.y);
        return position;
    }
}
