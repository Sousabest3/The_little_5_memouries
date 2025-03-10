using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string nextSceneName; // Nome da próxima cena

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifique se o objeto que entrou no trigger é o jogador
        if (collision.CompareTag("Player"))
        {
            // Carregue a próxima cena
            SceneManager.LoadScene(nextSceneName);
        }
    }
}