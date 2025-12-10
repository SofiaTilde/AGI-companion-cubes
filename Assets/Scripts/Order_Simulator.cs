using System;
using UnityEngine;
using System.Collections.Generic;

public class Order_Simulator : MonoBehaviour
{
    public static event Action OnBigEvent;
    public static event Action OnMediumEvent;
    public static event Action OnSmallEvent;

    public static event Action OnSmallChocolateEvent;
    public static event Action OnSmallAllEvent;

    public OrderingSystem ordering_system;

    public void TriggerBigEvent()
    {
        Debug.Log("Big pancake blueberries.");
        ordering_system.AddOrder("Big", new List<string> { "Blueberry" });

        OnBigEvent?.Invoke();
    }

    public void TriggerMediumEvent()
    {
        Debug.Log("Medium pancake blueberries.");
        ordering_system.AddOrder("Medium", new List<string> { "Blueberry" });

        OnMediumEvent?.Invoke();
    }

    public void TriggerSmallEvent()
    {
        Debug.Log("Small pancake blueberries.");
        ordering_system.AddOrder("Small", new List<string> { "Blueberry" });

        OnSmallEvent?.Invoke();
    }

    public void TriggerSmallChocolateEvent()
    {
        Debug.Log("Small pancake chocolate.");
        ordering_system.AddOrder("Small", new List<string> { "Chocolate" });

        OnSmallChocolateEvent?.Invoke();
    }

    public void TriggerSmallAllEvent()
    {
        Debug.Log("Small pancake All.");
        ordering_system.AddOrder("Small", new List<string> { "Blueberry", "Chocolate", "Rose" });

        OnSmallAllEvent?.Invoke();
    }
}
