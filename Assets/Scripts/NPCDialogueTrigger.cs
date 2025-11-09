using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NPCDialogueTrigger : MonoBehaviour
{
    public DialogueSystem dialogueSystem; // referência ao DialogueSystem
    public GameObject exclamationMark;    // ícone "!" sobre o NPC
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

        if (distance <= interactionDistance)
        {
            if (!canTalk)
            {
                canTalk = true;
                if (exclamationMark != null)
                    exclamationMark.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                dialogueSystem.StartDialogue();
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
