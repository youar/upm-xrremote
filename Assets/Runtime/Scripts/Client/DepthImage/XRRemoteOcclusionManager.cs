using UnityEngine;

public class XRRemoteOcclusionManager : MonoBehaviour
{
    public Material occlusionMaterial; 
    public string targetLayerName = "XRRemote-Occlusion"; 
    
    
    void Start()
    {
        int targetLayer = LayerMask.NameToLayer(targetLayerName);
        if (targetLayer == -1)
        {
            Debug.LogWarning("Layer name not found!");
            return;
        }
        AssignMaterialToLayer(targetLayer);
    }

    void AssignMaterialToLayer(int targetLayer)
    {
        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allGameObjects)
        {
            if (go.layer == targetLayer)
            {
                Renderer renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = occlusionMaterial;
                }
            }
        }
    }
}


