using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum STATE
{
    DISABLED,
    WAITING,
    TYPING,
    CHOOSING
}

public class DialogueSystem : MonoBehaviour
{
    [Header("Lista de DialogueData (Cenas)")]
    public List<DialogueData> dialogues;

    private int currentDialogueIndex = 0;
    private DialogueData currentDialogueData;

    private int currentText = 0;
    private bool finished = false;
    private int lastDialogueCompleted = 0; // 0 = nenhum diálogo feito ainda

    private TypeTextAnimation typeText;
    private DialogueUI dialogueUI;

    private STATE state;

    [Header("Escolhas de diálogo")]
    public GameObject choicesPanel; // painel onde os botões aparecerão
    public Button choiceButtonPrefab; // prefab de botão de escolha

    void Awake()
    {
        typeText = FindObjectOfType<TypeTextAnimation>();
        if (typeText != null)
            typeText.TypeFinished += OnTypeFinished;

        dialogueUI = FindObjectOfType<DialogueUI>();

        if (choicesPanel != null)
            choicesPanel.SetActive(false); // começa escondido
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

    public void StartDialogue(DialogueData data)
    {
        currentDialogueData = data;
        currentText = 0;
        finished = false;
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
        // Se houver escolhas, mostrar painel
        if (currentDialogueData != null && currentDialogueData.choices != null && currentDialogueData.choices.Count > 0)
        {
            ShowChoices(currentDialogueData.choices);
            return;
        }

        // Se não há escolhas, encerra normalmente
        if (dialogueUI != null)
            dialogueUI.Disable();

        state = STATE.DISABLED;
        currentText = 0;
        finished = false;
        currentDialogueIndex++;

        if (currentDialogueData != null)
            lastDialogueCompleted = currentDialogueData.dialogueOrder;

        currentDialogueData = null;
    }

    void ShowChoices(List<DialogueChoice> choices)
    {
        if (choicesPanel == null || choiceButtonPrefab == null) return;

        // Limpa botões antigos
        foreach (Transform child in choicesPanel.transform)
            Destroy(child.gameObject);

        choicesPanel.SetActive(true);
        state = STATE.CHOOSING;

        foreach (var choice in choices)
        {
            var button = Instantiate(choiceButtonPrefab, choicesPanel.transform);
            var tmp = button.GetComponentInChildren<TMPro.TMP_Text>();
            if (tmp != null)
                tmp.text = choice.choiceText;

            button.onClick.AddListener(() =>
            {
                choicesPanel.SetActive(false);
                if (choice.nextDialogue != null)
                {
                    StartDialogue(choice.nextDialogue);
                }
                else
                {
                    if (dialogueUI != null)
                        dialogueUI.Disable();
                    state = STATE.DISABLED;
                }
            });
        }
    }

    public void ResetDialogueSequence()
    {
        currentDialogueIndex = 0;
        currentDialogueData = null;
        currentText = 0;
        finished = false;
        state = STATE.DISABLED;
    }

    public int GetLastDialogueCompleted()
    {
        return lastDialogueCompleted;
    }
    public bool IsDialogueDisabled()
    {
        return state == STATE.DISABLED;
    }

}
