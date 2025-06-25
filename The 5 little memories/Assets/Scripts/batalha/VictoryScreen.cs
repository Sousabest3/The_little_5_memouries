using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public Button continueButton;

    void Start()
    {
        ShowVictoryData();
        continueButton.onClick.AddListener(ReturnToGame);
    }

    void ShowVictoryData()
    {
        // Simulação: Pegando o XP (altere para o seu sistema real)
        int xpGanho = 50;

        // Aqui você pode usar referência ao PlayerCombatant ou GameManager
        bool subiuDeNivel = true; // Simulação
        int ataqueGanho = 2;
        int vidaGanha = 10;

        string texto = $"<b>Vitória!</b>\n\nGanhaste <color=yellow>{xpGanho} XP</color>.";

        if (subiuDeNivel)
        {
            texto += $"\n\n<color=green>Subiste de nível!</color>\n";
            texto += $"<b>+{ataqueGanho}</b> ATK\n";
            texto += $"<b>+{vidaGanha}</b> HP";
        }

        resultText.text = texto;
    }

    void ReturnToGame()
    {
        SceneManager.LoadScene("Outside"); // ou o nome da sua cena principal
    }
}
