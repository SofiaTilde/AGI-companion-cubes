using UnityEngine;

public class WiggleObject : MonoBehaviour
{
    public float wiggleSpeed = 2f;   // how fast it wiggles
    public float wiggleAmount = 10f; // how many degrees left/right

    private float baseRotation;

    void Start()
    {
        // store the initial Y rotation
        baseRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        // calculate a smooth wiggle angle
        float wiggle = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount;

        // apply wiggle keeping world-space Y rotation
        Vector3 rot = transform.eulerAngles;
        rot.y = baseRotation + wiggle;
        transform.eulerAngles = rot;
    }
}
