using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    OperateFigure DownAction, UpAction;
    bool side;

    public void Set(OperateFigure downAction, OperateFigure upAction, bool s)
    {
        DownAction = downAction;
        UpAction = upAction;
        side = s;
    }

    void OnMouseDown()
    {
        DownAction?.Invoke(side);
    }

    void OnMouseUp()
    {
        UpAction?.Invoke(side);
    }
}
