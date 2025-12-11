using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanManager : MonoBehaviour
{
    public int MAX_BATTER_UNITS = 2;       // 0 minipancake, 1 normal pancake, 2 gigantic pancake.
    private float[] scales = new float[] { 0.03f, 0.06f, 0.1f }; // local scales for each type of pancake
    [SerializeField] private GameObject pancake; // this pancake is a prefab

    private bool has_pancake = false;
    public GameObject spawnedPancake;  // runtime instance

    public OrderingSystem ordering_system;

    // audio files
    [SerializeField] private List<AudioClip> audioFiles;// first is cooking, second is overcooked

    // particle system
    [SerializeField] private GameObject ps_splash;
    [SerializeField] private GameObject ps_cooking;
    [SerializeField] private GameObject ps_overcooked;

    private AudioSource audioSource;
    private int number_pancakes = 0;

    public bool pan_in_fire = false;

    public ChimeraPlugin chimeraManager = null;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void AddBatter()
    {
        if (chimeraManager == null)
        {
            Debug.LogWarning("Chimera Manager no properly set.");
            return;
        }
        if (!has_pancake)
        {
            //if (pancake == null)
            //{
            //    Debug.LogWarning("PanManager: No pancake prefab assigned.");
            //    return;
            //}

            // it is spawned a bit above the pan world position
            Vector3 spawnPos = transform.position + Vector3.up * 0.02f;

            chimeraManager.CreatePancakeAt(spawnPos, scales[0] * 0.1f);
            //spawnedPancake = Instantiate(pancake, spawnPos, Quaternion.identity);
            spawnedPancake.gameObject.name = "Pancake_" + number_pancakes.ToString();
            number_pancakes++;
            //spawnedPancake.transform.localScale = Vector3.one * scales[0]; 
            spawnedPancake.transform.SetParent(transform); //set the pan as the parent (temporarily)
            has_pancake = true;

            //spawnedPancake.GetComponent<PancakeData>().batter_units++;

            // particle system: splash
            ps_splash.SetActive(true);
            ps_splash.GetComponent<ParticleSystem>().Play();
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
                var pancakeData = spawnedPancake.GetComponent<PancakeData>();
                // increase batter units when isn't the max.
                pancakeData.batter_units++;

                // particle system: splash
                ps_splash.SetActive(true);
                ps_splash.GetComponent<ParticleSystem>().Play();

                //float fromSize = scales[size - 1];
                //float toSize = scales[size];

                //float fromSize = scales[size];
                //float toSize = scales[size+1];

                //StopAllCoroutines(); 
                //StartCoroutine(AnimateScale(
                //    spawnedPancake.transform,
                //    Vector3.one * fromSize,
                //    Vector3.one * toSize,
                //    1f // duration in seconds
                //));

                // for chimera pancake, no more animation here (NEED CONFIRMATION?).
                // just recreate a new pancake.
                var newBatterUnit = pancakeData.batter_units;
                spawnedPancake.GetComponent<ChimeraPancake>().DestroyPancake();

                // FIXME(Hengyi): refetch the position of the pan, but can be previous center of pancake.
                var position = transform.position;
                var previousPancakeName = spawnedPancake.name;
                spawnedPancake = chimeraManager.CreatePancakeAt(position, scales[newBatterUnit]);
                spawnedPancake.name = previousPancakeName;
                spawnedPancake.GetComponent<PancakeData>().batter_units = newBatterUnit;

                // FIXME(Hengyi): particle effects here?
            }
        }
    }

    // Hengyi: we dont need this anymore.
    public void ReleasePancake()
    {
        // disable kinematic behaviour
        if (spawnedPancake == null)
        {
            Debug.Log("PanManager: Pancake is null");
            return;
        }

        // the rigid body is in the second child
        Transform rbHolder = spawnedPancake.transform.GetChild(0);
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

    public void Trigger_PS_cooking()
    {
        if (pan_in_fire)
        {
            ps_cooking.SetActive(true);
            ps_cooking.GetComponent<ParticleSystem>().Play();

            audioSource.PlayOneShot(audioFiles[0]);
        }

    }

    public void Stop_PS_cooking()
    {
        ps_cooking.SetActive(false);
        audioSource.Stop();
    }

    public void Trigger_PS_overcooked()
    {
        if (pan_in_fire)
        {
            ps_overcooked.SetActive(true);
            ps_overcooked.GetComponent<ParticleSystem>().Play();
            audioSource.PlayOneShot(audioFiles[1]);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pancake"))
        {
            if (!has_pancake)
            {
                has_pancake = true;
                spawnedPancake = other.gameObject;
                Debug.Log("has pancake = TRUE");
            }

        }
        else if (other.CompareTag("Deleter"))
        {
            DestroyPancake(other);
        }
        else if (other.CompareTag("Fire"))
        {
            pan_in_fire = true;
        }

    }

    public void DestroyPancake(Collider other)
    {
        if (spawnedPancake == null && other == null)
        {
            Debug.Log("Pancake is null");
        }
        
        if (spawnedPancake != null)
        {
            spawnedPancake.GetComponent<ChimeraPancake>().DestroyPancake();
            //Destroy(spawnedPancake);
        }
        if (other != null)
        {
            // this branch is called from DeliveryPortalManager.onTriggerEnter
            if (other.gameObject.TryGetComponent<ChimeraPancake>(out var chimeraPancake))
            {
                chimeraPancake.DestroyPancake();
            }
            //Destroy(other.transform.parent.gameObject);
        }

        has_pancake = false;
        spawnedPancake = null;
        ordering_system.CurrentOrderFinished();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pancake"))
        {
            // when a pancake leave the pan.
            // this happens when a user DROPS a pancake.
            string current_name = null;

            if (spawnedPancake != null)
                current_name = spawnedPancake.name;

            string other_name = other.gameObject.name;

            if (current_name == other_name)
            {
                // no pancake in the pan
                has_pancake = false;
                // remove the latest spawned pancake reference.
                spawnedPancake = null;
                Debug.Log("has pancake = FALSE");
            }

        }
        else if (other.CompareTag("Fire"))
        {
            // pan leaves fire.
            pan_in_fire = false;
            Stop_PS_cooking();
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
