using UnityEngine;
using TMPro;
using System.Collections;

public class WorldDialogueSystem : MonoBehaviour
{
    public static WorldDialogueSystem Instance;

    [Header("UI Components")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Player Reference")]
    public PlayerMovement playerMovement;

    [Header("Dialogue Settings")]
    public KeyCode advanceKey = KeyCode.Space;
    public float defaultTypingSpeed = 0.04f;

    [Header("Typing Sound")]
    public AudioSource audioSource;
    public AudioClip blipSound;
    public float blipVolume = 0.5f;
    public int blipFrequency = 2;

    private WorldDialogueLine[] currentLines;
    private int currentIndex = 0;
    private Coroutine typingCoroutine;

    private bool isTyping = false;
    private bool isDialogueActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(advanceKey) && !isTyping)
        {
            AdvanceDialogue();
        }
    }

    public void ShowDialogue(WorldDialogueData data)
    {
        if (data == null || data.lines.Length == 0)
            return;

        currentLines = data.lines;
        StartDialogue();
    }

    public void ShowDialogue(string singleLine)
    {
        currentLines = new WorldDialogueLine[] {
            new WorldDialogueLine { text = singleLine, typingSpeed = defaultTypingSpeed }
        };
        StartDialogue();
    }

    private void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        currentIndex = 0;
        isDialogueActive = true;

        if (playerMovement != null)
            playerMovement.SetCanMove(false);

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentIndex]));
    }

    IEnumerator TypeLine(WorldDialogueLine line)
    {
        isTyping = true;
        dialogueText.text = "";

        float speed = Mathf.Max(0.01f, line.typingSpeed);
        int letterIndex = 0;

        foreach (char c in line.text)
        {
            dialogueText.text += c;

            if (char.IsLetterOrDigit(c) && blipSound != null && audioSource != null && letterIndex % blipFrequency == 0)
            {
                audioSource.PlayOneShot(blipSound, blipVolume);
            }

            letterIndex++;
            yield return new WaitForSeconds(speed);
        }

        isTyping = false;
    }

    private void AdvanceDialogue()
    {
        currentIndex++;

        if (currentIndex < currentLines.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        dialoguePanel.SetActive(false);
        isTyping = false;
        isDialogueActive = false;

        if (playerMovement != null)
            playerMovement.SetCanMove(true);
    }

    public bool IsDialogueActive()
{
    return isDialogueActive;
}

}
