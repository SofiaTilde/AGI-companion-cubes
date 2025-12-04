using System;
using UnityEngine;
using System.Collections.Generic;

public class Order_Simulator : MonoBehaviour
{
    public static event Action OnBigEvent;
    public static event Action OnMediumEvent;
    public static event Action OnSmallEvent;

    public OrderingSystem ordering_system;

    public void TriggerBigEvent()
    {
        Debug.Log("Big pancake blueberries.");
        ordering_system.AddOrder("Big", new List<string> { "Blueberries" });

        OnBigEvent?.Invoke();
    }

    public void TriggerMediumEvent()
    {
        Debug.Log("Medium pancake blueberries.");
        ordering_system.AddOrder("Medium", new List<string> { "Blueberries" });

        OnMediumEvent?.Invoke();
    }

    public void TriggerSmallEvent()
    {
        Debug.Log("Small pancake blueberries.");
        ordering_system.AddOrder("Small", new List<string> { "Blueberries" });

        OnSmallEvent?.Invoke();
    }
}
