using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExportObjectNamesByLayer : MonoBehaviour
{
    public Transform buildingRoot; // Root object of the building model
    public string exportFileName = "BuildingModelLayers.txt"; // Output file name

    private Dictionary<string, List<string>> layerObjectNames = new Dictionary<string, List<string>>();

    void Start()
    {
        // Start the export process
        ExportObjectNames();
    }

    void ExportObjectNames()
    {
        if (buildingRoot == null)
        {
            Debug.LogError("Building root is not assigned!");
            return;
        }

        // Traverse the hierarchy and group object names by layers
        TraverseHierarchy(buildingRoot);

        // Write the grouped data to a file
        WriteToFile();
    }

    void TraverseHierarchy(Transform parent)
    {
        foreach (Transform child in parent)
        {
            string layerName = child.name;

            // Add object names to the corresponding layer group
            if (!layerObjectNames.ContainsKey(layerName))
            {
                layerObjectNames[layerName] = new List<string>();
            }

            layerObjectNames[layerName].Add(child.name);

            // Recursively process child objects
            if (child.childCount > 0)
            {
                TraverseHierarchy(child);
            }
        }
    }

    void WriteToFile()
    {
        // Combine the path to save in the Assets folder
        string filePath = Path.Combine(Application.dataPath, exportFileName);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var layerEntry in layerObjectNames)
            {
                writer.WriteLine($"Layer: {layerEntry.Key}");
                foreach (var objectName in layerEntry.Value)
                {
                    writer.WriteLine($"    {objectName}");
                }
            }
        }

        Debug.Log($"Export complete. File saved at: {filePath}");
    }
}
