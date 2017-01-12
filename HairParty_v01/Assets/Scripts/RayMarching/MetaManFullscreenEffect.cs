using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MetaManFullscreenEffect : MonoBehaviour
{

    //public float intensity;
    private Material material;

    // Creates a private material used to the effect
    void Awake()
    {
        material = new Material(Shader.Find("Custom/Meta-Man"));
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //material.SetFloat("_bwBlend", intensity);
        Graphics.Blit(source, destination, material);
    }
}