using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainCamera : MonoBehaviour
{
    public GameObject tetrisGrid;
    public GameObject gameScreen;
    public GameObject nextFigureContainer;

    void Start()
    {
        TetrisGrid gridScript = tetrisGrid.GetComponent<TetrisGrid>();
        TweenCallback<GameObject> callb1 = OnFigureCreate;
        gridScript.SetOnFigureCreate(callb1);
        TweenCallback callb2 = OnFigureFastFall;
        gridScript.SetOnFigureFastFall(callb2);
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
        figure.transform.parent = nextFigureContainer.transform;
        figure.transform.localPosition = - positionDiff - FigureTypes.GetPositionShiftByIndex(figScript.GetIndex());
        Vector3 scale = figure.transform.localScale;
        scale *= 0.35f;
        figure.transform.localScale = scale;


        //figure.transform.position = nextFigureContainer.transform.position - new Vector3(0.4f * positionDiff.x, -0.27f * positionDiff.y);
    }

    void OnFigureFastFall()
    {
        Sequence sequence = DOTween.Sequence();
        float delay = 0.2f;
        sequence.Join(gameScreen.transform.DOMove(new Vector3(0, -0.2f, 0), delay));
        sequence.Join(gameScreen.transform.DOMove(new Vector3(0, 0, 0), delay).SetDelay(delay));
    }
}
