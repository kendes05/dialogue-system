using UnityEngine;

public class PlayerMoviment : MonoBehaviour
{
    public float velocidade = 5f;
    private Rigidbody rb;

    DialogueSystem dialogueSystem;

    private void Awake(){
        dialogueSystem = FindObjectOfType<DialogueSystem>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movimento = new Vector3(horizontal,0f,vertical);

        rb.MovePosition(rb.position + movimento*velocidade*Time.fixedDeltaTime);

    }
}
