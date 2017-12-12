using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Transform Root;
    public GameObject YesNoPopUpPrefab;
    public GameObject InfoPopUpPrefab;
    public GameObject MesagePopUpPrefab;
    public GameObject Loading;
    public Text LoadingText;

    private readonly List<MessagePopupComponent> _listMessage = new List<MessagePopupComponent>();

    public void InitYesNoPopUp(string message,
        Action yesCallback = null,
        Action noCallBack = null,
        string yesButonName = "YES",
        string noButtonName = "NO")
    {
        GameObject popup = null;
        popup = Instantiate(YesNoPopUpPrefab);
        popup.SetActive(true);
        popup.transform.SetParent(Root.transform);
        popup.transform.localPosition = Vector3.zero;
        popup.transform.localScale = Vector3.one;
        YesNoPopUpComponent script = popup.GetComponent<YesNoPopUpComponent>();
        script.Init(message, yesCallback, noCallBack, yesButonName, noButtonName);
    }

    public void InitInfoPopUp(string message, Action callback = null, string buttonName = "OK")
    {
        GameObject popup = null;
        popup = Instantiate(InfoPopUpPrefab);
        popup.SetActive(true);
        popup.transform.SetParent(Root.transform);
        popup.transform.localPosition = Vector3.zero;
        popup.transform.localScale = Vector3.one;
        InfoPopUpComponent script = popup.GetComponent<InfoPopUpComponent>();
        script.Init(message, callback, buttonName);
    }

    public void InitMessage(string message)
    {
        GameObject popup = null;
        popup = Instantiate(MesagePopUpPrefab);
        popup.SetActive(true);
        popup.transform.SetParent(Root.transform);
        popup.transform.localPosition = Vector3.zero;
        popup.transform.localScale = Vector3.one;
        MessagePopupComponent script = popup.GetComponent<MessagePopupComponent>();

        float size = script.Init(message);

        for (int i = 0; i < _listMessage.Count; i++)
        {
            if (_listMessage[i] != null)
            {
                _listMessage[i].OnMoveUp(size);
            }
        }

        _listMessage.Add(script);
    }

    public void OnDestroyMessagePopup(MessagePopupComponent item)
    {
        _listMessage.Remove(item);
    }

    public void ChangeTextLoading(string text)
    {
        LoadingText.text = text;
    }

    public void ShowLoading(string text = "")
    {
        Loading.SetActive(true);
        LoadingText.text = text;
    }

    public void HideLoading()
    {
        Loading.SetActive(false);
    }
}