using System;
using UnityEngine;
using UnityEngine.UI;

public enum StringType
{
    Normal,
    Upper,
    Lower
}

/// <summary>
///     Simple script that lets you localize a UIWidget.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class UILocalize : MonoBehaviour
{
    /// <summary>
    ///     Localization key.
    /// </summary>
    public string key;

    /// <summary>
    ///     Localization key unit.
    /// </summary>
    public string keyUnit;

    /// <summary>
    ///     String format
    /// </summary>
    public StringType type;

    /// <summary>
    ///     args for string.Format
    /// </summary>
    public object[] args;

    private bool _isRegisterEvent;
    private Text _text;

    /// <summary>
    ///     Manually change the value of whatever the localization component is attached to.
    /// </summary>
    public string value
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (_text == null)
                    _text = GetComponent<Text>();

                if (_text != null)
                {
                    switch (type)
                    {
                        case StringType.Normal:
                            if (args != null)
                                _text.text = string.Format(value, args);
                            else
                                _text.text = value;
                            break;

                        case StringType.Upper:
                            if (args != null)
                                _text.text = string.Format(value, args).ToUpper();
                            else
                                _text.text = value.ToUpper();
                            break;

                        case StringType.Lower:
                            if (args != null)
                                _text.text = string.Format(value, args).ToLower();
                            else
                                _text.text = value.ToLower();
                            break;
                    }
                }
            }
        }
    }

    private void Awake()
    {
        _text = GetComponent<Text>();
        RegisterEvent();
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

    public void Set(string key,
        string keyUnit = "",
        StringType type = StringType.Normal,
        params object[] args)
    {
        this.key = key;
        this.keyUnit = keyUnit;
        this.type = type;
        this.args = args;
        Refresh();
    }

    public void Refresh()
    {
        // If no localization key has been specified, use the label's text as the key
        if (string.IsNullOrEmpty(key))
        {
            if (_text == null)
                _text = GetComponent<Text>();

            if (_text != null)
                key = _text.text;
        }

        // If we still don't have a key, leave the value as blank
        if (!string.IsNullOrEmpty(key))
        {
            if (!string.IsNullOrEmpty(keyUnit))
            {
                value = Localization.Get(key, keyUnit);
            }
            else
            {
                value = Localization.Get(key);
            }
        }
    }
}

public static class UITextExtension
{
    public static UILocalize SetLocalization(this Text text,
        string key)
    {
        return SetLocalization(text, key, "", StringType.Normal);
    }

    public static UILocalize SetLocalization(this Text text,
        string key,
        params object[] args)
    {
        return SetLocalization(text, key, "", StringType.Normal, args);
    }

    public static UILocalize SetLocalization(this Text text,
        string key,
        string keyUnit,
        params object[] args)
    {
        return SetLocalization(text, key, keyUnit, StringType.Normal, args);
    }

    public static UILocalize SetLocalization(this Text text,
        string key,
        string keyUnit,
        StringType type,
        params object[] args)
    {
        UILocalize uiLocalize = text.GetComponent<UILocalize>();

        if (uiLocalize == null)
            uiLocalize = text.gameObject.AddComponent<UILocalize>();

        uiLocalize.Set(key, keyUnit, type, args);
        return uiLocalize;
    }
}