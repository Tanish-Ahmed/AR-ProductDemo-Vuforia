using System.Collections.Generic;
using UnityEngine;

public class ProductController : MonoBehaviour
{
    [Header("Assign your top-level model (e.g., ShoeModel)")]
    public GameObject productRoot;

    [Header("Rotation")]
    public float rotationSpeed = 60f;   // degrees per second
    private bool rotating = false;

    [Header("Scaling")]
    public float scaleMultiplier = 1.5f;
    private Vector3 originalScale;
    private bool scaledUp = false;

    [Header("Color Palette")]
    public Color[] colors = { Color.white, Color.red, Color.blue, Color.green, Color.black };
    private int colorIndex = 0;

    // Cached renderers + MPBs for performance
    private List<Renderer> renderers = new List<Renderer>();
    private MaterialPropertyBlock mpb;
    private static readonly int COLOR_PROP_HDRP = Shader.PropertyToID("_BaseColor");
    private static readonly int COLOR_PROP_BUILTIN = Shader.PropertyToID("_Color");

    void Awake()
    {
        if (productRoot == null)
        {
            Debug.LogError("[ProductController] Assign productRoot in Inspector.");
            enabled = false;
            return;
        }

        originalScale = productRoot.transform.localScale;

        // Collect all renderers under the product
        renderers.Clear();
        renderers.AddRange(productRoot.GetComponentsInChildren<Renderer>(true));

        mpb = new MaterialPropertyBlock();

        // Initialize to first color so visuals are consistent
        ApplyColor(colors[colorIndex]);
    }

    void Update()
    {
        if (rotating)
        {
            productRoot.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
        }
    }

    // --- UI Hooks (assign these to Button OnClick) ---

    public void ToggleRotate()
    {
        rotating = !rotating;
    }

    public void ToggleScale()
    {
        scaledUp = !scaledUp;
        productRoot.transform.localScale = scaledUp
            ? originalScale * scaleMultiplier
            : originalScale;
    }

    public void NextColor()
    {
        if (colors == null || colors.Length == 0) return;
        colorIndex = (colorIndex + 1) % colors.Length;
        ApplyColor(colors[colorIndex]);
    }

    // --- Helpers ---

    private void ApplyColor(Color c)
    {
        foreach (var r in renderers)
        {
            if (r == null) continue;

            // Read current MPB, set color on both common props to cover URP/Built-in/HDRP
            r.GetPropertyBlock(mpb);

            // Try HDRP/URP "_BaseColor" first
            mpb.SetColor(COLOR_PROP_HDRP, c);
            // Also set legacy built-in "_Color"
            mpb.SetColor(COLOR_PROP_BUILTIN, c);

            r.SetPropertyBlock(mpb);
        }
    }
}
