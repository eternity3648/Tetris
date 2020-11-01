using UnityEngine;

public class Button1 : MonoBehaviour
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
