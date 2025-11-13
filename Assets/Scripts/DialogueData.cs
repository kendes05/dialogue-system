using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Dialogue
{
    public string name;
    [TextArea(5, 10)]
    public string text;
}

[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObject/TalkScript", order = 1)]
public class DialogueData : ScriptableObject
{
    [Header("Fal falas deste diálogo")]
    public List<Dialogue> talkScript;

    [Header("Ordem cronológica do diálogo (ex: 1, 2, 3...)")]
    public int dialogueOrder = 1;

    [Header("Opções de escolha (opcional)")]
    public List<DialogueChoice> choices;
}
