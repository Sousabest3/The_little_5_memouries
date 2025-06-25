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
    public NPCDialogue dialogueData;
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
    private Vector2 lastMoveDirection;

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
            transform.SetParent(null); // Garante que está na raiz da cena
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
        DialogueLine line = dialogueData.dialogueLines[currentLineIndex];
        nameText.text = line.isPlayerSpeaking ? "Player" : dialogueData.npcName;
        portraitImage.sprite = line.isPlayerSpeaking
            ? (line.playerExpression ?? dialogueData.defaultPlayerExpression)
            : (line.npcExpression ?? dialogueData.defaultNPCExpression);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;
        dialogueText.text = "";
        float speed = Mathf.Max(0.01f, line.typingSpeed);

        foreach (char c in line.text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(speed);
        }

        isTyping = false;

        if (line.autoProgress)
        {
            yield return new WaitForSeconds(line.autoProgressDelay);
            NextLine();
        }
    }

    private void NextLine()
    {
        currentLineIndex++;
        if (currentLineIndex < dialogueData.dialogueLines.Length)
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

    private void UpdateAnimation(Vector2 move)
{
    animator.SetFloat("MoveX", move.x);
    animator.SetFloat("MoveY", move.y);
    animator.SetFloat("MoveSpeed", move.magnitude);

    if (move.magnitude > 0.01f)
    {
        lastMoveDirection = move;
        animator.SetFloat("LastMoveX", lastMoveDirection.x);
        animator.SetFloat("LastMoveY", lastMoveDirection.y);

        // Flip se estiver a mover para a esquerda
        if (spriteRenderer != null)
            spriteRenderer.flipX = move.x < 0;
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
