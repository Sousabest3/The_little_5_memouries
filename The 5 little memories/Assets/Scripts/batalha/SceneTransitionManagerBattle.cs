using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManagerBattle : MonoBehaviour
{
    public static SceneTransitionManagerBattle Instance;
    public Animator transitionAnimator;
    public float transitionTime = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        if (transitionAnimator != null)
            transitionAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }
}
