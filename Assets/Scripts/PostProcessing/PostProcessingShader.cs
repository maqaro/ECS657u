using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PostProcessingShader : MonoBehaviour
{
    public Material postProcessMaterial; // Assign your shader material here

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (postProcessMaterial != null)
        {
            // Apply the shader material to the screen
            Graphics.Blit(src, dest, postProcessMaterial);
        }
        else
        {
            // Pass the image through unchanged
            Graphics.Blit(src, dest);
        }
    }
}
