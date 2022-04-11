using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script processing the incoming depth image from RealSense sensor
/// </summary>
public class DepthProcessing : MonoBehaviour
{
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

    /// <summary>
    /// Update called once per frame
    /// </summary>
    void Update()
    {
        // No incoming depth texture
        if (depthTexture == null)
            return;

        // Process depth image
        ProcessDepthIm();
    }

    /// <summary>
    /// Processes depth image and sets the texture of image that displays it
    /// - any pixels further than far plane will be transparent
    /// - any pixels closer than near plane will be transparent
    /// </summary>
    private void ProcessDepthIm()
    {
        Texture2D tx = ImageProcessor.TextureToTexture2D(depthTexture);
        resultTexture = new Texture2D(tx.width, tx.height); 

        // TODO optimize
        // Go through all pixels
        int count = 0;
        for (int w = 0; w < tx.width; w++)
        {
            for (int h = 0; h < tx.height; h++)
            {
                // Rescale depth information to meters - values stolen from realsense shader
                Color d = tx.GetPixel(w, h);
                float r = d.r; //r is unscaled depth, normalized to [0-1]
                float distMeters = r * 65536 * 0.001f; // to meters

                // If in relevant depth
                Color res = new Color(0, 0, 0, 0);
                if (distMeters > min && distMeters < max)
                {
                    res = new Color(0, 0, 0, 1);
                    count++;
                }
                resultTexture.SetPixel(w, h, res);
            }
        }

        resultTexture.Apply();
        bgImage.texture = resultTexture;
    }
}
