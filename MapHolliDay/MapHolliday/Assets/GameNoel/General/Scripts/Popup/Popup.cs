using UnityEngine;
using System.Collections;

public class Popup : MonoBehaviour
{
    protected CanvasGroup CanvasGroup;
    protected float TimeDelay = 0.4f;

    private void Start()
    {
        CanvasGroup = gameObject.GetComponent<CanvasGroup>();
        CanvasGroup.alpha = 0;
        StartCoroutine(FadeIn());
    }

    protected IEnumerator FadeIn()
    {
        while (CanvasGroup.alpha < 1)
        {
            CanvasGroup.alpha += Time.deltaTime / TimeDelay;
            yield return null;
        }
    }

    protected IEnumerator FadeOut()
    {
        while (CanvasGroup.alpha > 0)
        {
            CanvasGroup.alpha -= Time.deltaTime / TimeDelay;
            yield return null;
        }
        Destroy(gameObject);
    }
}