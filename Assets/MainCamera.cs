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
        figure.transform.parent = null;
        figure.transform.localPosition = new Vector3();
        //figure.transform.position = new Vector3();
        figure.transform.parent = nextFigureContainer.transform;
        //figure.transform.localPosition = new Vector3();
        Vector3 scale = figure.transform.localScale;
        scale *= 0.65f;
        figure.transform.localScale = scale;

        Vector2 cellSize = tetrisGrid.GetComponent<TetrisGrid>().cellSize;
        int matrixSize = figure.GetComponent<Figure>().blockMatrix.GetUpperBound(0) + 1;
        Vector3 positionDiff = (float)matrixSize / 2.0f * cellSize;
        figure.transform.position = nextFigureContainer.transform.position - new Vector3(0.4f * positionDiff.x, -0.27f * positionDiff.y);
    }
}
