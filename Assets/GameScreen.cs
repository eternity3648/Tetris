using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : MonoBehaviour
{
    public TetrisGrid gameGrid;
    public GameObject cube;

    private Vector3 cubePosition = new Vector3(0, 0, 0);
    private int cubeSpeed = 3;

    // Start is called before the first frame update
    void Start()
    {
        gameGrid = new TetrisGrid(10, 20);
    }

    // Update is called once per frame
    void Update()
    {
        cubePosition = new Vector3(0, cubePosition.y - cubeSpeed * Time.deltaTime, 0);
        cube.transform.position = cubePosition;
    }
}
