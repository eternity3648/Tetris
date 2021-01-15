using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainCamera : MonoBehaviour
{
    public GameObject tetrisGrid;
    public GameObject gameScreen;
    public GameObject nextFigureContainer;

    GameObject currFigure;
    Vector3 currFigurePos, startMousePos;

    void Start()
    {
        TetrisGrid gridScript = tetrisGrid.GetComponent<TetrisGrid>();
        TweenCallback<GameObject> callb1 = OnFigureCreate;
        gridScript.SetOnFigureCreate(callb1);
        TweenCallback callb2 = OnFigureFastFall;
        gridScript.SetOnFigureFastFall(callb2);
        startMousePos = new Vector3();
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    startMousePos = Input.mousePosition;

        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    startMousePos = new Vector3();
        //}

        //if (startMousePos != new Vector3())
        //{
        //    Vector3 possDiff = startMousePos - Input.mousePosition;
        //    print(currFigure.GetComponent<Figure>().GetIndex());
        //    print(possDiff);
        //    currFigure.transform.localPosition = currFigurePos - possDiff;
        //}
    }

    void OnFigureCreate(GameObject figure)
    {
        foreach (Transform child in nextFigureContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        Vector2 cellSize = tetrisGrid.GetComponent<TetrisGrid>().cellSize;
        Figure figScript = figure.GetComponent<Figure>();
        int matrixSize = figScript.blockMatrix.GetUpperBound(0) + 1;

        Vector3 positionDiff = (float)matrixSize / 2.0f * cellSize * 1 / 0.35f;
        figure.transform.SetParent(nextFigureContainer.transform);
        figure.transform.localPosition = -positionDiff - FigureTypes.GetPositionShiftByIndex(figScript.GetIndex());
        Vector3 scale = figure.transform.localScale;
        scale *= 0.35f;
        figure.transform.localScale = scale;
        currFigure = figure;
        currFigurePos = -positionDiff;


        //figure.transform.position = nextFigureContainer.transform.position - new Vector3(0.4f * positionDiff.x, -0.27f * positionDiff.y);
    }

    void OnFigureFastFall()
    {
        Sequence sequence = DOTween.Sequence();
        float delay = 0.2f;
        Vector3 pos = gameScreen.transform.position;
        sequence.Join(gameScreen.transform.DOMove(pos + new Vector3(0, -0.2f, 0), delay));
        sequence.Join(gameScreen.transform.DOMove(pos, delay).SetDelay(delay));
    }
}
