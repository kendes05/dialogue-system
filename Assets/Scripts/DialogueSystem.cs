using System.Collections.Generic;
using UnityEngine;

public enum STATE
{
    DISABLED,
    WAITING,
    TYPING
}

public class DialogueSystem : MonoBehaviour
{
    [Header("Lista de DialogueData (Cenas)")]
    public List<DialogueData> dialogues;

    private int currentDialogueIndex = 0;
    private DialogueData currentDialogueData;

    private int currentText = 0;
    private bool finished = false;

    private TypeTextAnimation typeText;
    private DialogueUI dialogueUI;

    private STATE state;

    void Awake()
    {
        typeText = FindObjectOfType<TypeTextAnimation>();
        if (typeText != null)
            typeText.TypeFinished += OnTypeFinished;

        dialogueUI = FindObjectOfType<DialogueUI>();
    }

    void Start()
    {
        state = STATE.DISABLED;
    }

    void Update()
    {
        if (state == STATE.DISABLED) return;

        switch (state)
        {
            case STATE.WAITING:
                Waiting();
                break;
            case STATE.TYPING:
                Typing();
                break;
        }
    }

    void OnTypeFinished()
    {
        state = STATE.WAITING;
    }

    public void StartDialogue()
    {
        if (state != STATE.DISABLED) return;
        if (dialogues == null || dialogues.Count == 0) return;
        if (currentDialogueIndex >= dialogues.Count) return;

        currentDialogueData = dialogues[currentDialogueIndex];
        currentText = 0;
        finished = false;

        if (dialogueUI != null)
            dialogueUI.Enable();

        Next();
    }

    public void Next()
    {
        if (currentDialogueData == null) return;

        if (currentDialogueData.talkScript == null || currentDialogueData.talkScript.Count == 0)
        {
            FinishedDialogue();
            return;
        }

        if (currentText >= currentDialogueData.talkScript.Count)
        {
            finished = true;
            return;
        }

        var dialogueEntry = currentDialogueData.talkScript[currentText];
        if (dialogueUI != null)
            dialogueUI.SetName(dialogueEntry.name);
        typeText.fullText = dialogueEntry.text;

        currentText++;

        if (currentText == currentDialogueData.talkScript.Count)
            finished = true;

        typeText.StartTyping();
        state = STATE.TYPING;
    }

    void Waiting()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!finished)
                Next();
            else
                FinishedDialogue();
        }
    }

    void Typing()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            typeText.Skip();
            state = STATE.WAITING;
        }
    }

    void FinishedDialogue()
    {
        if (dialogueUI != null)
            dialogueUI.Disable();

        state = STATE.DISABLED;
        currentText = 0;
        finished = false;
        currentDialogueIndex++;
        currentDialogueData = null;
    }

    public void ResetDialogueSequence()
    {
        currentDialogueIndex = 0;
        currentDialogueData = null;
        currentText = 0;
        finished = false;
        state = STATE.DISABLED;
    }
}
