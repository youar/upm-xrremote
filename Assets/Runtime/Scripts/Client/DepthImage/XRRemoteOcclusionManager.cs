using UnityEngine;

public class DepthTextureScript : MonoBehaviour
{
    public Material material;  // Assign the material using this shader in the Inspector.
    public Texture2D depthTexture;  // Assume this is your depth texture.

    void Start()
    {
        // Make sure to assign the material and depthTexture either in the Inspector or via script.
        if (material == null || depthTexture == null)
        {
            Debug.LogError("Material or DepthTexture is not set.");
            return;
        }

        // Set the shader's texture variable.
        material.SetTexture("_MainTex", depthTexture);
    }
}
