using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator), typeof(Collider2D))]
public class NPCController2D : MonoBehaviour, IInteractable
{
    public static NPCController2D FollowingNPC;

    [Header("Dialogue Settings")]
    public DialogueData dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public Image portraitImage;

    [Header("Player & Movement")]
    public Transform player;
    public float followSpeed = 5f;
    public float pointReachThreshold = 0.1f;
    public float recordRate = 0.05f;
    public bool canFollowAfterDialogue = false;

    [Header("Keys")]
    public KeyCode advanceKey = KeyCode.Space;

    private Animator animator;
    private Coroutine typingCoroutine;
    private int currentLineIndex;
    private bool isTyping = false;
    private bool isDialogueActive = false;
    private bool isFollowing = false;
    private bool goingToBattle = false;

    private Queue<Vector3> positionHistory = new Queue<Vector3>();
    private float recordTimer;
    private Vector2 lastMoveInput;

    private Collider2D npcCollider;
    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (FollowingNPC != null && FollowingNPC != this && canFollowAfterDialogue)
        {
            Destroy(gameObject);
            return;
        }

        animator = GetComponent<Animator>();
        npcCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        dialoguePanel.SetActive(false);

        if (canFollowAfterDialogue)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(advanceKey) && !isTyping)
        {
            NextLine();
        }

        if (isFollowing && player != null)
        {
            RecordPlayerPosition();
            FollowPath();
        }
    }

    public bool CanInteract() => !isDialogueActive;

    public void Interact()
    {
        if (dialogueData == null || isDialogueActive) return;
        StartDialogue();
    }

    private void StartDialogue()
    {
        isDialogueActive = true;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        playerMovement?.SetCanMove(false);
        ShowLine();
    }

    private void ShowLine()
    {
        var line = dialogueData.lines[currentLineIndex];

        // Define nome e imagem com base no speaker
        switch (line.speaker)
        {
            case DialogueData.SpeakerType.Player:
                nameText.text = dialogueData.playerName;
                portraitImage.sprite = line.portrait ?? dialogueData.defaultPlayerPortrait;
                break;
            case DialogueData.SpeakerType.NPC:
                nameText.text = dialogueData.npcName;
                portraitImage.sprite = line.portrait ?? dialogueData.defaultNPCPortrait;
                break;
            case DialogueData.SpeakerType.Ally:
                nameText.text = dialogueData.allyName;
                portraitImage.sprite = line.portrait ?? dialogueData.defaultAllyPortrait;
                break;
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(DialogueData.Line line)
    {
        isTyping = true;
        dialogueText.text = "";
        float speed = Mathf.Max(0.01f, line.typeSpeed);

        foreach (char c in line.text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(speed);
        }

        isTyping = false;

        if (line.autoAdvance)
        {
            yield return new WaitForSeconds(line.advanceDelay);
            NextLine();
        }
    }

    private void NextLine()
    {
        currentLineIndex++;
        if (currentLineIndex < dialogueData.lines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        StopAllCoroutines();
        isTyping = false;
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        playerMovement?.SetCanMove(true);

        if (canFollowAfterDialogue)
        {
            isFollowing = true;
            npcCollider.enabled = false;

            if (FollowingNPC != null && FollowingNPC != this)
            {
                Destroy(FollowingNPC.gameObject);
            }

            FollowingNPC = this;
        }
    }

    private void RecordPlayerPosition()
    {
        recordTimer += Time.deltaTime;
        if (recordTimer >= recordRate)
        {
            positionHistory.Enqueue(player.position);
            recordTimer = 0f;
        }
    }

    private void FollowPath()
    {
        if (positionHistory.Count == 0) return;

        Vector3 targetPos = positionHistory.Peek();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, followSpeed * Time.deltaTime);

        Vector2 direction = (targetPos - transform.position).normalized;
        UpdateAnimation(direction);

        if (Vector3.Distance(transform.position, targetPos) < pointReachThreshold)
        {
            positionHistory.Dequeue();
        }
    }

    private void UpdateAnimation(Vector2 input)
    {
        bool isWalking = input != Vector2.zero;
        animator.SetBool("IsWalking", isWalking);

        if (isWalking)
        {
            animator.SetFloat("InputX", input.x);
            animator.SetFloat("InputY", input.y);
            lastMoveInput = input;
            animator.SetFloat("LastInputX", input.x);
            animator.SetFloat("LastInputY", input.y);
        }
    }

    private void FindPlayer()
    {
        if (player != null) return;

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerMovement = playerObj.GetComponent<PlayerMovement>();
        }
        else
        {
            Debug.LogWarning("⚠️ NPCController2D: Player não encontrado.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Battle")
        {
            if (isFollowing)
            {
                goingToBattle = true;
                spriteRenderer.enabled = false;
                animator.enabled = false;
                npcCollider.enabled = false;
                this.enabled = false;
            }
            else
            {
                Destroy(gameObject);
            }

            return;
        }

        StartCoroutine(SnapToPlayerAfterLoad());
    }

    private IEnumerator SnapToPlayerAfterLoad()
    {
        yield return null;

        FindPlayer();
        if (player == null) yield break;

        if (isFollowing)
        {
            transform.position = player.position + new Vector3(1.2f, 0f, 0f);
            positionHistory.Clear();
            positionHistory.Enqueue(player.position);
        }

        if (goingToBattle && isFollowing)
        {
            goingToBattle = false;
            spriteRenderer.enabled = true;
            animator.enabled = true;
            npcCollider.enabled = false;
            this.enabled = true;
        }
    }
}
