using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 45f; // degrees per second

    void Update()
    {
        // Rotate around the world's Y axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
