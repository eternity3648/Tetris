using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OperateFigure(bool side);

public class GameScreen : MonoBehaviour
{
    public GameObject grid;
    public GameObject leftButton, rightButton;
    public float startFastHorizontalMovementDelat;


    private OperateFigure MoveHorizontal;
    private TetrisGrid gridScript;
    private float fastHorizontalMovementDelay;

    // Start is called before the first frame update
    void Start()
    {
        MoveHorizontal = MoveFigure;
        gridScript = grid.GetComponent<TetrisGrid>();
        fastHorizontalMovementDelay = 0;

        leftButton.GetComponent<Button>().SetOnMouseDown(MoveHorizontal, false);
        rightButton.GetComponent<Button>().SetOnMouseDown(MoveHorizontal, true);
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        if (fastHorizontalMovementDelay > 0) { fastHorizontalMovementDelay -= delta; }

        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    MoveFigure(true);
        //    fastHorizontalMovementDelay = startFastHorizontalMovementDelat;
        //}
        //else if (Input.GetKey(KeyCode.RightArrow) && fastHorizontalMovementDelay < 0)
        //{
        //    MoveFigure(true);
        //}
        //else if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    MoveFigure(false);
        //    fastHorizontalMovementDelay = startFastHorizontalMovementDelat;
        //}
        //else if (Input.GetKey(KeyCode.LeftArrow) && fastHorizontalMovementDelay < 0)
        //{
        //    MoveFigure(false);
        //}
        //else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        //{
        //    fastHorizontalMovementDelay = 0;
        //}
        //else if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    currentFigureSpeed = fastFigureSpeed;
        //}
        //else if (Input.GetKeyUp(KeyCode.DownArrow))
        //{
        //    currentFigureSpeed = slowFigureSpeed;
        //}
        //else if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    RotateFigure();
        //}
    }

    void MoveFigure(bool side)
    {
        gridScript.MoveFigure(side);
    }
}
