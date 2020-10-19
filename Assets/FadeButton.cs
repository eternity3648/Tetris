using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class FadeButton : EventTrigger
{
    private TweenCallback onMouseUp, onMouseDown;
    // Start is called before the first frame update


    public void Set(TweenCallback onMouseDown1, TweenCallback onMouseUp1)
    {
        onMouseUp = onMouseUp1;
        onMouseDown = onMouseDown1;
    }

    // Update is called once per frame


    public override void OnPointerUp(PointerEventData data)
    {
        onMouseUp?.Invoke();
    }
    public override void OnPointerDown(PointerEventData data)
    {
        onMouseDown?.Invoke();
    }
}
