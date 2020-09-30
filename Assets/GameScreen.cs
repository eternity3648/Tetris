using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OperateFigure(bool side);

public class GameScreen : MonoBehaviour
{
    public GameObject grid;
    public GameObject leftButton, rightButton;
    public GameObject clickableBack;
    public float startFastHorizontalMovementDelat;


    private OperateFigure MoveHorizontal;
    private TetrisGrid gridScript;
    private float fastHorizontalMovementDelay;
    private bool rightSidePressed, leftSidePressed;

    // Start is called before the first frame update
    void Start()
    {
        MoveHorizontal = MoveFigure;
        gridScript = grid.GetComponent<TetrisGrid>();
        ResetVariables(true);

        leftButton.GetComponent<Button>().Set(MoveHorizontal, ResetVariables, false);
        rightButton.GetComponent<Button>().Set(MoveHorizontal, ResetVariables, true);
        OperateFigure Back = ResetVariables;
        clickableBack.GetComponent<Button>().Set(null, ResetVariables, true);
    }

    void ResetVariables(bool _)
    {
        print("job");
        fastHorizontalMovementDelay = 0;
        rightSidePressed = false;
        leftSidePressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        if (fastHorizontalMovementDelay > 0) { fastHorizontalMovementDelay -= delta; }

        if (rightSidePressed && fastHorizontalMovementDelay < 0)
        {
            gridScript.MoveFigure(true);
        }
        else if (leftSidePressed && fastHorizontalMovementDelay < 0)
        {
            gridScript.MoveFigure(false);
        }
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
        if (side) { rightSidePressed = true; }
        else { leftSidePressed = true; }
        fastHorizontalMovementDelay = startFastHorizontalMovementDelat;
    }
}
