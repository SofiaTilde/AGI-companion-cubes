using UnityEngine;

public class PanManager : MonoBehaviour
{
    public int batter_units = 0;           // this represents how big the pancake is. Every 2 seconds, the dispenser gives 1 unit of batter.
    public int MAX_BATTER_UNITS = 3;       // minipancake, normal pancake, gigantic pancake.
 
    private float[] scales = new float[] { 0.03f, 0.06f, 0.1f }; // local scales for each type of pancake

    private bool has_pancake = false;

    [SerializeField] private GameObject pancake; // this pancake is a prefab
    private GameObject spawnedPancake;           // runtime instance

    public void AddBatter()
    {
        if (batter_units < MAX_BATTER_UNITS)
        {
            int index = batter_units; 

            if (!has_pancake)
            {
                if (pancake == null)
                {
                    Debug.LogWarning("PanManager: No pancake prefab assigned.");
                    return;
                }

                // it is spawned 0.05 units above the pan world position
                Vector3 spawnPos = transform.position + Vector3.up * 0.04f;

                // by default it is spawned as kinematic
                spawnedPancake = Instantiate(pancake, spawnPos, Quaternion.identity);
                spawnedPancake.transform.localScale = Vector3.one * scales[index];

                has_pancake = true;
                batter_units++;
            }
            else
            {
                if (spawnedPancake == null)
                {
                    has_pancake = false;
                    return;
                }

                // set local size of the pancake to scales[batter_units]
                spawnedPancake.transform.localScale = Vector3.one * scales[index];
                batter_units++;
            }
        }
    }

    public void ReleasePancake()
    {
        if (spawnedPancake == null)
            return;

        // second child (index 1)
        Transform rbHolder = spawnedPancake.transform.GetChild(1);

        if (rbHolder == null)
            return;

        Rigidbody rb = rbHolder.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogWarning("PanManager: Second child has no Rigidbody component.");
            return;
        }

        rb.useGravity = true;
        rb.isKinematic = false;
    }

}
