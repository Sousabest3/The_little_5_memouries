using UnityEngine;

public class GameLoader : MonoBehaviour
{
    void Start()
    {
        // Verifica se há um save
        string path = Application.persistentDataPath + "/save.sav";
        if (System.IO.File.Exists(path))
        {
            Debug.Log("📂 Save encontrado. Carregando...");
            GameSaver.Instance?.LoadGame();
        }
        else
        {
            Debug.Log("🆕 Nenhum save encontrado. Novo jogo.");
        }
    }
}
