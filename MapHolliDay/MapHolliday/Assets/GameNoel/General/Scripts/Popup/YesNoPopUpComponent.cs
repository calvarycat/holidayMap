using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class YesNoPopUpComponent : Popup
{
    public Text MessageText;
    public Text YesButtonText;
    public Text NoButtonText;
    public RectTransform Background;

    private Action _yesCallback;
    private Action _noCallback;
    private string _message;
    private string _yesButonName;
    private string _noButtonName;

    public void Init(string message,
        Action yesCallback = null,
        Action noCallBack = null,
        string yesButonName = "YES",
        string noButtonName = "NO")
    {
        _message = message;
        _noButtonName = noButtonName;
        _yesButonName = yesButonName;
        _yesCallback = yesCallback;
        _noCallback = noCallBack;

        MessageText.text = _message;
        YesButtonText.text = _yesButonName;
        NoButtonText.text = _noButtonName;
        MessageText.text = _message;

        float height = MessageText.preferredHeight;
        Background.sizeDelta = new Vector2(Background.sizeDelta.x, 200 + height);
    }

    public void OnYesBtnClicked()
    {
        if (_yesCallback != null)
            _yesCallback();
        StartCoroutine(FadeOut());
    }

    public void OnNoBtnClicked()
    {
        if (_noCallback != null)
            _noCallback();
        StartCoroutine(FadeOut());
    }
}