using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NFC_Simulator))]
public class NFC_SimulatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NFC_Simulator sim = (NFC_Simulator)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Trigger Egg Event"))
        {
            sim.TriggerEggEvent();
        }

        if (GUILayout.Button("Trigger Milk Event"))
        {
            sim.TriggerMilkEvent();
        }

        if (GUILayout.Button("Trigger Flour Event"))
        {
            sim.TriggerFlourEvent();
        }
    }
}
