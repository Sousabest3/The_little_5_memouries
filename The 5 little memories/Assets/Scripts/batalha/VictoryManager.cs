using UnityEngine;
using TMPro;

public class VictoryManager : MonoBehaviour
{
    public TMP_Text victoryText;

    private void Start()
    {
        if (victoryText != null)
            victoryText.text = "Batalha vencida!";
    }

    public void ReturnToMap()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.GoBackToPreviousScene();
        }
        else
        {
            // Fallback sem fade
            UnityEngine.SceneManagement.SceneManager.LoadScene("MapScene");
        }
    }
}
