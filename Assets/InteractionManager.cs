using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Header("Building Hierarchy")]
    public Transform buildingRoot;

    public List<string> layerOrder = new List<string>
    {
        "Eben", "DG2", "DG1", "OG2", "OG1", "EG", "SG", "KG"
    };
    private int currentLayerIndex = 0;

    [Header("Possible Building Components")]
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
        "fundamente"
    };

    [Header("Dynamic Component Slots")]
    public List<string> componentSlots = new List<string> { "wände" };

    private Dictionary<string, List<Transform>> layerObjects
        = new Dictionary<string, List<Transform>>();
    private Dictionary<string, Dictionary<string, List<Transform>>> layerComponents
        = new Dictionary<string, Dictionary<string, List<Transform>>>();

    private bool inFreeViewMode = false;
    private int nextSlotToHideIndex = 0;
    private Stack<int> hiddenSlotsStack = new Stack<int>();

    void Start()
    {
        CacheLayerObjects();
        CacheLayerComponents();
        Debug.Log("Game started. All objects are visible by default.");
    }

    void Update()
    {
        // Toggle freeViewMode with Start/Menu (joystick button 7)
        if (Input.GetKeyDown("joystick button 7"))
        {
            ToggleFreeViewMode();
        }

        HandleCAVEInput();
    }

    private void HandleCAVEInput()
    {
        if (inFreeViewMode)
        {
            // A (0): Hide current layer, move to next
            if (Input.GetKeyDown("joystick button 0"))
            {
                HideCurrentLayerAndMoveNext();
            }

            // Y (3): Show current layer, move to previous
            if (Input.GetKeyDown("joystick button 3"))
            {
                ShowCurrentLayerAndMovePrevious();
            }

            // X (2): Hide the next slot in sequence
            if (Input.GetKeyDown("joystick button 2"))
            {
                HideNextSlot();
            }

            // B (1): Show the last hidden slot
            if (Input.GetKeyDown("joystick button 1"))
            {
                ShowLastHiddenSlot();
            }
        }
    }

    public void ToggleFreeViewMode()
    {
        inFreeViewMode = !inFreeViewMode;
        Debug.Log("Free View Mode: " + inFreeViewMode);

        if (inFreeViewMode)
        {
            currentLayerIndex = 0;
            Debug.Log("Entering Free View Mode. Default layer = Eben.");
            nextSlotToHideIndex = 0;
            hiddenSlotsStack.Clear();
        }
    }

    private void HideNextSlot()
    {
        if (componentSlots.Count == 0) return;

        int slotIndex = nextSlotToHideIndex;
        string layerName = layerOrder[currentLayerIndex];
        string componentName = componentSlots[slotIndex];

        SetComponentVisibility(layerName, componentName, false);
        hiddenSlotsStack.Push(slotIndex);

        nextSlotToHideIndex = (nextSlotToHideIndex + 1) % componentSlots.Count;
    }

    private void ShowLastHiddenSlot()
    {
        if (hiddenSlotsStack.Count == 0) return;

        string layerName = layerOrder[currentLayerIndex];
        int lastHiddenIndex = hiddenSlotsStack.Pop();
        string componentName = componentSlots[lastHiddenIndex];
        SetComponentVisibility(layerName, componentName, true);
    }

    private void HideCurrentLayerAndMoveNext()
    {
        SetLayerVisibility(false);
        currentLayerIndex = Mathf.Min(currentLayerIndex + 1, layerOrder.Count - 1);
        Debug.Log("Moved to layer: " + layerOrder[currentLayerIndex]);
    }

    private void ShowCurrentLayerAndMovePrevious()
    {
        SetLayerVisibility(true);
        currentLayerIndex = Mathf.Max(currentLayerIndex - 1, 0);
        Debug.Log("Moved to layer: " + layerOrder[currentLayerIndex]);
    }

    private void SetLayerVisibility(bool visible)
    {
        string currentLayerName = layerOrder[currentLayerIndex];
        if (!layerObjects.ContainsKey(currentLayerName)) return;

        foreach (Transform obj in layerObjects[currentLayerName])
        {
            obj.gameObject.SetActive(visible);
        }
    }

    private void SetComponentVisibility(string layerName, string componentName, bool visible)
    {
        if (!layerComponents.ContainsKey(layerName)) return;
        if (!layerComponents[layerName].ContainsKey(componentName)) return;

        foreach (Transform obj in layerComponents[layerName][componentName])
        {
            obj.gameObject.SetActive(visible);
        }

        Debug.Log($"Set '{componentName}' in {layerName} to {(visible ? "VISIBLE" : "HIDDEN")}");
    }

    private void CacheLayerObjects()
    {
        foreach (Transform child in buildingRoot)
        {
            string processedName = child.name.ToLower();
            foreach (string layer in layerOrder)
            {
                if (processedName == layer.ToLower())
                {
                    if (!layerObjects.ContainsKey(layer))
                    {
                        layerObjects[layer] = new List<Transform>();
                    }
                    layerObjects[layer].Add(child);
                }
            }
        }
    }

    private void CacheLayerComponents()
    {
        foreach (string layer in layerOrder)
        {
            layerComponents[layer] = new Dictionary<string, List<Transform>>();
        }

        foreach (var kvp in layerObjects)
        {
            string layerName = kvp.Key;
            foreach (Transform layerTransform in kvp.Value)
            {
                foreach (Transform componentCandidate in layerTransform)
                {
                    string compNameLower = componentCandidate.name.ToLower();
                    foreach (string bc in buildingComponents)
                    {
                        if (compNameLower.Contains(bc.ToLower()))
                        {
                            if (!layerComponents[layerName].ContainsKey(bc))
                            {
                                layerComponents[layerName][bc] = new List<Transform>();
                            }
                            layerComponents[layerName][bc].Add(componentCandidate);
                        }
                    }
                }
            }
        }
    }
}