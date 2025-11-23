using UnityEngine;

public class CookPancake : MonoBehaviour
{
    [Header("Pancake Materials")]
    public Material pancakeRaw;
    public Material pancakeCooked;
    public Material pancakeBurnt;

    [Header("Cooking Times (seconds)")]
    public float timeToCook = 4f;   
    public float timeToBurn = 8f;   

    private bool pancakeInside = false;
    private float cookTimer = 0f;

    private Renderer pancakeRenderer;

    private void Update()
    {
        if (!pancakeInside || pancakeRenderer == null)
            return;

        cookTimer += Time.deltaTime;

        // raw -> cooked -> burnt over time
        if (cookTimer >= timeToBurn)
        {
            pancakeRenderer.material = pancakeBurnt;
        }
        else if (cookTimer >= timeToCook)
        {
            pancakeRenderer.material = pancakeCooked;
        }
        else
        {
            pancakeRenderer.material = pancakeRaw;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Pancake"))
            return;

        pancakeInside = true;
        cookTimer = 0f;

        // "the material is in the parent of the pancake, then the first child"
        // so: parent -> child[0] -> Renderer
        Transform parent = other.transform.parent != null ? other.transform.parent : other.transform;
        Transform firstChild = parent.childCount > 0 ? parent.GetChild(0) : parent;

        pancakeRenderer = firstChild.GetComponent<Renderer>();

        if (pancakeRenderer == null)
        {
            Debug.LogWarning("CookPancake: Could not find Renderer on parent/first child of pancake.");
        }
        else
        {
            pancakeRenderer.material = pancakeRaw;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Pancake"))
            return;

        pancakeInside = false;
        cookTimer = 0f;
        pancakeRenderer = null;
    }
}
