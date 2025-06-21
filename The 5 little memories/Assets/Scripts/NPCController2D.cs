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
    private Vector3 velocity;

    private Queue<Vector3> positionHistory = new Queue<Vector3>();
    private float recordTimer;

    private Collider2D npcCollider;
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
        dialoguePanel.SetActive(false);

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
            DontDestroyOnLoad(gameObject);
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
    }

    private void FindPlayer()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerMovement = playerObj.GetComponent<PlayerMovement>();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ✅ DESTROI O NPC SE ESTIVER A SEGUIR E A CENA FOR DE BATALHA
        if (isFollowing && scene.name == "Battle")
        {
            Destroy(gameObject);
            return;
        }

        FindPlayer();

        if (isFollowing && player != null)
        {
            transform.position = player.position + new Vector3(1.2f, 0f, 0f);
            positionHistory.Clear();
        }
    }
}
