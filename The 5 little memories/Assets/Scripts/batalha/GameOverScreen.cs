using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI infoText;
    public Button retryButton;
    public Button menuButton; // <- Altere o nome para menuButton

    private void Start()
    {
        titleText.text = "Derrota...";
        infoText.text = "Todos foram derrotados.";

        retryButton.onClick.AddListener(RestartBattle);
        menuButton.onClick.AddListener(BackToMainMenu);
    }

    void RestartBattle()
    {
        SceneManager.LoadScene("Florest2"); // ou a cena anterior à batalha
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
