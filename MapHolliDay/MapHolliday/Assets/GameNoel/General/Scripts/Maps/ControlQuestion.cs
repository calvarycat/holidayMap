using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlQuestion : MonoBehaviour
{

    public GameObject RootMultipleChoise;
    public GameObject RootDragAndDrop;
    public GameObject RootScratch;
    public Transform parrentAnswer;
    public GameObject Answer;
    public Text txtQuestion;
    public float timeRuning = 0;
    public bool isStartRun;
    public Text txtTimeCount ;

    void Start()
    {
        DataManagerCourse.Instance.LoadListQuestion();
        DataManagerCourse.Instance.LoadListQuestionScratch();
        DataManagerCourse.Instance.LoadListQuestionDragAndDrop();
     
    }
    public void InitMultipleChoise()
    {
        RootMultipleChoise.gameObject.SetActive(true);
        StartSessionQuest();
        int chooseQuest = DataManagerCourse.Instance.data.group.Count;
      
        Group g = DataManagerCourse.Instance.data.group[Random.Range(0, chooseQuest)];
        SessionQuestAnswer q = new SessionQuestAnswer();
        q.answer = -1;
        q.quest = g; // thay bang cách lay dữ liệu from data layer
        Utils.RemoveAllChildren(parrentAnswer);
        txtQuestion.text = g.title;
        int LableAnswer = 0;
        Debug.Log(chooseQuest + g.answer.Count);
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
        AddQuest(q);
        timeRuning = 0;
        isStartRun = true;
    }
    void OnClickAnswer(int asn, string answer)
    {
        Debug.Log(asn + " // " + answer);
        curentSessionQuestList[0].answer = asn;

        if (CheckAnswer(curentSessionQuestList[0]))
        {
            //PanelPopUp.intance.OnInitInforPopUp("Congratulation!!", "Xin chúc mừng bạn, Nào hãy qua câu thứ 2 ");
            //  ShowDragAndDropGame();
            ShowScratchGame();
        }
        else
        {
            PanelPopUp.intance.OnInitInforPopUp("Opps!!", "Bạn chưa trả lời đúng. Vui lòng thử lại!! ");
        }

    }
  
    public List<SessionQuestAnswer> curentSessionQuestList;
    public void StartSessionQuest()
    {
        curentSessionQuestList = new List<SessionQuestAnswer>();
    }

    public void AddQuest(SessionQuestAnswer quest)
    {
        curentSessionQuestList.Add(quest);
    }
    bool CheckAnswer(SessionQuestAnswer chooseAns)
    {
        int posright = -1;
        Group quest = chooseAns.quest;
        foreach (Answer an in quest.answer)
        {
            if (an.isCorrect)
                posright = an.sort;
        }
        if (posright == chooseAns.answer)
        {
            return true;
        }

        return false;

    }

  
    #region ScratchGame
    public ScratchControl scratch;
    void ShowScratchGame()
    {
        scratch.InitScratch();
        RootScratch.gameObject.SetActive(true);
        RootMultipleChoise.gameObject.SetActive(false);


    }
    #endregion

    public void OnHomeClick()
    {
        Debug.Log("On Home Click");
    }
    public void OnBackClick()
    {
        Debug.Log("OnBack Click");
    }
    private void Update()
    {
        if(isStartRun)
        {
            timeRuning += Time.deltaTime;
            txtTimeCount.text = Utils.SecondToString((int)timeRuning);
        }
       
    }
}
public class SessionQuestAnswer
{
    public int answer = -1;
    public Group quest;

}
