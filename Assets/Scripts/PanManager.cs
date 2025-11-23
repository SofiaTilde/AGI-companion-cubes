using UnityEngine;
using System.Collections;


public class PanManager : MonoBehaviour
{
    public int MAX_BATTER_UNITS = 3;       // minipancake, normal pancake, gigantic pancake.
    private float[] scales = new float[] { 0.03f, 0.06f, 0.1f }; // local scales for each type of pancake
    [SerializeField] private GameObject pancake; // this pancake is a prefab

    private bool has_pancake = false;
    public GameObject spawnedPancake;           // runtime instance

    public void AddBatter()
    {
        if (!has_pancake)
        {
            if (pancake == null)
            {
                Debug.LogWarning("PanManager: No pancake prefab assigned.");
                return;
            }

            // it is spawned a bit above the pan world position
            Vector3 spawnPos = transform.position + Vector3.up * 0.02f;

            spawnedPancake = Instantiate(pancake, spawnPos, Quaternion.identity);
            spawnedPancake.transform.localScale = Vector3.one * scales[0];
            spawnedPancake.transform.SetParent(transform); // set the pan as the parent (temporarily)
            has_pancake = true;

            spawnedPancake.GetComponent<PancakeData>().batter_units++;
        }
        else
        {
            if (spawnedPancake == null)
            {
                has_pancake = false;
                return;
            }

            int size = spawnedPancake.GetComponent<PancakeData>().batter_units;

            if (size < MAX_BATTER_UNITS)
            {
                // set local size of the pancake to scales[batter_units]
                //spawnedPancake.transform.localScale = Vector3.one * scales[size];
                //spawnedPancake.GetComponent<PancakeData>().batter_units++;

                float fromSize = scales[size - 1];
                float toSize = scales[size];

                spawnedPancake.GetComponent<PancakeData>().batter_units++;

                StopAllCoroutines(); 
                StartCoroutine(AnimateScale(
                    spawnedPancake.transform,
                    Vector3.one * fromSize,
                    Vector3.one * toSize,
                    1f // duration in seconds
                ));
            }

        }
    }

    public void ReleasePancake()
    {
        // disable kinematic behaviour
        if (spawnedPancake == null)
        {
            Debug.Log("PanManager: Pancake is null");
            return;
        }
            
        // the rigid body is in the second child
        Transform rbHolder = spawnedPancake.transform.GetChild(1);
        if (rbHolder == null)
        {
            Debug.Log("PanManager: Pancake does not have second child");
            return;
        }
            
        Rigidbody rb = rbHolder.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("PanManager: Second child has no Rigidbody component.");
            return;
        }

        spawnedPancake.transform.SetParent(null);
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pancake"))
        {
            has_pancake = true;
            Debug.Log("has pancake = TRUE");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pancake"))
        {
            has_pancake = false;
            spawnedPancake = null;
            Debug.Log("has pancake = FALSE");
        }
    }


    private IEnumerator AnimateScale(Transform target, Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            target.localScale = Vector3.Lerp(from, to, lerp);

            yield return null;
        }

        target.localScale = to;
    }


}
