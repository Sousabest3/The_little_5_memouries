using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static bool IsGamePaused { get; private set; } = false;
    public static System.Action<bool> OnPauseStateChanged;

    public static void SetPause(bool pause)
    {
        IsGamePaused = pause;
        Time.timeScale = pause ? 0 : 1;
        OnPauseStateChanged?.Invoke(pause);
    }
}