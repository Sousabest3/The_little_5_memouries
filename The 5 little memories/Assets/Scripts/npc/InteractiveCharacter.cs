using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractiveCharacter : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class FollowSettings
    {
        [Tooltip("Enable NPC to follow player after dialogue?")]
        public bool enableFollow = true;
        
        [Tooltip("Prefab of the NPC follower")]
        public GameObject followerPrefab;
        
        [Tooltip("Following distance from player")]
        [Range(1f, 3f)] public float followDistance = 1.8f;
    }

    [Header("UI References")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image portraitImage;

    [Header("Dialogue Settings")]
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private float interactionCooldown = 0.5f;

    [Header("Input Settings")]
    [SerializeField] private Key interactKey = Key.E;
    [SerializeField] private GameObject advancePrompt;

    [Header("Follow System")]
    [SerializeField] private FollowSettings followSettings;

    // State variables
    private int _currentDialogueIndex;
    private bool _isTyping, _isInteracting, _waitingForInput;
    private float _lastInteractionTime;
    private PlayerMovement _playerMovement;
    private InputAction _interactAction;

    private void Awake()
    {
        // Set up input action
        _interactAction = new InputAction(binding: "<Keyboard>/e");
        _interactAction.performed += ctx => OnInteractPerformed();
    }

    private void Start()
    {
        dialogueCanvas?.SetActive(false);
        _playerMovement = FindObjectOfType<PlayerMovement>();
        
        if (advancePrompt != null)
            advancePrompt.SetActive(false);
    }

    private void OnEnable() => _interactAction.Enable();
    private void OnDisable() => _interactAction.Disable();

    public bool CanInteract() => !_isInteracting && Time.time > _lastInteractionTime + interactionCooldown;

    private void OnInteractPerformed()
    {
        if (dialogueData == null || !CanInteract()) return;

        if (_isInteracting) 
            AdvanceDialogue();
        else 
            StartDialogue();
    }

    private void Update()
    {
        if (_isInteracting && _waitingForInput && !_isTyping)
        {
            if (Keyboard.current[interactKey].wasPressedThisFrame)
            {
                AdvanceDialogue();
            }
        }
    }

    #region Dialogue Flow
    private void StartDialogue()
    {
        _isInteracting = true;
        _currentDialogueIndex = 0;
        _lastInteractionTime = Time.time;

        _playerMovement?.SetCanMove(false);
        dialogueCanvas.SetActive(true);
        
        UpdateDialogueUI();
        StartCoroutine(TypeDialogue());
    }

    private IEnumerator TypeDialogue()
    {
        _isTyping = true;
        _waitingForInput = false;
        
        DialogueData.Line currentLine = dialogueData.lines[_currentDialogueIndex];
        dialogueText.text = "";

        foreach (char c in currentLine.text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(currentLine.typeSpeed);
        }

        _isTyping = false;

        if (!currentLine.autoAdvance)
        {
            ShowAdvancePrompt(true);
            _waitingForInput = true;
        }
        else
        {
            yield return new WaitForSeconds(currentLine.advanceDelay);
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        ShowAdvancePrompt(false);
        _waitingForInput = false;

        if (_isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = dialogueData.lines[_currentDialogueIndex].text;
            _isTyping = false;
            ShowAdvancePrompt(true);
            _waitingForInput = true;
            return;
        }

        _currentDialogueIndex++;
        
        if (_currentDialogueIndex < dialogueData.lines.Length)
        {
            UpdateDialogueUI();
            StartCoroutine(TypeDialogue());
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        ShowAdvancePrompt(false);
        StopAllCoroutines();
        
        _isInteracting = false;
        dialogueCanvas.SetActive(false);
        _playerMovement?.SetCanMove(true);

        if (followSettings.enableFollow)
            SpawnFollower();
    }
    #endregion

    #region Helper Methods
    private void UpdateDialogueUI()
    {
        DialogueData.Line line = dialogueData.lines[_currentDialogueIndex];
        
        nameText.text = line.isPlayerSpeaking ? "Player" : dialogueData.characterName;
        portraitImage.sprite = line.isPlayerSpeaking 
            ? (line.playerExpression ?? dialogueData.defaultPlayerExpression) 
            : (line.npcExpression ?? dialogueData.defaultNPCExpression);
    }

    private void ShowAdvancePrompt(bool show)
    {
        if (advancePrompt != null)
            advancePrompt.SetActive(show);
    }

    private void SpawnFollower()
    {
        if (followSettings.followerPrefab == null || _playerMovement == null) 
            return;

        GameObject follower = Instantiate(
            followSettings.followerPrefab,
            transform.position,
            Quaternion.identity
        );

        var followerAI = follower.GetComponent<AdvancedNPCFollower>();
        if (followerAI != null)
        {
            followerAI.target = _playerMovement.transform;
            followerAI.minFollowDistance = followSettings.followDistance;
        }

        Destroy(gameObject);
    }
    #endregion

    // IInteractable implementation
    public void Interact() => OnInteractPerformed();
}