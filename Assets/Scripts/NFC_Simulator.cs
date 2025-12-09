using System;
using UnityEngine;

public class NFC_Simulator : MonoBehaviour
{
    public static event Action OnEggEvent;
    public static event Action OnMilkEvent;
    public static event Action OnFlourEvent;

    public static event Action OnBlueberryEvent;
    public static event Action OnChocolateEvent;
    public static event Action OnRoseEvent;

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

    // toppings

    public void TriggerBlueberryEvent()
    {
        Debug.Log("Blueberry event triggered.");
        OnBlueberryEvent?.Invoke();
    }

    public void TriggerChocolateEvent()
    {
        Debug.Log("Chocolate event triggered.");
        OnChocolateEvent?.Invoke();
    }

    public void TriggerRoseEvent()
    {
        Debug.Log("Rose event triggered.");
        OnRoseEvent?.Invoke();
    }
}
