using System;
using UnityEngine;

[Serializable]
public class DialogueChoice
{
    [Tooltip("Texto que será mostrado na opção (ex: 'Ajudar o NPC')")]
    public string choiceText;

    [Tooltip("Próximo conjunto de falas que será exibido se o jogador escolher essa opção")]
    public DialogueData nextDialogue;
}
