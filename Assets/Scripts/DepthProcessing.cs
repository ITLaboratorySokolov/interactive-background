using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script processing the incoming depth image from RealSense sensor
/// </summary>
public class DepthProcessing : MonoBehaviour
{
    private double timeToSnapshot;

    [Header("Depth image")]
    /// <summary> Image displaying the texture </summary>
    [SerializeField]
    RawImage bgImage;
    /// <summary> Incoming depth texture </summary>
    [SerializeField]
    Texture depthTexture { get; set; }
    /// <summary> Processed depth texture </summary>
    Texture2D resultTexture;

    /// <summary> Min depth (near plane) </summary>
    internal float min;
    /// <summary> Max depth (far plane) </summary>
    internal float max;

    Color[] pixels;
    Texture2D tx;

    private void Start()
    {
        ImageProcessor.NewT();
        resultTexture = new Texture2D(100, 100);
    }

    /// <summary>
    /// Update called once per frame
    /// </summary>
    void Update()
    {
        // No incoming depth texture
        if (depthTexture == null)
            return;

        // Process depth image
        // if (timeToSnapshot < 0.01)
        {
            ProcessDepthIm();
            timeToSnapshot = 1;
        }

        //timeToSnapshot -= Time.deltaTime;
    }

    /// <summary>
    /// Processes depth image and sets the texture of image that displays it
    /// - any pixels further than far plane will be transparent
    /// - any pixels closer than near plane will be transparent
    /// </summary>
    private void ProcessDepthIm()
    {
        tx = ImageProcessor.TextureToTexture2D(depthTexture);
        resultTexture.Reinitialize(tx.width, tx.height); // = new Texture2D(tx.width, tx.height);

        // Go through all pixels
        pixels = tx.GetPixels();
        int count = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            Color d = pixels[i];
            float r = d.r; //r is unscaled depth, normalized to [0-1]
            float distMeters = r * 65536 * 0.001f; // to meters

            // If in relevant depth
            Color res = new Color(0, 0, 0, 0);
            if (distMeters > min && distMeters < max)
            {
                res = new Color(0, 0, 0, 1);
                count++;
            }
            pixels[i] = res;
        }
        resultTexture.SetPixels(pixels);
        resultTexture.Apply();

        bgImage.texture = resultTexture;
    }
}
