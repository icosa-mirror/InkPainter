using System;
using Es.InkPainter;
using UnityEngine;
using UnityEngine.Serialization;

public class BrushTipGenerator : MonoBehaviour
{
    public Shader brushShader;

    public Color color = Color.white;
    [Range(0, 1)] public float opacity = 1.0f;
    [Range(0, 1)] public float softness = 0.5f;
    [Range(0, 1)] public float aspect = 1.0f;
    [Range(0, 180)] public float angle = 0.0f;
    public int textureSize = 256;
    public GameObject debugQuad;
    public GameObject brushControllerGameObject;

    private Material brushMaterial;
    private RenderTexture brushTexture;
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int Opacity = Shader.PropertyToID("_Opacity");
    private static readonly int Softness = Shader.PropertyToID("_Softness");
    private static readonly int Aspect = Shader.PropertyToID("_Aspect");
    private static readonly int Angle = Shader.PropertyToID("_Angle");
    private ITextureBrushController brushController;

    void Awake()
    {
        // Initialize shader material
        brushMaterial = new Material(brushShader);
        brushController = brushControllerGameObject.GetComponent<ITextureBrushController>();

        // Create RenderTexture
        brushTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);
        brushTexture.enableRandomWrite = true;
        brushTexture.Create();

        // Update the brush texture with initial parameters
        UpdateBrushTexture();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        UpdateBrushTexture();
    }

    void UpdateBrushTexture()
    {
        if (brushMaterial == null) return;
        brushMaterial.SetColor(Color1, color);
        brushMaterial.SetFloat(Opacity, opacity);
        brushMaterial.SetFloat(Softness, softness);
        brushMaterial.SetFloat(Aspect, aspect);
        brushMaterial.SetFloat(Angle, angle);

        // Render the texture
        Graphics.Blit(null, brushTexture, brushMaterial);
        brushController.SetBrushTexture(brushTexture);
        if (debugQuad != null)
        {
            debugQuad.GetComponent<MeshRenderer>().material.mainTexture = brushTexture;
        }
    }

    void Update()
    {
        // Update the brush texture if parameters change
        if (Input.GetKeyDown(KeyCode.Space)) // Example trigger
        {
            UpdateBrushTexture();
        }
    }

    public RenderTexture GetBrushTexture()
    {
        return brushTexture;
    }
}