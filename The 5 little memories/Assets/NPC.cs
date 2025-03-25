using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public PlayerMovement playerMovement; // Referência ao script de movimento do jogador

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
            return;

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        // Bloqueia o movimento do jogador
        if (playerMovement != null)
        {
            playerMovement.canMove = false;
        }

        UpdateDialogueUI();
        dialoguePanel.SetActive(true);
        PauseController.SetPause(true);

        StartCoroutine(Typeline());
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            DisplayCurrentLineImmediately();
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.isPlayerSpeaking.Length)
        {
            UpdateDialogueUI();
            StartCoroutine(Typeline());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator Typeline()
    {
        isTyping = true;
        dialogueText.SetText("");

        string currentLine = dialogueData.isPlayerSpeaking[dialogueIndex] ?
            dialogueData.playerDialogueLines[dialogueIndex] :
            dialogueData.dialogueLines[dialogueIndex];

        foreach (char letter in currentLine)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        // Avança automaticamente se configurado
        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    void UpdateDialogueUI()
    {
        if (dialogueData.isPlayerSpeaking[dialogueIndex])
        {
            nameText.SetText("Player"); // Nome do jogador
            portraitImage.sprite = null; // Ou adicione um retrato do jogador
        }
        else
        {
            nameText.SetText(dialogueData.npcName);
            portraitImage.sprite = dialogueData.npcPortrait;
        }
    }

    void DisplayCurrentLineImmediately()
    {
        string currentLine = dialogueData.isPlayerSpeaking[dialogueIndex] ?
            dialogueData.playerDialogueLines[dialogueIndex] :
            dialogueData.dialogueLines[dialogueIndex];

        dialogueText.SetText(currentLine);
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        PauseController.SetPause(false);

        // Libera o movimento do jogador
        if (playerMovement != null)
        {
            playerMovement.canMove = true;
        }
    }
}