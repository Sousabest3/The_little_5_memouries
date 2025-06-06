using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public Image portraitImage;

    public KeyCode advanceKey = KeyCode.E;

    private Coroutine typingCoroutine;
    private DialogueLine[] currentLines;
    private int currentIndex;
    private System.Action onDialogueComplete;

    public bool IsDialoguePlaying { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (IsDialoguePlaying && Input.GetKeyDown(advanceKey))
        {
            NextLine();
        }
    }

    public void StartDialogue(DialogueLine[] lines, string npcName, Sprite portrait, System.Action onFinish = null)
    {
        dialoguePanel.SetActive(true);
        currentLines = lines;
        currentIndex = 0;
        IsDialoguePlaying = true;
        nameText.text = npcName;
        portraitImage.sprite = portrait;
        onDialogueComplete = onFinish;
        ShowLine();
    }

    void ShowLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        DialogueLine line = currentLines[currentIndex];
        typingCoroutine = StartCoroutine(TypeText(line));
    }

    IEnumerator TypeText(DialogueLine line)
    {
        dialogueText.text = "";
        foreach (char c in line.text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(line.typingSpeed);
        }

        if (line.autoProgress)
        {
            yield return new WaitForSeconds(line.autoProgressDelay);
            NextLine();
        }
    }

    void NextLine()
    {
        currentIndex++;
        if (currentIndex < currentLines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        IsDialoguePlaying = false;
        onDialogueComplete?.Invoke();
    }
}
