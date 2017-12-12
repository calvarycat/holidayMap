using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocaleSearch : MonoBehaviour {

    public Transform rect;
    public GameObject pfLocale;
    public InputField SearchInputField;
    LocaleLable lableCurrent;
    public void Init(LocaleLable lable)
    {
        if (lableCurrent!=lable)
        {
            lableCurrent = lable;
            Utils.RemoveAllChildren(rect);
            SearchInputField.text = "";
        }
        Show(true);
    }

    public void OnSearchButtonClick()
    {
        PopupManager.Instance.ShowLoading();
        OnlineMapsOSMNominatim.Search(SearchInputField.text).OnComplete += OnSearchLocationComplete;
    }
   public void OnValueChange(string ip)
    {
        Debug.Log("on valuer change");
        OnlineMapsOSMNominatim.Search(SearchInputField.text).OnComplete += OnSearchLocationComplete;
    }
    public void OnEnEdit(string value)
    {
        Debug.Log("OnEnd edit");

    }
    public void OnUnitClick(OnlineMapsOSMNominatimResult _data)
    {
        Show(false);
        lableCurrent.Init(_data);
    }
    private void OnSearchLocationComplete(string result)
    {
        OnlineMapsOSMNominatimResult[] results = OnlineMapsOSMNominatim.GetResults(result);
        if (results != null)
        {
            Utils.RemoveAllChildren(rect);
            for (int i = 0; i < results.Length; i++)
            {
                GameObject _obj = Utils.Spawn(pfLocale, rect);
                LocaleUnit _locale = _obj.GetComponent<LocaleUnit>();
                _locale.Init(results[i]);
            }
        }
        PopupManager.Instance.HideLoading();
    }

	public void Show(bool value)
    {
        gameObject.SetActive(value);
    }
    
}
