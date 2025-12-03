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

    public PanManager pan; 

    private bool pancakeInside = false;
    private float cookTimer = 0f;

    private Renderer pancakeRenderer;
    private bool triggered_burnt = false;
    private int cooking_state = 0; 

    private void Update()
    {
        if (!pancakeInside || pancakeRenderer == null)
        {
            pan.Stop_PS_cooking();
            return;
        }

        cookTimer += Time.deltaTime;

        // raw -> cooked -> burnt over time
        if (cookTimer >= timeToBurn)
        {
            if (!triggered_burnt)
            {
                pan.Trigger_PS_overcooked();
                triggered_burnt = true;
                cooking_state = 2;
            }
            
            pancakeRenderer.material = pancakeBurnt;
        }
        else if (cookTimer >= timeToCook)
        {
            cooking_state = 1;
            pancakeRenderer.material = pancakeCooked;
        }
        else
        {
            pan.Trigger_PS_cooking();
            cooking_state = 0;
            pancakeRenderer.material = pancakeRaw;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Pancake"))
            return;

        triggered_burnt = false;
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

        other.transform.gameObject.GetComponent<PancakeData>().state = cooking_state; // save if the pancake is burnt or not
        cooking_state = 0;
        pancakeInside = false;
        cookTimer = 0f;
        pancakeRenderer = null;

    }

}
