using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneFromOther : MonoBehaviour
{
#if UNITY_EDITOR
    private static bool _isLoad;

    private void Awake()
    {
        if (!_isLoad)
        {
            _isLoad = true;

            if (SceneManager.GetActiveScene().name != SceneName.Splash)
                SceneManager.LoadScene(SceneName.Empty);
        }
    }
#endif
}