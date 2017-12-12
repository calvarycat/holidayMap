using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System;

public class DataManagerCourse : MonoSingleton<DataManagerCourse>
{

    public RootObjectQuestion data; // data question multipleChoose
    public RootObjectQuestionScratch dataQuestScratch;
    public RootObjectQuestionDragAndDrop dataQuestDragAndDrop;

    public void LoadListQuestion()
    {
        string path = PathManager.ListQuest;

        try
        {
            // string asset = Utils.LoadTextFromFile(path);
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            if (textAsset == null)
                return;
            string asset = textAsset.text;
            if (string.IsNullOrEmpty(asset))
            {
                return;
            }
            data = JsonMapper.ToObject<RootObjectQuestion>(asset);
        }
        catch (Exception error)
        {
            Debug.LogError("Getting question Data error: " + error.StackTrace);
        }
    }
    public void LoadListQuestionScratch()
    {
     
        string path = PathManager.ListQuestScratch;

        try
        {
            // string asset = Utils.LoadTextFromFile(path);
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            if (textAsset == null)
                return;
            string asset = textAsset.text;
            if (string.IsNullOrEmpty(asset))
            {
                return;
            }
            dataQuestScratch = JsonMapper.ToObject<RootObjectQuestionScratch>(asset);
        }
        catch (Exception error)
        {
            Debug.LogError("Getting question scratch Data error: " + error.StackTrace);
        }
    }
    public void LoadListQuestionDragAndDrop()
    {
   
        string path = PathManager.ListDragAndDrop;

        try
        {
         
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            if (textAsset == null)
                return;
            string asset = textAsset.text;
            if (string.IsNullOrEmpty(asset))
            {
                return;
            }
            dataQuestDragAndDrop = JsonMapper.ToObject<RootObjectQuestionDragAndDrop>(asset);
        }
        catch (Exception error)
        {
            Debug.LogError("Getting question Drag and Drop Data error: " + error.StackTrace);
        }
    }
}

#region data multiple choose
[System.Serializable]
public class Answer
{
    public int answerID { get; set; }
    public string title { get; set; }
    public int sort { get; set; }
    public bool isCorrect { get; set; }
}
[System.Serializable]
public class Group
{
    public int questionID { get; set; }
    public string title { get; set; }
    public int sort { get; set; }
    public List<Answer> answer { get; set; }
}
[System.Serializable]
public class RootObjectQuestion
{
    public int CourseID { get; set; }
    public List<Group> group { get; set; }
}
#endregion


#region data multiple choose

[System.Serializable]
public class GroupScratch
{
    public int questionID { get; set; }
    public string title { get; set; }
    public string linkImageName { get; set; }
    public int sort { get; set; }
    public List<Answer> answer { get; set; }
}
[System.Serializable]
public class RootObjectQuestionScratch
{
    public int CourseID { get; set; }
    public List<GroupScratch> groupScratch { get; set; }
}
#endregion

#region data drag and drop game
[System.Serializable]
public class DragELement
{
    public string title { get; set; }
    public int sort { get; set; }
}
[System.Serializable]
public class GroupDrag
{
    public int questionID { get; set; }
    public List<DragELement> dragELement { get; set; }
}
[System.Serializable]
public class RootObjectQuestionDragAndDrop
{
    public List<GroupDrag> groupDrag { get; set; }
}
//[System.Serializable]
//public class GroupDrag
//{
//    public int questionID { get; set; }
//    public List<DragElement> dragELement { get; set; }
//}
//[System.Serializable]
//public class DragElement
//{
//    public string title { get; set; }
//    public int sort { get; set; }
//}
//[System.Serializable]
//public class RootObjectQuestionDragAndDrop
//{ 
//    public List<GroupDrag> groupDrag { get; set; }
//}
#endregion