using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScratchControl : MonoBehaviour
{

    public Transform parrentAnswer;
    public GameObject Answer;
    public Text txtQuestion;
    public Image imgQuest;
    public GameObject RootDragAndDrop;
    public GameObject RootScratch;
    private GroupScratch g;
    public GameObject ScratchCamera;
    public PNDragAndDropControl dragAndDropControl;
    public Text txtTimeCount;
    public void InitScratch()
    {
        ScratchCamera.gameObject.SetActive(true);
        int chooseQuest = DataManagerCourse.Instance.dataQuestScratch.groupScratch.Count;
        g = DataManagerCourse.Instance.dataQuestScratch.groupScratch[Random.Range(0, chooseQuest)];

        Utils.RemoveAllChildren(parrentAnswer);
        txtQuestion.text = g.title;
        imgQuest.sprite = Resources.Load<Sprite>("IMGScratch/" + g.linkImageName);

        int LableAnswer = 0;
        foreach (Answer ans in g.answer)
        {

            GameObject obj = Instantiate(Answer, parrentAnswer);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            AnswerInit ani = obj.GetComponent<AnswerInit>();
            ani.Init(Utils.GetLableAnswer(LableAnswer), ans.title);
            int answerChoose = LableAnswer;
            ani.GetComponentInChildren<Button>().onClick.AddListener(delegate { OnClickAnswer(answerChoose, ans.title); });
            LableAnswer++;
        }

    }
    void OnClickAnswer(int asn, string answer)
    {
        Debug.Log(asn + " // " + answer);

        if (CheckAnswer(asn))
        {

            ShowDragAndDropGame();

        }
        else
        {
            PanelPopUp.intance.OnInitInforPopUp("Opps!!", "Bạn chưa trả lời đúng. Vui lòng thử lại!! ");
        }

    }
    bool CheckAnswer(int chooseAns)
    {
        int posright = -1;

        foreach (Answer an in g.answer)
        {
            if (an.isCorrect)
                posright = an.sort;
        }
        if (posright == chooseAns)
        {
            return true;
        }

        return false;

    }
    #region DragAndDropGame
    void ShowDragAndDropGame()
    {
        ScratchCamera.gameObject.SetActive(false);
        RootDragAndDrop.gameObject.SetActive(true);
        dragAndDropControl.InitDragAndDropData();
        RootScratch.gameObject.SetActive(false);
    }
    #endregion
    public ControlQuestion controlQuest;
    private void Update()
    {
        txtTimeCount.text = Utils.SecondToString((int)controlQuest.timeRuning);

    }
}
