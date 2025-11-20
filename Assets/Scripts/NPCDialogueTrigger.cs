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
    private bool dialoguePlaying = false;   // evita spam de E durante o diálogo

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

        // Jogador está dentro da distância
        if (distance <= interactionDistance)
        {
            if (!canTalk)
            {
                canTalk = true;
                AtualizarExclamacao();
            }

            // Só pode iniciar diálogo se não estiver rolando um
            if (Input.GetKeyDown(KeyCode.E) && !dialoguePlaying)
            {
                DialogueData dialogueToPlay = null;
                int playerProgress = dialogueSystem.GetLastDialogueCompleted();

                // Descobrir qual diálogo está liberado
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
                    dialoguePlaying = true;

                    // Espera o diálogo terminar e então atualiza o "!"
                    StartCoroutine(WaitDialogueEndAndRefreshExclamation());
                }
                else
                {
                    Debug.Log("Nenhum diálogo disponível para este NPC no momento.");
                }
            }
        }
        else
        {
            if (canTalk)
            {
                canTalk = false;

                // quando sai do range, sempre desativa o "!"
                if (exclamationMark != null)
                    exclamationMark.SetActive(false);
            }
        }
    }

    // Coroutine para detectar quando o diálogo terminou e então atualizar o ícone
    private System.Collections.IEnumerator WaitDialogueEndAndRefreshExclamation()
    {
        // Espera até o DialogueSystem desativar a caixa de texto (diálogo finalizado)
        while (!dialogueSystem.IsDialogueDisabled())
            yield return null;

        // marca que já não estamos em diálogo
        dialoguePlaying = false;

        // Recalcula se há diálogo disponível para este NPC
        // (se o próximo diálogo deste NPC foi liberado, AtualizarExclamacao irá reativar o "!")
        AtualizarExclamacao();
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

        // Só ativa o "!" se o jogador estiver no range e houver diálogo disponível
        if (canTalk)
            exclamationMark.SetActive(temDialogoDisponivel);
        else
            exclamationMark.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
