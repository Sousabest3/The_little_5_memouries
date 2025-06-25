using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SettingsPageUI : MonoBehaviour
{
    [Header("Áudio")]
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    [Header("Vídeo")]
    public Toggle fullscreenToggle;
    public TMP_Dropdown graphicsDropdown;

    [Header("Navegação")]
    public Button backToMenuButton;

    void Start()
    {
        SetupVolume();
        SetupFullscreen();
        SetupGraphicsDropdown();
        SetupBackButton();
    }

    void SetupVolume()
    {
        if (audioMixer != null && volumeSlider != null)
        {
            if (audioMixer.GetFloat("volume", out float currentVolume))
            {
                volumeSlider.value = Mathf.Pow(10, currentVolume / 20f);
            }

            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    void SetupFullscreen()
    {
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.RemoveAllListeners();
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }

    void SetupGraphicsDropdown()
    {
        if (graphicsDropdown != null)
        {
            graphicsDropdown.ClearOptions();
            graphicsDropdown.AddOptions(new List<string>(QualitySettings.names));
            graphicsDropdown.value = QualitySettings.GetQualityLevel();
            graphicsDropdown.RefreshShownValue();
            graphicsDropdown.onValueChanged.RemoveAllListeners();
            graphicsDropdown.onValueChanged.AddListener(SetGraphicsQuality);
        }
    }

    void SetupBackButton()
    {
        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.RemoveAllListeners();
            backToMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
    }

    public void SetVolume(float value)
    {
        float dB = (value > 0) ? Mathf.Log10(value) * 20f : -80f;
        audioMixer.SetFloat("volume", dB);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu"); // ajuste para o nome correto da sua cena
    }
}
