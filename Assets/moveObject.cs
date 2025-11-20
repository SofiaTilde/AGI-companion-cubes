using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovePan : MonoBehaviour
{
    public float moveSpeed = 5f;      // X-Z plane
    public float verticalSpeed = 3f;  // Up/down
    public float rotationSpeed = 60f; // Z rotation (I/K)

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // important: we drive it manually
    }

    void FixedUpdate()
    {
        // --- Movement with arrow keys ---
        float moveX = Input.GetAxisRaw("Horizontal");   // Left/Right arrows
        float moveZ = Input.GetAxisRaw("Vertical");     // Up/Down arrows

        Vector3 move = new Vector3(moveX, 0f, moveZ) * moveSpeed;

        // --- Vertical movement with U/J ---
        float moveY = 0f;
        if (Input.GetKey(KeyCode.U))
            moveY = verticalSpeed;
        if (Input.GetKey(KeyCode.J))
            moveY = -verticalSpeed;

        move.y = moveY;

        // --- Rotation with I/K around Z axis ---
        float rotationZ = 0f;
        if (Input.GetKey(KeyCode.I))
            rotationZ = rotationSpeed;
        if (Input.GetKey(KeyCode.K))
            rotationZ = -rotationSpeed;

        // Apply movement & rotation via Rigidbody
        Vector3 newPosition = rb.position + move * Time.fixedDeltaTime;
        Quaternion newRotation = rb.rotation * Quaternion.Euler(0f, 0f, rotationZ * Time.fixedDeltaTime);

        rb.MovePosition(newPosition);
        rb.MoveRotation(newRotation);
    }
}
