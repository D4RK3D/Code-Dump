using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PixelationEffect : MonoBehaviour
{
    [Header("Render Resolution")]
    public int renderWidth = 320;
    public int renderHeight = 180;

    private RenderTexture pixelTexture;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        CreateRenderTexture();
    }

    void CreateRenderTexture()
    {
        pixelTexture = new RenderTexture(
            renderWidth,
            renderHeight,
            24,
            RenderTextureFormat.Default
        );

        pixelTexture.filterMode = FilterMode.Point;
        pixelTexture.useMipMap = false;
        pixelTexture.autoGenerateMips = false;
        pixelTexture.Create();

        cam.targetTexture = pixelTexture;
    }

    void OnGUI()
    {
        if (pixelTexture == null)
            return;

        GUI.DrawTexture(
            new Rect(0, 0, Screen.width, Screen.height),
            pixelTexture,
            ScaleMode.StretchToFill,
            false
        );
    }

    void OnDestroy()
    {
        if (pixelTexture != null)
        {
            pixelTexture.Release();
            Destroy(pixelTexture);
        }

        if (cam != null)
            cam.targetTexture = null;
    }
}
