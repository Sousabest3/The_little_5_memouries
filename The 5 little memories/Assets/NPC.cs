using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public PlayerMovement playerMovement;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    public bool CanInteract() => !isDialogueActive;

    public void Interact()
    {
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
            return;

        if (isDialogueActive) NextLine();
        else StartDialogue();
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        playerMovement?.SetCanMove(false);
        
        UpdateDialogueUI();
        dialoguePanel.SetActive(true);
        PauseController.SetPause(true);
        StartCoroutine(Typeline());
    }

    void UpdateDialogueUI()
    {
        var currentLine = dialogueData.dialogueLines[dialogueIndex];
        
        if (currentLine.isPlayerSpeaking)
        {
            nameText.SetText("Mari");
            portraitImage.sprite = currentLine.playerExpression ?? dialogueData.defaultPlayerExpression;
        }
        else
        {
            nameText.SetText(dialogueData.npcName);
            portraitImage.sprite = currentLine.npcExpression ?? dialogueData.defaultNPCExpression;
        }
    }

    IEnumerator Typeline()
    {
        isTyping = true;
        dialogueText.SetText("");
        var currentLine = dialogueData.dialogueLines[dialogueIndex];

        foreach (char letter in currentLine.text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(currentLine.typingSpeed);
        }

        isTyping = false;

        if (currentLine.autoProgress)
        {
            yield return new WaitForSeconds(currentLine.autoProgressDelay);
            NextLine();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        PauseController.SetPause(false);
        playerMovement?.SetCanMove(true);
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex].text);
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            UpdateDialogueUI();
            StartCoroutine(Typeline());
        }
        else
        {
            EndDialogue();
        }
    }
}