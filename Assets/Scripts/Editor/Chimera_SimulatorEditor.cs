using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChimeraPlugin))]
public class ChimeraEditor : Editor
{
    private List<ChimeraPancake> pancakes = new();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var manager = (ChimeraPlugin)target;

        if (GUILayout.Button("Create a 5cm Pancake"))
        {   
            var pancake = manager.CreatePancakeAt();
            pancakes.Add(pancake.GetComponent<ChimeraPancake>());
        }

        if (GUILayout.Button("Prune all chimera pancakes"))
        {
            foreach (var pancake in pancakes)
            {
                pancake.DestroyPancake();
            }
            pancakes.Clear();
        }
    }
}
