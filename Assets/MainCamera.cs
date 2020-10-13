using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainCamera : MonoBehaviour
{
    public GameObject tetrisGrid;
    public GameObject nextFigureContainer;

    void Start()
    {
        TweenCallback<GameObject> callb = OnFigureCreate;
        tetrisGrid.GetComponent<TetrisGrid>().SetOnFigureCreate(callb);
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
}
