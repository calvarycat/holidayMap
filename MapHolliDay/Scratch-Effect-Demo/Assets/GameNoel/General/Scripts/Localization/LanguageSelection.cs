using UnityEngine;

/// <summary>
///     Turns the popup list it's attached to into a language selection list.
/// </summary>
public class LanguageSelection : MonoBehaviour
{
    private void Awake()
    {
        Refresh();
    }

    /// <summary>
    ///     Immediately refresh the list of known languages.
    /// </summary>
    public void Refresh()
    {
        if (Localization.knownLanguages != null)
        {
        }
    }

    public void onChange(string value)
    {
        Localization.language = value;
    }
}