using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public Image portraitImage;

    [Header("Configuração")]
    public KeyCode advanceKey = KeyCode.E;

    private Coroutine typingCoroutine;
    private DialogueLine[] currentLines;
    private int currentIndex;
    private System.Action onDialogueComplete;

    public bool IsDialoguePlaying { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        TryReconnectUI();
        HidePanel();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"🌀 Cena carregada: {scene.name}");
        TryReconnectUI();
        HidePanel();
    }

    private void HidePanel()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
            Debug.Log("👁 Painel de diálogo ocultado.");
        }
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
        TryReconnectUI(); // reconecta sempre que for iniciar

        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("🛑 Nenhuma linha de diálogo fornecida.");
            return;
        }

        if (dialoguePanel == null || dialogueText == null || nameText == null)
        {
            Debug.LogError("❌ DialogueManager: UI ainda não atribuída. Verifique se os nomes dos objetos estão corretos.");
            return;
        }

        Debug.Log("✅ Ativando painel de diálogo...");
        dialoguePanel.SetActive(true);
        currentLines = lines;
        currentIndex = 0;
        IsDialoguePlaying = true;

        nameText.text = npcName;

        if (portraitImage != null && portrait != null)
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
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        IsDialoguePlaying = false;
        onDialogueComplete?.Invoke();
    }

    void TryReconnectUI()
    {
        GameObject canvas = GameObject.Find("Canvas (1)");
        if (canvas == null)
        {
            Debug.LogWarning("⚠️ Canvas não encontrado na cena.");
            return;
        }

        Transform panelTransform = canvas.transform.Find("DialoguePanel");
        if (panelTransform != null)
        {
            dialoguePanel = panelTransform.gameObject;
            dialogueText = panelTransform.Find("DialogueText")?.GetComponent<TMP_Text>();
            nameText = panelTransform.Find("NPCNameText")?.GetComponent<TMP_Text>();
            portraitImage = panelTransform.Find("Image")?.GetComponent<Image>();

            Debug.Log("✅ UI reconectada com sucesso.");
        }
        else
        {
            Debug.LogWarning("⚠️ Dialogue Panel não encontrado dentro do Canvas.");
        }
    }
}
