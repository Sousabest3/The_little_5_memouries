using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    
    [Header("Fade Settings")]
    public float fadeSpeed = 1.5f;
    public Color fadeColor = Color.black;
    
    [Header("Player Settings")]
    public string playerTag = "Player";
    
    private Image fadeImage;
    private Canvas transitionCanvas;
    private bool isTransitioning;
    private Stack<SceneInfo> sceneHistory = new Stack<SceneInfo>();
    private GameObject playerOverride;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTransitionSystem();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsTransitioning()
    {
        return isTransitioning;
    }

    void InitializeTransitionSystem()
    {
        if (transitionCanvas == null)
        {
            GameObject canvasObj = new GameObject("TransitionCanvas");
            transitionCanvas = canvasObj.AddComponent<Canvas>();
            transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            transitionCanvas.sortingOrder = 9999;
            DontDestroyOnLoad(canvasObj);
        }

        if (fadeImage == null)
        {
            GameObject imageObj = new GameObject("FadeImage");
            imageObj.transform.SetParent(transitionCanvas.transform);
            fadeImage = imageObj.AddComponent<Image>();
            
            RectTransform rt = imageObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
            fadeImage.raycastTarget = false;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeTransitionSystem();
        
        if (playerOverride != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                player.transform.position = playerOverride.transform.position;
                Destroy(playerOverride);
            }
        }
    }

    public void ChangeScene(string sceneName, Vector2 spawnPosition, bool registerHistory = true)
    {
        if (isTransitioning) return;
        
        if (registerHistory)
        {
            GameObject currentPlayer = GameObject.FindGameObjectWithTag(playerTag);
            if (currentPlayer != null)
            {
                sceneHistory.Push(new SceneInfo(
                    SceneManager.GetActiveScene().name,
                    currentPlayer.transform.position
                ));
            }
        }
        
        StartCoroutine(TransitionRoutine(sceneName, spawnPosition));
    }

    public void GoBackToPreviousScene()
    {
        if (sceneHistory.Count > 0 && !isTransitioning)
        {
            SceneInfo previousScene = sceneHistory.Pop();
            
            playerOverride = new GameObject("PlayerPositionOverride");
            playerOverride.transform.position = previousScene.playerPosition;
            
            StartCoroutine(TransitionRoutine(previousScene.sceneName, previousScene.playerPosition, false));
        }
    }

    private IEnumerator TransitionRoutine(string sceneName, Vector2 spawnPosition, bool movePlayer = true)
    {
        isTransitioning = true;
        
        yield return StartCoroutine(FadeRoutine(0, 1));
        
        SceneManager.LoadScene(sceneName);
        
        if (movePlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                player.transform.position = spawnPosition;
            }
        }
        
        yield return StartCoroutine(FadeRoutine(1, 0));
        
        isTransitioning = false;
    }

    private IEnumerator FadeRoutine(float startAlpha, float endAlpha)
    {
        if (fadeImage == null) yield break;

        fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        float fadeDuration = Mathf.Abs(endAlpha - startAlpha) / fadeSpeed;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, endAlpha);
        
        if (endAlpha == 0)
        {
            fadeImage.gameObject.SetActive(false);
        }
    }

    private struct SceneInfo
    {
        public string sceneName;
        public Vector2 playerPosition;

        public SceneInfo(string name, Vector2 position)
        {
            sceneName = name;
            playerPosition = position;
        }
    }
}