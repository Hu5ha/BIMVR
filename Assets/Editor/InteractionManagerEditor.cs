using UnityEngine;
using UnityEditor;  // Important for custom editor stuff
using System.Collections.Generic;

[CustomEditor(typeof(InteractionManager))]
public class InteractionManagerEditor : Editor
{
    // We'll keep a reference to the actual target (our MonoBehaviour)
    private InteractionManager manager;

    private void OnEnable()
    {
        manager = (InteractionManager)target;
    }

    public override void OnInspectorGUI()
    {
        // Draw the default stuff first (like buildingRoot, layerOrder, etc.)
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Component Slots (Dynamic)");

        // Safety check: if buildingComponents is empty, the dropdown won't have options
        if (manager.buildingComponents == null || manager.buildingComponents.Count == 0)
        {
            EditorGUILayout.HelpBox("No buildingComponents found. Please populate them.", MessageType.Warning);
            return;
        }

        // For each slot in manager.componentSlots, draw a dropdown
        for (int i = 0; i < manager.componentSlots.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Current selected component
            string currentComponent = manager.componentSlots[i];
            int currentIndex = manager.buildingComponents.IndexOf(currentComponent);
            if (currentIndex < 0) currentIndex = 0; // fallback

            // Make a popup with all buildingComponents
            int newIndex = EditorGUILayout.Popup($"Slot {i}",
                currentIndex,
                manager.buildingComponents.ToArray()
            );

            // If user picked a different index, update the slot
            if (newIndex != currentIndex)
            {
                manager.componentSlots[i] = manager.buildingComponents[newIndex];
                EditorUtility.SetDirty(manager);
            }

            // (Optional) "Remove Slot" button
            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                manager.componentSlots.RemoveAt(i);
                EditorUtility.SetDirty(manager);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        // "Add Slot" button
        if (GUILayout.Button("Add Slot"))
        {
            // For example, default the new slot to the first buildingComponent
            if (manager.buildingComponents.Count > 0)
            {
                manager.componentSlots.Add(manager.buildingComponents[0]);
            }
            else
            {
                manager.componentSlots.Add("wände");
            }
            EditorUtility.SetDirty(manager);
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Use the X/B button presses in Play Mode to hide/show all selected components in the current layer.\n" +
            "You can reorder or change these slots at runtime, but typically changes persist only in the Editor.",
            MessageType.Info
        );
    }
}
