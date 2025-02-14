using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(AutoColliderAssigner))]
public class AutoColliderAssignerEditor : Editor
{
    private AutoColliderAssigner assigner;

    private void OnEnable()
    {
        assigner = (AutoColliderAssigner)target;
    }

    public override void OnInspectorGUI()
    {
        // Draw default Inspector fields (buildingRoot, etc.)
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Collider Assignment Slots", EditorStyles.boldLabel);

        // Ensure buildingComponents is populated
        if (assigner.buildingComponents == null || assigner.buildingComponents.Count == 0)
        {
            EditorGUILayout.HelpBox("No building components found. Please populate them in the script.", MessageType.Warning);
            return;
        }

        // --- BoxCollider Components ---
        EditorGUILayout.LabelField("BoxCollider Components", EditorStyles.boldLabel);
        DrawColliderSlots(ref assigner.boxColliderComponents, "BoxCollider");

        // --- MeshCollider Components ---
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("MeshCollider Components", EditorStyles.boldLabel);
        DrawColliderSlots(ref assigner.meshColliderComponents, "MeshCollider");

        // Add buttons for adding/removing colliders
        EditorGUILayout.Space();
        if (GUILayout.Button("Add Colliders"))
        {
            assigner.AssignColliders();
        }

        if (GUILayout.Button("Remove All Colliders"))
        {
            assigner.RemoveAllColliders();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Use this UI to configure which components should receive BoxColliders or MeshColliders.\n" +
            "You can also manually add or remove colliders using the buttons above.",
            MessageType.Info
        );

        // Apply changes to the serialized object
        if (GUI.changed)
        {
            EditorUtility.SetDirty(assigner);
        }
    }

    private void DrawColliderSlots(ref List<string> colliderSlots, string colliderType)
    {
        // Display all existing slots
        for (int i = 0; i < colliderSlots.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Current component in the slot
            string currentComponent = colliderSlots[i];
            int currentIndex = assigner.buildingComponents.IndexOf(currentComponent);
            if (currentIndex < 0) currentIndex = 0; // Fallback to first component

            // Dropdown for selecting components
            int newIndex = EditorGUILayout.Popup($"Slot {i + 1}",
                currentIndex,
                assigner.buildingComponents.ToArray()
            );

            // Update the slot if a new component is selected
            if (newIndex != currentIndex)
            {
                colliderSlots[i] = assigner.buildingComponents[newIndex];
            }

            // Remove slot button
            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                colliderSlots.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        // Add slot button
        if (GUILayout.Button($"Add {colliderType} Slot"))
        {
            if (assigner.buildingComponents.Count > 0)
            {
                colliderSlots.Add(assigner.buildingComponents[0]); // Default to the first component
            }
        }
    }
}
