using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleSizeText : MonoBehaviour
{
    public Vector2 Max;
    public bool ControlHorizontal;
    public bool ControlVertical;

    private bool _isRegisterEvent;
    private Text _text;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _text = gameObject.GetComponent<Text>();
        _rectTransform = gameObject.GetComponent<RectTransform>();
        RegisterEvent();
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDestroy()
    {
        UnRegisterEvent();
    }

    private void RegisterEvent()
    {
        if (_isRegisterEvent)
            return;
        _isRegisterEvent = true;
        Localization.onLanguageChange += Refresh;
        Refresh();
    }

    private void UnRegisterEvent()
    {
        if (!_isRegisterEvent)
            return;
        _isRegisterEvent = false;
        Localization.onLanguageChange -= Refresh;
    }

    public void Refresh()
    {
        if (_text == null || _rectTransform == null)
            return;

        float width = 0;
        float height = 0;

        if (ControlHorizontal)
        {
            width = _text.preferredWidth;
            width = Mathf.Min(width, Max.x);
        }
        else
        {
            width = _rectTransform.sizeDelta.x;
        }

        if (ControlVertical)
        {
            height = _text.preferredHeight;
            height = Mathf.Min(height, Max.y);
        }
        else
        {
            height = _rectTransform.sizeDelta.y;
        }

        _rectTransform.sizeDelta = new Vector2(width, height);
    }
}