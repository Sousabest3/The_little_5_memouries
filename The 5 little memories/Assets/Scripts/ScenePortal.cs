using UnityEngine;
using UnityEngine.Events;

public class ScenePortal : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    public string targetSceneName;
    public Vector2 playerSpawnPosition;
    
    [Header("Portal Type")]
    [Tooltip("If true, creates automatic return path")]
    public bool isTwoWayPortal = true;
    
    [Header("Interaction Settings")]
    public bool requireInteraction = false;
    public KeyCode interactionKey = KeyCode.E;
    public GameObject interactionPrompt;
    
    [Header("Visual Settings")]
    public Animator portalAnimator;
    public string activateTrigger = "Activate";
    
    [Header("Events")]
    public UnityEvent onPortalActivated;
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    private bool playerInRange;
    private GameObject playerObj;
    private Collider2D portalCollider;

    void Awake()
    {
        portalCollider = GetComponent<Collider2D>();
        if (portalCollider == null)
        {
            portalCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        portalCollider.isTrigger = true;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !SceneTransitionManager.Instance.IsTransitioning())
        {
            playerObj = other.gameObject;
            playerInRange = true;
            onPlayerEnter.Invoke();
            
            if (!requireInteraction)
            {
                ActivatePortal();
            }
            else if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            onPlayerExit.Invoke();
            
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (playerInRange && requireInteraction && Input.GetKeyDown(interactionKey) 
            && !SceneTransitionManager.Instance.IsTransitioning())
        {
            ActivatePortal();
        }
    }

    public void ActivatePortal()
    {
        if (playerObj == null || SceneTransitionManager.Instance.IsTransitioning()) return;
        
        onPortalActivated.Invoke();
        
        if (portalAnimator != null)
        {
            portalAnimator.SetTrigger(activateTrigger);
        }
        
        var movement = playerObj.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }
        
        if (portalCollider != null)
        {
            portalCollider.enabled = false;
        }
        
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        SceneTransitionManager.Instance.ChangeScene(
            targetSceneName, 
            playerSpawnPosition,
            true
        );
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