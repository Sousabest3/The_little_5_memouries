using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public void Retry()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.GoBackToPreviousScene();
        }
        else
        {
            // Fallback
            UnityEngine.SceneManagement.SceneManager.LoadScene("MapScene");
        }
    }

}
