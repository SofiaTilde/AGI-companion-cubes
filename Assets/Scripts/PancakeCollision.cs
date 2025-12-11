using UnityEngine;

// this behavior is built into ChimeraPancake.
public class PancakeCollision : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Destroy(gameObject, 60f); // destroy after 60 seconds
        }
    }
}

