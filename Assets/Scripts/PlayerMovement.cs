using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // WASD / Flechas
        float h = Input.GetAxisRaw("Horizontal"); // X
        float v = Input.GetAxisRaw("Vertical");   // Z

        // Dirección en plano XZ
        Vector3 move = new Vector3(h, 0f, v).normalized;

        if (move.magnitude >= 0.1f)
        {
            controller.Move(move * moveSpeed * Time.deltaTime);

            // Opcional: rotar al personaje en dirección de movimiento
            transform.rotation = Quaternion.LookRotation(move);
        }

        // Gravedad
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
