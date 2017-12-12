using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class ResetGame : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(DeleteAll());
    }

    private IEnumerator DeleteAll()
    {
        LeanTween.cancelAll();
        Localization.onLanguageChange = null;
        Localization.onLocalize = null;

        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            if (go != gameObject)
            {
                if (go.name != "~LeanTween")
                    Destroy(go);
            }
        }

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Resources.UnloadUnusedAssets();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        GC.Collect();
        SceneManager.LoadScene(SceneName.Splash);
    }
}