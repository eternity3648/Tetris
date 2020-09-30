using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    OperateFigure Action;
    bool side;
    public void SetOnMouseDown(OperateFigure action, bool s)
    {
        Action = action;
        side = s;
    }

    void OnMouseDown()
    {
        Action(side);
    }
}
