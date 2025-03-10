using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer; // Referência para o Audio Mixer
    public Slider volumeSlider;   // Slider para ajustar o volume
    public Toggle fullscreenToggle; // Toggle para tela cheia
    public TMP_Dropdown graphicsDropdown; // Dropdown para qualidade gráfica (TextMeshPro)

    void Start()
    {
        // Configura o slider de volume com o valor atual
        if (audioMixer != null && volumeSlider != null)
        {
            float volume;
            audioMixer.GetFloat("volume", out volume);
            volumeSlider.value = volume;
        }

        // Configura o toggle de tela cheia com o estado atual
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
        }

        // Configura o dropdown de qualidade gráfica com as opções disponíveis
        if (graphicsDropdown != null)
        {
            graphicsDropdown.ClearOptions();
            graphicsDropdown.AddOptions(new List<string>(QualitySettings.names));
            graphicsDropdown.value = QualitySettings.GetQualityLevel();
        }
    }

    // Função para ajustar o volume
 public void SetVolume(float volume)
{
    if (audioMixer != null)
    {
        // Converte o valor do slider (0 a 1) para decibéis (-80 a 0)
        float dB = volume > 0 ? 20 * Mathf.Log10(volume) : -80;
        audioMixer.SetFloat("volume", dB);
    }
}

    // Função para alternar entre tela cheia e modo janela
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // Função para ajustar a qualidade dos gráficos
    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // Função para voltar ao menu principal
    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void GoToVideoMenu()
   {
      SceneManager.LoadScene("VideoMenu");
   }
}
