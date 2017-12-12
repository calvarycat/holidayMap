using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class InfoPopUpComponent : Popup
{
    public Text MessageText;
    public Text ButtonText;
    public RectTransform Background;

    private Action _callback;
    private string _message;
    private string _buttonName;

    public void Init(string message, Action callback = null, string buttonName = "OK")
    {
        _message = message;
        _buttonName = buttonName;
        _callback = callback;

        MessageText.text = _message;
        ButtonText.text = _buttonName;

        float height = MessageText.preferredHeight;
        Background.sizeDelta = new Vector2(Background.sizeDelta.x, 200 + height);
    }

    public void OnButtonClicked()
    {
        StartCoroutine(FadeOut());

        if (_callback != null)
            _callback();
    }
}