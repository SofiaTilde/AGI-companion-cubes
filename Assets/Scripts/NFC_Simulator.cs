using System;
using UnityEngine;

public class NFC_Simulator : MonoBehaviour
{
    public static event Action OnEggEvent;
    public static event Action OnMilkEvent;
    public static event Action OnFlourEvent;

    public void TriggerEggEvent()
    {
        Debug.Log("Egg event triggered.");
        OnEggEvent?.Invoke();
    }

    public void TriggerMilkEvent()
    {
        Debug.Log("Milk event triggered.");
        OnMilkEvent?.Invoke();
    }

    public void TriggerFlourEvent()
    {
        Debug.Log("Flour event triggered.");
        OnFlourEvent?.Invoke();
    }
}
