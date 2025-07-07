using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ScenePortal : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    public string targetSceneName;
    public Vector2 playerSpawnPosition;

    [Header("Portal Type")]
    public bool isTwoWayPortal = true;

    [Header("Interaction Settings")]
    public bool requireInteraction = false;
    public KeyCode interactionKey = KeyCode.E;
    public GameObject interactionPrompt;

    [Header("Visual Settings")]
    public Animator portalAnimator;
    public string activateTrigger = "Activate";

    [Header("Follower Requirements")]
    public bool requireFollower = true;
    [TextArea] public string missingFollowerMessage = "Preciso de alguém comigo para seguir em frente.";

    [Header("Blocker (Optional)")]
    public GameObject blockerWall;

    [Header("Missing Message UI")]
    public GameObject missingMessageUI;
    public TMP_Text missingMessageText;
    public float messageDuration = 2.5f;

    [Header("Events")]
    public UnityEvent onPortalActivated;
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    private bool playerInRange;
    private GameObject playerObj;
    private Collider2D portalCollider;
    private float messageTimer;

    void Awake()
    {
        portalCollider = GetComponent<Collider2D>();
        if (portalCollider == null)
            portalCollider = gameObject.AddComponent<BoxCollider2D>();

        portalCollider.isTrigger = true;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (blockerWall != null)
            blockerWall.SetActive(requireFollower);

        if (missingMessageUI != null)
            missingMessageUI.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || SceneTransitionManager.Instance.IsTransitioning())
            return;

        playerObj = other.gameObject;
        playerInRange = true;
        onPlayerEnter.Invoke();

        if (requireFollower && NPCController2D.FollowingNPC != null && blockerWall != null)
        {
            blockerWall.SetActive(false);
        }

        if (!requireInteraction)
        {
            TryActivatePortal();
        }
        else if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        onPlayerExit.Invoke();

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && requireInteraction && Input.GetKeyDown(interactionKey) &&
            !SceneTransitionManager.Instance.IsTransitioning())
        {
            TryActivatePortal();
        }

        // Oculta mensagem na tela após delay
        if (missingMessageUI != null && missingMessageUI.activeSelf)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f)
                missingMessageUI.SetActive(false);
        }
    }

    private void TryActivatePortal()
    {
        if (requireFollower && NPCController2D.FollowingNPC == null)
        {
            ShowDialogueMessage(missingFollowerMessage);
            if (blockerWall != null)
                blockerWall.SetActive(true);
            return;
        }

        if (blockerWall != null)
            blockerWall.SetActive(false);

        ActivatePortal();
    }

    public void ActivatePortal()
    {
        if (playerObj == null || SceneTransitionManager.Instance.IsTransitioning())
            return;

        onPortalActivated.Invoke();

        if (portalAnimator != null)
            portalAnimator.SetTrigger(activateTrigger);

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (portalCollider != null)
            portalCollider.enabled = false;

        var movement = playerObj.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        SceneTransitionManager.Instance.ChangeScene(
            targetSceneName,
            playerSpawnPosition,
            true
        );
    }

    private void ShowDialogueMessage(string message)
    {
        if (missingMessageUI != null && missingMessageText != null)
        {
            missingMessageText.text = message;
            missingMessageUI.SetActive(true);
            messageTimer = messageDuration;
        }
        else
        {
            Debug.Log($"📢 Player: {message}");
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, playerSpawnPosition);
        Gizmos.DrawWireSphere(playerSpawnPosition, 0.3f);

        Gizmos.color = isTwoWayPortal ?
            new Color(0.2f, 1f, 0.8f, 0.7f) :
            new Color(1f, 0.5f, 0.2f, 0.7f);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
#endif
}
