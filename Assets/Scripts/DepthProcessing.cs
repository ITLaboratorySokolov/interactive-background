using System.Collections.Generic;
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

    Color[] pixels;
    Texture2D tx;

    /// <summary> Is color depth displayed </summary>
    internal bool colorOn;
        
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
        ProcessDepthIm();
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

        float toM = 0.001f;
        float cmpMax = max / (0xffff * toM);
        float cmpMin = min / (0xffff * toM);
        Debug.Log(cmpMax + " " + cmpMin);

        HashSet<float> set = new HashSet<float>();

        for (int i = 0; i < pixels.Length; i++)
        {
            Color res = new Color(0, 0, 0, 0);
            Color d = pixels[i];
            
            float r = d.r; //r is unscaled depth, normalized to [0-1]
            float distMeters = r * 0xffff * 0.001f; // to meters
            float z = (distMeters - min) / (max - min);
            if (r > cmpMin && r < cmpMax)
            {
                set.Add(z);
                res = new Color(0, 0, 0, 1);
                if (colorOn)
                    res = new Color(0, 0, z, 1);
                count++;
            }

            // If in relevant depth
            /*
            if (r > cmpMin && r < cmpMax)
            {

                // return tex2D(_Colormaps, float2(z, 1 - (_Colormap + 0.5) * _Colormaps_TexelSize.y));

                res = new Color(0, 0, 0, 1);
                if (colorOn)
                {
                    float m = (r - cmpMin) / (cmpMax - cmpMin); // should be 0 - 1
                    res = new Color(1 - m, 0, m, 1);
                }

                count++;
            }
            */

            pixels[i] = res;

        }
        resultTexture.SetPixels(pixels);
        resultTexture.Apply();

        Debug.Log(set.Count);

        bgImage.texture = resultTexture;
    }


}
