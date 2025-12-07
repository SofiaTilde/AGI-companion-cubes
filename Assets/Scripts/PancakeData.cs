using UnityEngine;
using System.Collections.Generic;

public class PancakeData : MonoBehaviour
{
    [Header("Pancake Materials")]
    public Material pancakeRaw;
    public Material pancakeCooked;
    public Material pancakeBurnt;

    public int batter_units = 0; // 0 is small, 1 is medium, 2 is big
    public int state = 0; // 0 is raw, 1 is cooked, 2 is burnt
    public List<string> list_toppings = new List<string>();

    // private data
    private Renderer pancakeRenderer;

    private void Start()
    {
        // the renderer is in the first child of the pancake"
        Transform firstChild = this.transform.GetChild(0);

        pancakeRenderer = firstChild.GetComponent<Renderer>();
        pancakeRenderer.material = pancakeRaw;
    }


    public void Cook()
    {
        if (state < 2)
            state++;

        if(state == 0)
            pancakeRenderer.material = pancakeRaw;
        else if(state == 1)
            pancakeRenderer.material = pancakeCooked;
        else if(state == 2)
            pancakeRenderer.material = pancakeBurnt;
    }
}
