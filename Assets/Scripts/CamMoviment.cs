using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;       // Player a ser seguido
    public Vector3 offset = new Vector3(0f, 2f, -8f); // posição atrás e acima do player
    public float smoothSpeed = 0.125f;

    void FixedUpdate()
    {
        if (player == null) return;

        // posição desejada (player + offset)
        Vector3 desiredPosition = player.position + offset;

        // suavizar movimento
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;

        // câmera sempre olhando para o player
        transform.LookAt(player);
    }
}

