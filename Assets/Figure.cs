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
    public Vector2 centerCoord = new Vector2(1, 1);

    public void Set(int[,] matrix, GameObject cube, Vector3 cellSize)
    {
        blockMatrix = matrix;
        cubePrefab = cube;
        figureCellSize = cellSize;
        blocks = new GameObject[blocksCount + 1];

        for (int i = 1; i <= blocksCount; i++)
        {
            GameObject figureCube = Instantiate(cubePrefab, this.transform);
            blocks[i] = figureCube;
            Vector2 blockCoord = GetBlockCoord(i);
            print(i);
            Vector3 position = GetBlockPositionByCoord(blockCoord);
            print(position);
            figureCube.transform.localPosition = GetBlockPositionByCoord(blockCoord);
            figureCube.SetActive(true);
        }
    }

    public void Rotate()
    {
        Debug.Log("Rotate");
    }

    private Vector2 GetBlockCoord(int blockIndex)
    {
        for (int i = 0; i < blockMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < blockMatrix.GetLength(1); j++)
            {
                if (blockMatrix[i, j] == blockIndex)
                {
                    return new Vector2(j, i);
                }
            }
        }

        return new Vector2(-1, -1); // invalid index
    }

    private Vector3 GetBlockPositionByCoord(Vector2 coord)
    {
        Vector3 centerPosition = new Vector3(centerCoord.x * figureCellSize.x, centerCoord.y * figureCellSize.y);
        Vector3 position = new Vector3(figureCellSize.x * coord.x - centerPosition.x, -figureCellSize.y * coord.y - centerPosition.y);
        return position;
    }
}
