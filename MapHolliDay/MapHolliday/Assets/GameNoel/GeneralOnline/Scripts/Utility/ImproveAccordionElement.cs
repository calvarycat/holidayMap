using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(LayoutElement))]
public class ImproveAccordionElement : Toggle
{
    public float MinHeight;

    private ImproveAccordion _improveAccordion;
    private RectTransform _rectTransform;
    private LayoutElement _layoutElement;
    private float _duration;
    private LTDescr _ltDescr;

    protected override void Awake()
    {
        base.Awake();

        _improveAccordion = gameObject.GetComponentInParent<ImproveAccordion>();
        _rectTransform = transform as RectTransform;
        _layoutElement = gameObject.GetComponent<LayoutElement>();

        if (_improveAccordion != null)
        {
            ToggleGroup toggleGroup = _improveAccordion.gameObject.GetComponent<ToggleGroup>();
            group = toggleGroup;
        }

        onValueChanged.AddListener(OnValueChanged);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        LayoutElement layoutElement = gameObject.GetComponent<LayoutElement>();

        if (layoutElement != null)
        {
            if (isOn)
            {
                layoutElement.preferredHeight = -1f;
            }
            else
            {
                layoutElement.preferredHeight = MinHeight;
            }
        }
    }
#endif

    public void OnValueChanged(bool state)
    {
        if (_layoutElement == null)
            return;

        _duration = _improveAccordion != null ? _improveAccordion.Duration : 0;

        if (_duration <= 0)
        {
            if (state)
            {
                if (_layoutElement.preferredHeight != -1)
                    _layoutElement.preferredHeight = -1f;
            }
            else
            {
                if (_layoutElement.preferredHeight != MinHeight)
                    _layoutElement.preferredHeight = MinHeight;
            }
        }
        else
        {
            if (state)
            {
                float targetHeight = GetExpandedHeight();
                if (_rectTransform.rect.height != targetHeight)
                    StartTween(MinHeight, GetExpandedHeight());
            }
            else
            {
                if (_rectTransform.rect.height != MinHeight)
                    StartTween(_rectTransform.rect.height, MinHeight);
            }
        }
    }

    protected float GetExpandedHeight()
    {
        if (_layoutElement == null)
            return MinHeight;

        float originalPrefH = _layoutElement.preferredHeight;
        _layoutElement.preferredHeight = -1f;
        float h = LayoutUtility.GetPreferredHeight(_rectTransform);
        _layoutElement.preferredHeight = originalPrefH;

        return h;
    }

    protected void StartTween(float startFloat, float targetFloat)
    {
        if (_ltDescr != null)
            LeanTween.cancel(gameObject, _ltDescr.id);

        _ltDescr = LeanTween.value(gameObject, SetHeight, startFloat, targetFloat, _duration);
    }

    protected void SetHeight(float height)
    {
        if (_layoutElement == null)
            return;

        _layoutElement.preferredHeight = height;
    }
}