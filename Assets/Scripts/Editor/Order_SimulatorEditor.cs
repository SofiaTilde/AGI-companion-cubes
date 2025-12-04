using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Order_Simulator))]
public class Order_SimulatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Order_Simulator sim = (Order_Simulator)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Order Big Pancake Blueberries"))
        {
            sim.TriggerBigEvent();
        }

        if (GUILayout.Button("Order Medium Pancake Blueberries"))
        {
            sim.TriggerMediumEvent();
        }

        if (GUILayout.Button("Order Small Pancake Blueberries"))
        {
            sim.TriggerSmallEvent();
        }
    }
}

