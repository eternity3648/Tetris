using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public delegate void OperateFigure(bool side);

public class GameScreen : MonoBehaviour
{
    public GameObject grid;
    public Swipe swipeControls;
    public GameObject leftButton, rightButton, downButton, rotateButton;
    public GameObject restartButton;
    public Text scoreText;
    public float startFastHorizontalMovementDelat;
    public float nativeAspectRatio;
    public float horizontalDragSpeed;
    public float verticalDragSpeed;
    public float superAccelerationDragSpeed;
    public int[] pointsForDestroyingLines = new int[5];

    private TetrisGrid gridScript;
    private float fastHorizontalMovementDelay;
    private bool rightSidePressed, leftSidePressed;
    private Vector3 mouseMoved;
    private Vector3 lastMousePosition;
    private bool mousePressed = false;
    private bool wasFigureMoved = false;
    private bool wasFigureAcceleratedVertically = false;
    private Tweener scoreTweener;
    private int score = 0;
    private int scoreForTween = 0;
    private int transitionValueForTween = 0;

    // Start is called before the first frame update
    void Start()
    {
        float aspectRatio = (Screen.width * 1.0f) / Screen.height;
        float scale = aspectRatio / nativeAspectRatio;
        this.transform.localScale = new Vector3(scale, scale);
        Vector3 lastMousePosition = Input.mousePosition;
        mouseMoved = new Vector3();

        gridScript = grid.GetComponent<TetrisGrid>();
        TweenCallback<int> callb = OnLineDestroy;
        gridScript.SetOnLinesDestroy(callb);
        ResetVariables(true);

        if (Application.platform == RuntimePlatform.Android)
        {
            horizontalDragSpeed *= 2.8f;
            verticalDragSpeed *= 2.8f;
            superAccelerationDragSpeed *= 2.8f;
        }

        //leftButton.GetComponent<Button>().Set(MoveFigure, ResetVariables, false);
        //rightButton.GetComponent<Button>().Set(MoveFigure, ResetVariables, true);
        ////downButton.GetComponent<Button>().Set(SetFigureSpeed, ResetVariables, true);
        //rotateButton.GetComponent<Button>().Set(null, RotateFigure, true);
        //restartButton.GetComponent<Button>().Set(null, Restart, true);
        ////clickableBack.GetComponent<Button>().Set(null, ResetVariables, true);
    }

    void ResetVariables(bool _)
    {
        fastHorizontalMovementDelay = 0;
        rightSidePressed = false;
        leftSidePressed = false;
        SetFigureSpeed(1);
        score = 0;
        scoreForTween = 0;
        OnLineDestroy(0);
        //print("ResetVariables");
    }

    void OnLineDestroy(int lineCount)
    {
        if (scoreTweener != null)  scoreTweener.Kill(false); 
        score += pointsForDestroyingLines[lineCount];
        scoreTweener = DOTween.To(x => scoreForTween = (int)x, scoreForTween, score, 1.5f)
                               .SetEase(Ease.OutCubic)
                               .SetAutoKill(false);
        scoreTweener.OnUpdate(() => scoreText.text = scoreForTween.ToString());
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

            Vector3 posDiff = Input.mousePosition - lastMousePosition;
            if (!wasFigureMoved) RotateFigure(true);
            wasFigureMoved = false;
            wasFigureAcceleratedVertically = false;
        }

        if (mousePressed)
        {
            Vector3 posDiff = Input.mousePosition - lastMousePosition;
            if (posDiff.magnitude < 1) { posDiff = new Vector3(); }
            mouseMoved += posDiff;

            if (Mathf.Abs(mouseMoved.x) >= horizontalDragSpeed && !wasFigureAcceleratedVertically)
            {
                wasFigureMoved = true;
                int turnsCount = 0;
                bool turnSide = true;

                if (mouseMoved.x > 0)
                {
                    turnsCount = (int)Mathf.Round(mouseMoved.x / horizontalDragSpeed);
                    mouseMoved.x -= horizontalDragSpeed * turnsCount;
                }
                else
                {
                    turnSide = false;
                    turnsCount = (int)Mathf.Round(Mathf.Abs(mouseMoved.x) / horizontalDragSpeed);
                    mouseMoved.x += horizontalDragSpeed * turnsCount;
                }

                for (int i = 0; i < turnsCount; i++)
                {
                    gridScript.MoveFigure(turnSide);
                }
            }

            if (posDiff.y < -superAccelerationDragSpeed && !wasFigureAcceleratedVertically)
            {
                wasFigureMoved = true;
                wasFigureAcceleratedVertically = true;
                SetFigureSpeed(3);
            }

            if (mouseMoved.y <= -verticalDragSpeed && gridScript.CanFigureSpeedBeChanged() && !wasFigureAcceleratedVertically)
            {
                wasFigureMoved = true;
                wasFigureAcceleratedVertically = true;
                SetFigureSpeed(2);
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
