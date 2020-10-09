using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public delegate void OperateFigure(bool side);

public class GameScreen : MonoBehaviour
{
    public GameObject grid;
    public Swipe swipeControls;
    public GameObject leftButton, rightButton, downButton, rotateButton;
    public GameObject restartButton;
    public float startFastHorizontalMovementDelat;
    public float nativeAspectRatio;
    public float horizontalDragSpeed;

    private TetrisGrid gridScript;
    private float fastHorizontalMovementDelay;
    private bool rightSidePressed, leftSidePressed;
    private Vector3 mouseMoved;
    private Vector3 lastMousePosition;
    private bool mousePressed = false;

    // Start is called before the first frame update
    void Start()
    {
        float aspectRatio = (Screen.width * 1.0f) / Screen.height;
        float scale = aspectRatio / nativeAspectRatio;
        this.transform.localScale = new Vector3(scale, scale);
        Vector3 lastMousePosition = Input.mousePosition;
        horizontalDragSpeed = 10;
        mouseMoved = new Vector3();

        gridScript = grid.GetComponent<TetrisGrid>();
        ResetVariables(true);

        leftButton.GetComponent<Button>().Set(MoveFigure, ResetVariables, false);
        rightButton.GetComponent<Button>().Set(MoveFigure, ResetVariables, true);
        //downButton.GetComponent<Button>().Set(SetFigureSpeed, ResetVariables, true);
        rotateButton.GetComponent<Button>().Set(null, RotateFigure, true);
        restartButton.GetComponent<Button>().Set(null, Restart, true);
        //clickableBack.GetComponent<Button>().Set(null, ResetVariables, true);
    }

    void ResetVariables(bool _)
    {
        fastHorizontalMovementDelay = 0;
        rightSidePressed = false;
        leftSidePressed = false;
        SetFigureSpeed(1);
        //print("ResetVariables");
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

        if (Input.GetMouseButtonDown(0))
        {
            mousePressed = true;
            lastMousePosition = Input.mousePosition;

        } 
        else if (Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
            mouseMoved = new Vector3();
            if (gridScript.CanFigureSpeedBeChanged())
                SetFigureSpeed(1);
        }

        if (mousePressed)
        {
            Vector3 posDiff = Input.mousePosition - lastMousePosition;
            mouseMoved += posDiff;

            if (Mathf.Abs(mouseMoved.x) >= 40)
            {
                int turnsCount = 0;
                bool turnSide = true;

                if (mouseMoved.x > 0)
                {
                    turnsCount = (int)Mathf.Round(mouseMoved.x / 40);
                    mouseMoved.x -= 40 * turnsCount;
                }
                else
                {
                    turnSide = false;
                    turnsCount = (int)Mathf.Round(Mathf.Abs(mouseMoved.x) / 40);
                    mouseMoved.x += 40 * turnsCount;
                }

                for (int i = 0; i < turnsCount; i++)
                {
                    gridScript.MoveFigure(turnSide);
                }
            }

            if (mouseMoved.y <= -40 && gridScript.CanFigureSpeedBeChanged())
            {
                SetFigureSpeed(2);
            }

            print(posDiff.y);
            if (posDiff.y < -15)
            {
                SetFigureSpeed(3);
            }

            lastMousePosition = Input.mousePosition;
        }

        //if ((Input.mousePosition - lastMousePosition).magnitude >

        //Debug.Log("Yeah");
        //if (swipeControls.SwipeLeft)
        //    Debug.Log("Left");
        //if (swipeControls.SwipeRight)
        //    Debug.Log("Right");
        //if (swipeControls.SwipeUp)
        //    Debug.Log("Up");
        //if (swipeControls.SwipeDown)
        //    Debug.Log("Down");
    }

    void MoveFigure(bool side)
    {
        gridScript.MoveFigure(side);
        if (side) { rightSidePressed = true; }
        else { leftSidePressed = true; }
        fastHorizontalMovementDelay = startFastHorizontalMovementDelat;
    }

    void SetFigureSpeed(int speed)
    {
        gridScript.SetFigureSpeed(speed);
    }

    void RotateFigure(bool _)
    {
        gridScript.RotateFigure();
    }

    void Restart(bool _)
    {
        gridScript.Start();
    }
}
