using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessagePopupComponent : MonoBehaviour
{
    public Text MessageText;
    public Text GuiText;
    public float LifeTime;
    public Animator MyAnimator;

    private string _message;

    public float Init(string message)
    {
        _message = message;
        GuiText.text = _message;
        MessageText.text = _message;
        StartCoroutine(HidePopUp());
        return GuiText.rectTransform.sizeDelta.y;
    }

    private IEnumerator HidePopUp()
    {
        if (MyAnimator != null)
        {
            MyAnimator.SetTrigger("Hide");
        }
        yield return new WaitForSeconds(LifeTime);
        PopupManager.Instance.OnDestroyMessagePopup(this);
        Destroy(gameObject);
    }

    public void OnMoveUp(float size)
    {
        Vector3 pos = GuiText.rectTransform.localPosition;
        GuiText.rectTransform.localPosition = new Vector3(pos.x, pos.y + size + 50, pos.z);
    }
}