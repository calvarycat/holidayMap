using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PNDragAndDropControl : MonoBehaviour
{


    public GameObject prefabAnswer;
    private GroupDrag g;
    public Transform parrentAnswer;
    public Transform parrentAnswer1;
    public Transform parrentAnswer2;


    public List<DragELement> TronMangAnswer(List<DragELement> dr)
    {
        List<DragELement> listResult = new List<DragELement>();
        //foreach (DragELement ans in g.dragELement)
        //{
        //}
        int num = dr.Count;
        for(int i=0;i<num;i++)
        {
            int a = Random.Range(0, dr.Count);
            listResult.Add(dr[a]);
            dr.RemoveAt(a);
        }
        return listResult;
    }
    public void InitDragAndDropData()
    {

        int chooseQuest = DataManagerCourse.Instance.dataQuestDragAndDrop.groupDrag.Count;
        g = DataManagerCourse.Instance.dataQuestDragAndDrop.groupDrag[Random.Range(0, chooseQuest)];

        Utils.RemoveAllChildren(parrentAnswer);
        Utils.RemoveAllChildren(parrentAnswer1);
        //  Utils.RemoveAllChildren(parrentAnswer2);

        List<DragELement> listResult = TronMangAnswer(g.dragELement);

        int d = 0;
        foreach (DragELement ans in listResult)
        {
            d++;
            GameObject obj;
            int textLengInparrent = CheckTextInParrent();
            int newtext = textLengInparrent + ans.title.Length;
            if (newtext < 40)
            {
                obj = Instantiate(prefabAnswer, parrentAnswer);
            }
            else
            {
                obj = Instantiate(prefabAnswer, parrentAnswer1);
            }

            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.GetChild(0).GetComponent<Text>().text = ans.title;
            Canvas.ForceUpdateCanvases();
            DragAnswerElement dr = obj.GetComponent<DragAnswerElement>();
            dr.InitAnswer(ans.sort, ans.title);
        }

    }
    public int CheckTextInParrent()
    {
        int leng = 0;
        foreach (Transform tran in parrentAnswer.transform)
        {

            DragAnswerElement dr = tran.GetComponent<DragAnswerElement>();
            Debug.Log("vo day " + dr.ans);
            leng += dr.ans.Length;
        }
        return leng;
    }
    public void OnClickDone() //Check Gmae
    {
        Debug.Log("Click Done");
        if (CheckAnswer())
        {

            ShowNextStep();

        }
        else
        {
            PanelPopUp.intance.OnInitInforPopUp("Opps!!", "Bạn chưa trả lời đúng. Vui lòng thử lại!! ");
        }

    }
    bool CheckAnswer()
    {

        bool result = true;
        foreach (Transform tran in parrentAnswer)
        {
            DragAnswerElement dr = tran.GetComponent<DragAnswerElement>();
            if (dr.step != tran.GetSiblingIndex())
            {
                result = false;
            }
        }
        if (result)
            foreach (Transform tran in parrentAnswer1)
            {
                DragAnswerElement dr = tran.GetComponent<DragAnswerElement>();
                if (dr.step != tran.GetSiblingIndex() + parrentAnswer.childCount)
                {
                    result = false;
                }
            }
        //if (result)
        //    foreach (Transform tran in parrentAnswer2)
        //    {
        //        DragAnswerElement dr = tran.GetComponent<DragAnswerElement>();
        //        if (dr.step != tran.GetSiblingIndex() + parrentAnswer1.childCount + parrentAnswer2.childCount)
        //        {
        //            result = false;
        //        }
        //    }

        return result;

    }
    #region DragAndDropGame
    void ShowNextStep()
    {
        PanelPopUp.intance.OnInitInforPopUp("Hurray!!", "xin chúc mừng bạn đã hoàn thành thử thách, nào ta cùng selfie nào :D!! ");
    }
    #endregion
    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.H))
    //    {
    //        CheckTextInParrent();
    //    }
    //}
}
