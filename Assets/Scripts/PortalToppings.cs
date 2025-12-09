using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class PortalToppings : MonoBehaviour
{
    [Header("Toppings Prefabs")]
    public GameObject blueberries_small;
    public GameObject blueberries_medium;
    public GameObject blueberries_big;

    public GameObject chocolate_small;
    public GameObject chocolate_medium;
    public GameObject chocolate_big;

    public GameObject rose_small;
    public GameObject rose_medium;
    public GameObject rose_big;

    [Header("Pan reference")]
    public PanManager pan;

    private void OnEnable()
    {
        // subscribe to events
        NFC_Simulator.OnBlueberryEvent += TriggerBlueberry;
        NFC_Simulator.OnChocolateEvent += TriggerChocolate;
        NFC_Simulator.OnRoseEvent += TriggerRose;
    }

    private void OnDisable()
    {
        // unsubscribe from events
        NFC_Simulator.OnBlueberryEvent -= TriggerBlueberry;
        NFC_Simulator.OnChocolateEvent -= TriggerChocolate;
        NFC_Simulator.OnRoseEvent -= TriggerRose;
    }

    public void TriggerBlueberry()
    {
        SpawnTopping("Blueberry",
            blueberries_small,
            blueberries_medium,
            blueberries_big
        );
    }

    public void TriggerChocolate()
    {
        SpawnTopping("Chocolate",
            chocolate_small,
            chocolate_medium,
            chocolate_big
        );
    }

    public void TriggerRose()
    {
        SpawnTopping("Rose",
            rose_small,
            rose_medium,
            rose_big
        );
    }

    private void SpawnTopping(string toppingName,
                          GameObject prefabSmall,
                          GameObject prefabMedium,
                          GameObject prefabBig)
    {
        Quaternion rot = Quaternion.Euler(90f, 0f, 0f);

        // no pancake in pan
        if (pan.spawnedPancake == null)
        {
            Vector3 spawnPos = transform.position;
            Instantiate(prefabMedium, spawnPos, rot);
            return;
        }

        // Pancake exists
        Transform parent = pan.spawnedPancake.transform;
        PancakeData data = parent.GetComponent<PancakeData>();

        // Select prefab
        GameObject prefabToSpawn = prefabSmall;
        Debug.Log("Batter units  = " + data.batter_units);
        switch (data.batter_units)
        {
            case 0: prefabToSpawn = prefabSmall; break;
            case 1: prefabToSpawn = prefabMedium; break;
            case 2: prefabToSpawn = prefabBig; break;
        }

        // Spawn without parent first
        Vector3 spawnPosFinal = transform.position;
        GameObject spawned = Instantiate(prefabToSpawn, spawnPosFinal, rot);

        // Set parent **without changing world position/scale**
        spawned.transform.SetParent(parent, worldPositionStays: true);


        data.list_toppings.Add(toppingName);
    }



}

