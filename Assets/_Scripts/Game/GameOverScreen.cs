﻿using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviourEx
{

    public GameOverScreen Initialize(Action restart)
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();
        bool[] activated = transforms.Select(currentTransform =>
        {
            if (currentTransform.name == "Restart")
            {
                this.tryAgain = currentTransform.gameObject.GetComponent<Button>();
                this.tryAgain.onClick.AddListener(() =>
                {
                    SoundData playRestart = new SoundData(GetInstanceID(), SRResources.Audio.Effects.Confirm);
                    Messenger.Publish(new PlayEffectMessage(playRestart));
                    restart();
                });
                return true;
            }
            if (currentTransform.name == "Title")
            {
                this.title = currentTransform.gameObject;
                return true;
            }
            if (currentTransform.name == "GameOverScreen")
            {
                this.background = currentTransform.gameObject;
                return true;
            }
            return false;
        }).ToArray();
        int activatedAmount = activated.Where(element => element).Count();
        if (activatedAmount != 3)
        {
            Debug.LogError("Cound not find proper amount of elements");
        }
        return this;
    }

    public GameOverScreen Show()
    {
        this.background.GetComponent<Image>().enabled = true;
        this.tryAgain.gameObject.SetActive(true);
        this.title.SetActive(true);

        this.tryAgain.interactable = false;

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(this.tryAgain.transform.DOScale(0, 1.2f)
            .From()
            .SetEase(Ease.OutElastic, 0.4f));
        mySequence.Insert(0, this.title.transform.DOScale(0, 1.2f)
            .From()
            .SetEase(Ease.OutElastic, 0.4f));     
        mySequence.OnComplete(() => this.tryAgain.interactable = true);
        return this;
    }

    public GameOverScreen Hide()
    {
        this.background.GetComponent<Image>().enabled = false;
        this.tryAgain.gameObject.SetActive(false);
        this.title.SetActive(false);
        return this;
    }

    private Button tryAgain;
    private GameObject title;
    private GameObject background;
}
