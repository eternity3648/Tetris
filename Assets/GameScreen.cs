using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public delegate void OperateFigure(bool side);

public class GameScreen : MonoBehaviour
{
    public GameObject grid;
    public GameObject leftButton, rightButton, downButton, rotateButton;
    public GameObject restartButton;
    public float startFastHorizontalMovementDelat;
    public float nativeAspectRatio;

    private TetrisGrid gridScript;
    private float fastHorizontalMovementDelay;
    private bool rightSidePressed, leftSidePressed;

    // Start is called before the first frame update
    void Start()
    {
        float aspectRatio = (Screen.width * 1.0f) / Screen.height;
        float scale = aspectRatio / nativeAspectRatio;
        this.transform.localScale = new Vector3(scale, scale);

        gridScript = grid.GetComponent<TetrisGrid>();
        ResetVariables(true);

        leftButton.GetComponent<Button>().Set(MoveFigure, ResetVariables, false);
        rightButton.GetComponent<Button>().Set(MoveFigure, ResetVariables, true);
        downButton.GetComponent<Button>().Set(SetFigureSpeed, ResetVariables, true);
        rotateButton.GetComponent<Button>().Set(null, RotateFigure, true);
        restartButton.GetComponent<Button>().Set(null, Restart, true);
        //clickableBack.GetComponent<Button>().Set(null, ResetVariables, true);

        //leftButton.transform.DOScale(new Vector3(0, 0), 5);
    }

    void ResetVariables(bool _)
    {
        fastHorizontalMovementDelay = 0;
        rightSidePressed = false;
        leftSidePressed = false;
        SetFigureSpeed(false);
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
    }

    void MoveFigure(bool side)
    {
        gridScript.MoveFigure(side);
        if (side) { rightSidePressed = true; }
        else { leftSidePressed = true; }
        fastHorizontalMovementDelay = startFastHorizontalMovementDelat;
    }

    void SetFigureSpeed(bool speed)
    {
        print("SetFigureSpeed");
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
