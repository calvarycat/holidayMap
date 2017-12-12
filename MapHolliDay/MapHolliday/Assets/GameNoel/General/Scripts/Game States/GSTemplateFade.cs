using System.Collections;
using UnityEngine;

////////////////////////////////////////////////////////
//Author:
//TODO: a game state sample
////////////////////////////////////////////////////////

public class GSTemplateFade : GSTemplate
{
    private CanvasGroup canvasGroup;
    protected float timeIn = 0.2f;
    protected float timeOut = 0.2f;

    protected override void Awake()
    {
        base.Awake();
        canvasGroup = GuiMain.GetComponent<CanvasGroup>();
    }

    public override void OnSuspend()
    {
        GameStatesManager.OnBackKey = null;
        if (canvasGroup != null)
        {
            FadeOut();
        }
        else
        {
            GuiMain.SetActive(false);
        }
    }

    public override void OnResume()
    {
        base.OnResume();
        if (canvasGroup != null)
        {
            FadeIn();
        }
    }

    private void FadeIn()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        Hashtable ht = iTween.Hash("from", 0, "to", 1, "time", timeIn, "onupdate", "UpdateAlphaCanvas", "onComplete",
            "FadeInFinish");
        iTween.ValueTo(gameObject, ht);
    }

    private void FadeOut()
    {
        canvasGroup.interactable = false;
        Hashtable ht = iTween.Hash("from", 1, "to", 0, "time", timeOut, "onupdate", "UpdateAlphaCanvas", "onComplete",
            "FadeOutFinish");
        iTween.ValueTo(gameObject, ht);
    }

    private void UpdateAlphaCanvas(float value)
    {
        canvasGroup.alpha = value;
    }

    protected virtual void FadeInFinish()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
    }

    protected virtual void FadeOutFinish()
    {
        GuiMain.SetActive(false);
    }
}