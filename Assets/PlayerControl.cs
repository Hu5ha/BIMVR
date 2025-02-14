using UnityEngine;
using System.Collections.Generic;
using Valve.VR;

public class PlayerControl : MonoBehaviour
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

    private int nextSlotToHideIndex = 0;
    private Stack<int> hiddenSlotsStack = new Stack<int>();
    // SteamVR Input Actions
    public SteamVR_Action_Vector2 leftTouchpadAction;  // Movement (Left Controller)
    public SteamVR_Action_Vector2 rightTouchpadAction; // Interaction (Right Controller)

    public SteamVR_Input_Sources leftHand = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;

    public float moveSpeed = 2.0f; // Movement speed
    private bool isInteracting = false; // Prevents continuous interaction triggers

    void Start()
    {
        CacheLayerObjects();
        CacheLayerComponents();
        Debug.Log("Game started. All objects are visible by default.");
    }


    private void Update()
    {
        HandleMovement();
        HandleBuildingInteraction();
    }

    private void HandleMovement()
    {
        Vector2 input = leftTouchpadAction.GetAxis(leftHand);
        if (input.magnitude > 0.2f) // Only move if there's a valid input
        {
            MovePlayer(input);
        }
    }

    private void MovePlayer(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;

        if (input.y > 0.2f) // Forward
            moveDirection += transform.forward;
        if (input.y < -0.2f) // Backward
            moveDirection -= transform.forward;
        if (input.x > 0.2f) // Right
            moveDirection += transform.right;
        if (input.x < -0.2f) // Left
            moveDirection -= transform.right;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void HandleBuildingInteraction()
    {
        Vector2 input = rightTouchpadAction.GetAxis(rightHand);

        if (input.magnitude < 0.2f) // Prevent unintended interactions when no input
        {
            isInteracting = false;
            return;
        }

        if (!isInteracting) // Prevent repeated triggers
        {
            if (input.y > 0.5f) // Up → Xbox Y
            {
                ShowCurrentLayerAndMovePrevious();
            }
            else if (input.y < -0.5f) // Down → Xbox A
            {
                HideCurrentLayerAndMoveNext();
            }
            else if (input.x < -0.5f) // Left → Xbox X
            {
                HideNextSlot();
            }
            else if (input.x > 0.5f) // Right → Xbox B
            {
                ShowLastHiddenSlot();
            }

            isInteracting = true; // Lock interaction until touchpad is released
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