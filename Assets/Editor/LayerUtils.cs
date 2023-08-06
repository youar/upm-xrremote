using UnityEngine;

public static class LayerUtils
{
    // Dictionary to store layer names and their corresponding indices
    private static readonly System.Collections.Generic.Dictionary<string, int> layerNameToIndex = new System.Collections.Generic.Dictionary<string, int>();

    // Method to get the layer index from the layer name
    public static int NameToLayer(string layerName)
    {
        // Check if the layer name is already cached
        if (layerNameToIndex.TryGetValue(layerName, out int layerIndex))
        {
            return layerIndex;
        }
        else
        {
            // If not cached, find the layer index and cache it
            for (int i = 0; i < 32; i++)
            {
                string name = LayerMask.LayerToName(i);
                if (name == layerName)
                {
                    layerNameToIndex[layerName] = i;
                    return i;
                }
            }

            // If layer name not found, return -1 (invalid layer index)
            return -1;
        }
    }
}
