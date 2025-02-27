using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class IntroController : MonoBehaviour
{
   public PlayableDirector timeline;

    void Start()
    {
        // Assina o evento de fim da Timeline
        timeline.stopped += OnTimelineFinished;
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        // Carrega a cena do jogo
        SceneManager.LoadScene("bedroom");
    }
}
