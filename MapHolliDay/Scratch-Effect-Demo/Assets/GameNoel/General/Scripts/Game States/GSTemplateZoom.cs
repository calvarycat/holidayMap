using System.Collections;
using UnityEngine;

////////////////////////////////////////////////////////
//Author:
//TODO: a game state sample
////////////////////////////////////////////////////////

public class GSTemplateZoom : GSTemplate
{
    protected CanvasGroup MyCanvasGroup;
    protected Vector3 ScaleStart = new Vector3(0.7f, 0.7f, 0.7f);
    protected Vector3 ScaleEnd = new Vector3(0.9f, 0.9f, 0.9f);
    protected float TimeIn = 0.3f;
    protected float TimeOut = 0.2f;

    protected override void Awake()
    {
        base.Awake();
        MyCanvasGroup = GuiMain.GetComponent<CanvasGroup>();
    }

    public override void OnSuspend()
    {
        GameStatesManager.OnBackKey = null;
        FadeOut();
    }

    public override void OnResume()
    {
        base.OnResume();
        FadeIn();
    }

    protected virtual void FadeIn()
    {
        GuiMain.SetActive(true);
        MyCanvasGroup.interactable = false;

        // scale effect
        GuiMain.transform.localScale = ScaleStart;
        iTween.ScaleTo(GuiMain, Vector3.one, TimeIn);

        // alpha effect
        MyCanvasGroup.alpha = 1;
        MyCanvasGroup.interactable = true;
        FadeInFinish();
    }

    protected virtual void FadeOut()
    {
        MyCanvasGroup.interactable = false;
        Hashtable ht = iTween.Hash("from", 1, "to", 0, "time", TimeOut / 2, "onupdate", "UpdateAlphaCanvas",
            "onComplete", "FadeOutFinish");
        iTween.ValueTo(gameObject, ht);
    }

    private void UpdateAlphaCanvas(float value)
    {
        MyCanvasGroup.alpha = value;
    }

    protected virtual void FadeInFinish()
    {
    }

    protected virtual void FadeOutFinish()
    {
        GuiMain.SetActive(false);
    }
}