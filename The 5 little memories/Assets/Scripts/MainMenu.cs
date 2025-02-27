using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame()
   {
      SceneManager.LoadScene("IntroScene");
   }

   public void GoToOptionsMenu()
   {
      SceneManager.LoadScene("OptionsMenu");
   }

   public void GoToMainMenu()
   {
      SceneManager.LoadScene("MainMenu");
   }
   public void QuitGame()
   {
    Application.Quit();
   }
}
