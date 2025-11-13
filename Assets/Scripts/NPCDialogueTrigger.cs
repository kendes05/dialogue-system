using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class NPCDialogueTrigger : MonoBehaviour
{
    [System.Serializable]
    public class DialogueStage
    {
        public DialogueData dialogueData;  // Dados do diálogo
        public int requiredOrder;          // Ordem em que esse diálogo é desbloqueado
    }

    public DialogueSystem dialogueSystem;   // Referência ao DialogueSystem
    public List<DialogueStage> dialogueStages = new List<DialogueStage>();
    public GameObject exclamationMark;      // Ícone "!" sobre o NPC
    public float interactionDistance = 2f;

    private Transform player;
    private bool canTalk = false;

    void Start()
    {
        if (exclamationMark != null)
            exclamationMark.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogWarning("Player não encontrado. Coloque a tag 'Player' no seu objeto.");
    }

    void Update()
    {
        if (player == null || dialogueSystem == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Se o jogador estiver perto o suficiente
        if (distance <= interactionDistance)
        {
            if (!canTalk)
            {
                canTalk = true;
                AtualizarExclamacao();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Verifica qual diálogo está disponível de acordo com o progresso
                DialogueData dialogueToPlay = null;
                int playerProgress = dialogueSystem.GetLastDialogueCompleted();

                foreach (var stage in dialogueStages)
                {
                    if (playerProgress == stage.requiredOrder - 1)
                    {
                        dialogueToPlay = stage.dialogueData;
                        break;
                    }
                }

                if (dialogueToPlay != null)
                {
                    dialogueSystem.StartDialogue(dialogueToPlay);
                }
                else
                {
                    Debug.Log("Nenhum diálogo disponível neste momento para este NPC.");
                }
            }
        }
        else
        {
            if (canTalk)
            {
                canTalk = false;
                if (exclamationMark != null)
                    exclamationMark.SetActive(false);
            }
        }
    }

    void AtualizarExclamacao()
    {
        if (exclamationMark == null) return;

        bool temDialogoDisponivel = false;
        int playerProgress = dialogueSystem.GetLastDialogueCompleted();

        foreach (var stage in dialogueStages)
        {
            if (playerProgress == stage.requiredOrder - 1)
            {
                temDialogoDisponivel = true;
                break;
            }
        }

        exclamationMark.SetActive(temDialogoDisponivel);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
