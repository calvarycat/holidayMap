using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dragable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform parentToReturnTo = null;
    public Transform placeholderParent = null;

    GameObject placeholder = null;


    public void OnBeginDrag(PointerEventData eventData)
    {
        ////   Debug.Log("beginDrag");
        placeholder = new GameObject();
        placeholder.AddComponent<RectTransform>();//RectTransform
        placeholder.transform.SetParent(this.transform.parent);
        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
        Vector2 newsize = transform.GetComponent<RectTransform>().sizeDelta;
        //  placeholder.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(newsize.x, 80);
        placeholder.transform.localScale = Vector3.one;
        placeholder.transform.localPosition = Vector3.one;
        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

        parentToReturnTo = this.transform.parent;
        placeholderParent = parentToReturnTo;
        this.transform.SetParent(this.transform.parent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;

    }

    public void OnDrag(PointerEventData eventData)
    {     
        this.transform.position = eventData.position;
        if (placeholder.transform.parent != placeholderParent)
            placeholder.transform.SetParent(placeholderParent);
        int newSiblingIndex = placeholderParent.childCount;
        for (int i = 0; i < placeholderParent.childCount; i++)
        {
            if (this.transform.position.x < placeholderParent.GetChild(i).position.x)
            {

                newSiblingIndex = i;
                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex--;
                break;
            }
        }
        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
   
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;    
        Destroy(placeholder);

    }
}
