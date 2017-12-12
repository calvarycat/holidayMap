using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Transform parrentAnswer;
    public Transform parrentAnswer1;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("OnPointerEnter");
        if (eventData.pointerDrag == null)
            return;

        Dragable d = eventData.pointerDrag.GetComponent<Dragable>();
        if (d != null)
        {
            d.placeholderParent = this.transform;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("OnPointerExit");
        if (eventData.pointerDrag == null)
            return;

        Dragable d = eventData.pointerDrag.GetComponent<Dragable>();
        if (d != null && d.placeholderParent == this.transform)
        {
            d.placeholderParent = d.parentToReturnTo;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);

        Dragable d = eventData.pointerDrag.GetComponent<Dragable>();
        if (d != null)
        {
            d.parentToReturnTo = this.transform;
           // CheckAfterDrop();
        }
      

    }
    void CheckAfterDrop()
    {
        int text1 = CheckTextInParrent(0);
        if (text1 > 40)
        {
            GameObject ob = parrentAnswer.GetChild(parrentAnswer.childCount - 1).gameObject;
            ob.transform.SetParent(parrentAnswer1);
            ob.transform.SetAsFirstSibling();
        }

    }
    public int CheckTextInParrent(int prID)
    {
        int leng = 0;
        Transform tranloop;
        if (prID == 0)
        {
            tranloop = parrentAnswer;
        }
        else
        {
            tranloop = parrentAnswer1;
        }


        foreach (Transform tran in tranloop.transform)
        {

            DragAnswerElement dr = tran.GetComponent<DragAnswerElement>();
            leng += dr.ans.Length;
        }
        return leng;
    }
}
