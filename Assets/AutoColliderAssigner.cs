using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AutoColliderAssigner : MonoBehaviour
{
    [Header("Building Root")]
    [Tooltip("Assign the top-level building (Beyerbau) object here.")]
    public Transform buildingRoot;

    [Header("Building Components")]
    [Tooltip("List of all building components for selection.")]
    public List<string> buildingComponents = new List<string>
    {
        "körper",
        "wände",
        "dächer",
        "modell",
        "geläander",
        "fenster",
        "türen",
        "leuchten",
        "decken",
        "treppen",
        "stützen",
        "skelettbau",
        "wall",
        "floor",
        "stair",
        "fundamente"
    };

    [Header("Collider Assignment Slots")]
    [Tooltip("Components to assign BoxColliders.")]
    public List<string> boxColliderComponents = new List<string> { "wände", "decken" };

    [Tooltip("Components to assign MeshColliders.")]
    public List<string> meshColliderComponents = new List<string> { "treppen" };

    [Header("Collider Properties")]
    [Tooltip("Set BoxCollider.isTrigger property.")]
    public bool boxCollidersAreTrigger = false;

    [Tooltip("Set MeshCollider.isTrigger property.")]
    public bool meshCollidersAreTrigger = false;

    [Tooltip("Set MeshCollider.convex property.")]
    public bool meshCollidersAreConvex = false;

    public void AssignColliders()
    {
        if (buildingRoot == null)
        {
            Debug.LogWarning("No buildingRoot assigned! Please set it in the Inspector.");
            return;
        }

        foreach (Transform layer in buildingRoot)
        {
            // Traverse each layer under buildingRoot (e.g., Eben, OG1, etc.)
            foreach (Transform componentGroup in layer)
            {
                // Check if the componentGroup's name matches a keyword
                string groupNameLower = componentGroup.name.ToLower();

                foreach (string keyword in buildingComponents)
                {
                    if (groupNameLower.Contains(keyword.ToLower()))
                    {
                        // Add colliders to all leaf nodes under this group
                        if (boxColliderComponents.Contains(keyword))
                        {
                            AddBoxCollidersToLeaves(componentGroup);
                        }
                        else if (meshColliderComponents.Contains(keyword))
                        {
                            AddMeshCollidersToLeaves(componentGroup);
                        }
                    }
                }
            }
        }

        Debug.Log("Colliders successfully assigned.");
    }

    public void RemoveAllColliders()
    {
        if (buildingRoot == null)
        {
            Debug.LogWarning("No buildingRoot assigned! Cannot remove colliders.");
            return;
        }

        int removed = 0;

        foreach (Collider col in buildingRoot.GetComponentsInChildren<Collider>())
        {
            DestroyImmediate(col); // Removes colliders immediately in Edit Mode
            removed++;
        }

        Debug.Log($"Removed {removed} colliders from '{buildingRoot.name}'.");
    }

    private void AddBoxCollidersToLeaves(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.childCount == 0) // Leaf node
            {
                if (child.GetComponent<Collider>() == null)
                {
                    BoxCollider boxCol = child.gameObject.AddComponent<BoxCollider>();
                    boxCol.isTrigger = boxCollidersAreTrigger;
                    Debug.Log($"Added BoxCollider to leaf node '{child.name}'");
                }
            }
            else
            {
                AddBoxCollidersToLeaves(child); // Recurse for non-leaf nodes
            }
        }
    }

    private void AddMeshCollidersToLeaves(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.childCount == 0) // Leaf node
            {
                if (child.GetComponent<Collider>() == null)
                {
                    MeshCollider meshCol = child.gameObject.AddComponent<MeshCollider>();
                    meshCol.convex = meshCollidersAreConvex;
                    meshCol.isTrigger = meshCollidersAreTrigger;
                    Debug.Log($"Added MeshCollider to leaf node '{child.name}'");
                }
            }
            else
            {
                AddMeshCollidersToLeaves(child); // Recurse for non-leaf nodes
            }
        }
    }
}
