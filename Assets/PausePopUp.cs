﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PausePopUp : MonoBehaviour
{
    public TweenCallback OnNewGameButtonClick, OnContinueButtonClick, OnMainMenuButtonClick;

    public void Start(TweenCallback OnNewGameButtonClick1, TweenCallback OnContinueButtonClick1, TweenCallback OnMainMenuButtonClick1)
    {
        OnNewGameButtonClick = OnNewGameButtonClick1;
        OnContinueButtonClick = OnContinueButtonClick1;
        OnMainMenuButtonClick = OnMainMenuButtonClick1;

        Vector3 scale = this.transform.localScale;
        this.transform.localScale = new Vector3(scale.x * 0.3f, scale.x * 0.3f);
        this.transform.DOScale(scale.x, 0.5f).SetEase(Ease.OutQuad);

        SpriteRenderer rend = this.GetComponent<SpriteRenderer>();
        Color color = rend.color;
        color.a = 0;
        rend.DOColor(color, 2f);
    }


    public void OnNewGameButtonClickAction()
    {
        OnNewGameButtonClick();
    }

    public void OnContinueButtonClickAction()
    {
        OnContinueButtonClick();
    }

    public void OnMainMenuButtonClickAction()
    {
        OnMainMenuButtonClick();
    }
}
