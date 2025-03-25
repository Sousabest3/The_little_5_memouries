using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{

    FadeinOut fade;
    void Start()
    {
        fade = FindAnyObjectByType<FadeinOut>();
    }

  
  public IEnumerator ChangeScene()
  {
    fade.FadeIn();
    yield return new WaitForSeconds(1);
    SceneManager.LoadScene(nextSceneName);
  }




    public string nextSceneName; // Nome da próxima cena

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifique se o objeto que entrou no trigger é o jogador
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ChangeScene());
        }
    }
}