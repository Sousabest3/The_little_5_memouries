using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject subMenuPanel;         // Painel com Continue / Novo Jogo
    public GameObject noSaveText;           // Mensagem "sem save" (TMP desativado no início)

    [Header("Config")]
    public string firstSceneName = "IntroScene";

    private string savePath => Application.persistentDataPath + "/save.sav";

    public void PlayGame()
    {
        subMenuPanel.SetActive(true);
    }

    public void ContinueGame()
    {
        if (File.Exists(savePath))
        {
            Debug.Log("🔁 Save encontrado. Carregando...");
            SceneManager.LoadScene(firstSceneName);
        }
        else
        {
            Debug.Log("❌ Nenhum save encontrado!");
            StartCoroutine(ShowNoSaveMessage());
        }
    }

    public void NewGame()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("🗑️ Save antigo deletado.");
        }

        SceneManager.LoadScene(firstSceneName); // Novo jogo
    }

    public void QuitGame()
    {
        Debug.Log("🚪 Saindo do jogo...");
        Application.Quit();
    }

    private IEnumerator ShowNoSaveMessage()
    {
        noSaveText.SetActive(true);
        yield return new WaitForSeconds(2f);
        noSaveText.SetActive(false);
    }
}
