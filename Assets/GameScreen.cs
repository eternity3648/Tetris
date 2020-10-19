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
    public GameObject leftButton, rightButton, downButton, rotateButton, pausePopUp, fadeButton;
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
    float scale;

    // Start is called before the first frame update
    void Start()
    {
        float aspectRatio = (Screen.width * 1.0f) / Screen.height;
        scale = aspectRatio / nativeAspectRatio;
        Vector3 localScale = this.transform.localScale;
        //this.transform.localScale = new Vector3(scale * localScale.x, scale * localScale.y);
        Vector3 lastMousePosition = Input.mousePosition;
        mouseMoved = new Vector3();

        gridScript = grid.GetComponent<TetrisGrid>();
        TweenCallback<int> callb = OnLineDestroy;
        gridScript.SetOnLinesDestroy(callb);
        ResetVariables(true);

        TweenCallback OnMouseUp1 = OnMouseUp;
        TweenCallback OnMouseDown1 = OnMouseDown;
        fadeButton.GetComponent<FadeButton>().Set(OnMouseDown1, OnMouseUp1);

        if (Application.platform == RuntimePlatform.Android)
        {
            horizontalDragSpeed *= 2.5f;
            verticalDragSpeed *= 2.5f;
            superAccelerationDragSpeed *= 2.5f;
        }
    }

    void ResetVariables(bool _)
    {
        fastHorizontalMovementDelay = 0;
        rightSidePressed = false;
        leftSidePressed = false;
        SetFigureSpeed(1);

        if (scoreTweener != null) scoreTweener.Kill(true);
        scoreText.text = "0";
        score = 0;
        scoreForTween = 0;
    }

    void OnLineDestroy(int lineCount)
    {
        if (lineCount > 0)
        {
            if (scoreTweener != null) scoreTweener.Kill(true);
            score += pointsForDestroyingLines[lineCount];
            scoreTweener = DOTween.To(x => scoreForTween = (int)x, scoreForTween, score, 1.5f)
                                   .SetEase(Ease.OutCubic)
                                   .SetAutoKill(false);
            scoreTweener.OnUpdate(() => scoreText.text = scoreForTween.ToString());
        }
    }

    private void OnMouseDown()
    {
        mousePressed = true;
        lastMousePosition = Input.mousePosition;
        //print("Down");
    }

    private void OnMouseUp()
    {
        mousePressed = false;
        mouseMoved = new Vector3();
        if (gridScript.CanFigureSpeedBeChanged())
            SetFigureSpeed(1);

        Vector3 posDiff = Input.mousePosition - lastMousePosition;
        if (!wasFigureMoved) RotateFigure(true);
        wasFigureMoved = false;
        wasFigureAcceleratedVertically = false;
        //print("UPPPP");
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

        if (mousePressed)
        {
            Vector3 posDiff = Input.mousePosition - lastMousePosition;
            if (posDiff.magnitude < 1) { posDiff = new Vector3(); }
            mouseMoved += posDiff;

            if (Mathf.Abs(mouseMoved.x) >= horizontalDragSpeed)
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

                if (gridScript.CanFigureSpeedBeChanged()) 
                {
                    SetFigureSpeed(1);
                    wasFigureAcceleratedVertically = false;
                    mouseMoved.y = 0;
                }

            }
            else if (posDiff.y < -superAccelerationDragSpeed && !wasFigureAcceleratedVertically)
            {
                wasFigureMoved = true;
                wasFigureAcceleratedVertically = true;
                SetFigureSpeed(3);
            }
            else if (mouseMoved.y <= -verticalDragSpeed && gridScript.CanFigureSpeedBeChanged() && !wasFigureAcceleratedVertically)
            {
                wasFigureMoved = true;
                wasFigureAcceleratedVertically = true;
                SetFigureSpeed(2);
            }

            lastMousePosition = Input.mousePosition;
        }
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

    void Restart()
    {
        ResetVariables(true);
        gridScript.Start();
        pausePopUp.SetActive(false);
    }

    public void StartPausePopUp()
    {
        PausePopUp pauseScript = pausePopUp.GetComponent<PausePopUp>();
        TweenCallback callb = Restart;
        pausePopUp.SetActive(true);
        pauseScript.Start(callb, callb, callb);
        gridScript.SetPause(true);
    }

    public void ClickOnFade()
    {
       
    }
}
