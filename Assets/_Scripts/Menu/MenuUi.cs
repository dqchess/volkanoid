﻿using System.Collections;
using System.Collections.Generic;
using Resources = SRResources.Menu.Ui;
using UnityEngine;
using RSG;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class MenuUi : MonoBehaviour
{

    public MenuUi Initialize(WorldSave[] worldSaves)
    {
        InitializeBackground()
        .InitializeCanvasGroup()
        .InitializeLevelSelector(worldSaves)
        .InitializeButtons();
        return this;
    }

    public IPromise<string> WaitForNextLevel()
    {
        return this.levelSelector.WaitForNextLevel();
    }

    public IPromise ZoomIn(float time = 1)
    {
        //hack to zoom we have to get all first depth child into a gameobject and scale that
        GameObject zoomableObject = new GameObject("ZoomableObject");
        zoomableObject.AddComponent<CanvasGroup>();
        zoomableObject.transform.position = new Vector2(0, -300);
        zoomableObject.transform.SetParent(this.transform, false);
        Transform[] firstdepthChildren = GetComponentsInChildren<Transform>()
                                         .filter(childTransfrom => childTransfrom.parent == this.transform)                                            
                                         .map(childTransform =>
                                         {
                                             childTransform.SetParent(zoomableObject.transform, true);
                                             return childTransform;
                                         })
                                         .ToArray();
        return new Promise((resolve, reject) =>
        {
            zoomableObject.transform.DOScale(new Vector2(6, 6), time)
                                    .OnComplete(() => resolve());
        });
    }

    public IPromise EnterAnimation()
    {
        return Promise.All
        (
             new Promise((resolve, reject) =>
             {
                 RectTransform rectTransform = this.closeGame.GetComponent<RectTransform>();
                 Sequence mySequence = DOTween.Sequence();
                 rectTransform.DOMoveY(600, 1f, false)
                 .From()
                 .OnComplete(() => resolve());
             }),
             new Promise((resolve, reject) =>
              {
                  RectTransform rectTransform = this.openLeaderboard.GetComponent<RectTransform>();
                  Sequence mySequence = DOTween.Sequence();
                  rectTransform.DOMoveY(600, 1f, false)
                  .From()
                  .OnComplete(() => resolve());
             }),
             this.levelSelector.EnterAnimation()
        );
    }

    public MenuUi MakeNonInteractable()
    {
        this.canvasGroup.interactable = false;
        return this;
    }

    public MenuUi MakeInteractable()
    {
        this.canvasGroup.interactable = true;
        return this;
    }

    private MenuUi InitializeBackground()
    {
        GameObject background = Resources.Background.Instantiate();
        background.name = "Background";
        background.transform.SetParent(this.gameObject.transform, false);
        return this;
    }

    private MenuUi InitializeLevelSelector(WorldSave[] worldSaves)
    {
        this.levelSelector = Resources.LevelSelector.Instantiate().GetComponent<LevelSelector>();
        this.levelSelector.Initialize(worldSaves);
        this.levelSelector.name = "LevelSelector";
        this.levelSelector.transform.SetParent(this.gameObject.transform, false);
        return this;
    }

    public MenuUi InitializeCanvasGroup()
    {
        this.canvasGroup = GetComponent<CanvasGroup>();
        return this;
    }

    private MenuUi InitializeButtons()
    {
        this.closeGame = Resources.CloseGame.Instantiate().GetComponent<Button>();
        this.closeGame.transform.SetParent(this.gameObject.transform, false);
        this.openLeaderboard = Resources.OpenLeaderboard.Instantiate().GetComponent<Button>(); ;
        this.openLeaderboard.transform.SetParent(this.gameObject.transform, false);
        return this;
    }

    LevelSelector levelSelector;
    private CanvasGroup canvasGroup;
    Button closeGame;
    Button openLeaderboard;
}
