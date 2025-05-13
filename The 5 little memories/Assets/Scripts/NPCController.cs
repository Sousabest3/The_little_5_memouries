using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator), typeof(Collider2D))]
public class NPCController : MonoBehaviour, IInteractable
{
    [Header("Dialogue Settings")]
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    [Header("Movement Settings")]
    public bool canFollowPlayer = true;
    public float followSpeed = 3f;
    public float minFollowDistance = 1.5f;
    public float followStartDelay = 0.5f;

    [Header("Animation Settings")]
    public string horizontalParam = "MoveX";
    public string verticalParam = "MoveY";
    public string speedParam = "MoveSpeed";

    [Header("Interaction Settings")]
    public bool disableAfterDialogueEnds = true;
    public bool disableColliderAfterDialogue = true;
    public KeyCode advanceKey = KeyCode.Space;

    [Header("Cross-Scene Settings")]
    public bool persistBetweenScenes = true;
    public Vector3 followOffset = new Vector3(1f, 0f, 0f);

    // Component references
    private Animator animator;
    private Collider2D npcCollider;
    private Transform playerTransform;
    private PlayerMovement playerMovement;
    private Vector2 currentVelocity;

    // State variables
    private bool isFollowing = false;
    private bool isInteractable = true;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private int currentDialogueIndex;
    private Coroutine typingCoroutine;

    // Singleton pattern
    private static NPCController instance;

    void Awake()
    {
        // Singleton implementation
        if (persistBetweenScenes)
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        animator = GetComponent<Animator>();
        npcCollider = GetComponent<Collider2D>();
        
        // Find player references
        FindPlayer();
        
        // Subscribe to scene change events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void FindPlayer()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerTransform = playerMovement.transform;
        }
        else
        {
            Debug.LogWarning("PlayerMovement not found in scene!");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
        
        // Adjust position if following
        if (isFollowing && playerTransform != null)
        {
            transform.position = playerTransform.position + followOffset;
        }
    }

    void Update()
    {
        // Advance dialogue with key press
        if (isDialogueActive && Input.GetKeyDown(advanceKey))
        {
            NextDialogueLine();
        }

        // Handle following behavior
        if (isFollowing && playerTransform != null)
        {
            HandleFollowing();
        }
    }

    // ===== INTERACTION SYSTEM =====
    public bool CanInteract() => isInteractable && !isDialogueActive;

    public void Interact()
    {
        if (!CanInteract() || dialogueData == null || PauseController.IsGamePaused)
            return;

        if (!isDialogueActive)
        {
            StartDialogue();
        }
    }

    // ===== DIALOGUE SYSTEM =====
    void StartDialogue()
    {
        isDialogueActive = true;
        currentDialogueIndex = 0;

        if (playerMovement != null)
            playerMovement.SetCanMove(false);

        PauseController.SetPause(true);
        UpdateDialogueUI();
        dialoguePanel.SetActive(true);
        typingCoroutine = StartCoroutine(TypeDialogueLine());
    }

    IEnumerator TypeDialogueLine()
    {
        isTyping = true;
        dialogueText.text = "";
        var currentLine = dialogueData.dialogueLines[currentDialogueIndex];

        foreach (char letter in currentLine.text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(currentLine.typingSpeed);
        }

        isTyping = false;

        if (currentLine.autoProgress)
        {
            yield return new WaitForSeconds(currentLine.autoProgressDelay);
            NextDialogueLine();
        }
    }

    void NextDialogueLine()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = dialogueData.dialogueLines[currentDialogueIndex].text;
            isTyping = false;
            return;
        }

        currentDialogueIndex++;

        if (currentDialogueIndex < dialogueData.dialogueLines.Length)
        {
            UpdateDialogueUI();
            typingCoroutine = StartCoroutine(TypeDialogueLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void UpdateDialogueUI()
    {
        var currentLine = dialogueData.dialogueLines[currentDialogueIndex];
        bool isPlayerSpeaking = currentLine.isPlayerSpeaking;

        nameText.text = isPlayerSpeaking ? "Player" : dialogueData.npcName;
        portraitImage.sprite = isPlayerSpeaking ?
            (currentLine.playerExpression ?? dialogueData.defaultPlayerExpression) :
            (currentLine.npcExpression ?? dialogueData.defaultNPCExpression);
    }

    void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        PauseController.SetPause(false);

        if (playerMovement != null)
            playerMovement.SetCanMove(true);

        if (disableAfterDialogueEnds)
        {
            isInteractable = false;
            
            if (disableColliderAfterDialogue && npcCollider != null)
                npcCollider.enabled = false;
        }

        if (canFollowPlayer)
            StartCoroutine(StartFollowingAfterDelay());
    }

    // ===== FOLLOW SYSTEM =====
    IEnumerator StartFollowingAfterDelay()
    {
        yield return new WaitForSeconds(followStartDelay);
        isFollowing = true;
    }

    void HandleFollowing()
    {
        Vector2 targetPos = playerTransform.position + followOffset;
        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPos - currentPos).normalized;
        float distance = Vector2.Distance(currentPos, targetPos);

        if (distance > minFollowDistance)
        {
            transform.position = Vector2.SmoothDamp(
                currentPos,
                targetPos,
                ref currentVelocity,
                0.3f,
                followSpeed
            );
            UpdateAnimation(direction);
        }
        else
        {
            UpdateAnimation(Vector2.zero);
            currentVelocity = Vector2.zero;
        }
    }

    // ===== ANIMATION SYSTEM =====
    void UpdateAnimation(Vector2 movement)
    {
        if (animator == null) return;

        animator.SetFloat(horizontalParam, movement.x);
        animator.SetFloat(verticalParam, movement.y);
        animator.SetFloat(speedParam, movement.magnitude);
    }

    // ===== PUBLIC METHODS =====
    public void SetFollowing(bool shouldFollow)
    {
        isFollowing = shouldFollow;
        if (!shouldFollow)
        {
            currentVelocity = Vector2.zero;
            UpdateAnimation(Vector2.zero);
        }
    }

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        if (npcCollider != null)
            npcCollider.enabled = interactable;
    }
}