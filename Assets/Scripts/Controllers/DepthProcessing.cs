using System;
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
    /// <summary> Color image displaying the texture </summary>
    [SerializeField]
    RawImage bgColorImage;
    /// <summary> Incoming depth texture </summary>
    [SerializeField]
    Texture depthTexture { get; set; }
    /// <summary> Processed depth texture </summary>
    Texture2D resultTexture;

    /// <summary> Min depth (near plane) </summary>
    float min;
    /// <summary> Max depth (far plane) </summary>
    float max;

    [Header("Helper variables")]
    /// <summary> Array with color pixels </summary>
    Color[] pixels;
    /// <summary> Texture used for transformations </summary>
    Texture2D tx;
    /// <summary> Depth scale </summary>
    float depthScale = 0.001f;
    /// <summary> Is color coding on or not </summary>
    internal bool colorOn;

    public float Min { get => min;
                       set { bgColorImage.material.SetFloat("_MinRange", value); min = value; } }
    public float Max { get => max;
                       set { bgColorImage.material.SetFloat("_MaxRange", value); max = value; } }

    /// <summary>
    /// Performed once upon start
    /// </summary>
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
        // If color coding active, then processed through shader
        //if (colorOn)
        //    return;

        tx = ImageProcessor.TextureToTexture2D(depthTexture);
        resultTexture.Reinitialize(tx.width, tx.height); // = new Texture2D(tx.width, tx.height);

        pixels = tx.GetPixels();

        // Min and max visible valuable
        double cmpMax = Max / (0xffff * depthScale);
        double cmpMin = Min / (0xffff * depthScale);

        // Go through all pixels
        for (int i = 0; i < pixels.Length; i++)
        {
            Color res = new Color(0, 0, 0, 0);
            Color d = pixels[i];

            float r = d.r; //r is unscaled depth, normalized to [0-1]
            // double distMeters = r * 0xffff * 0.001f; // to meters
            
            // Is depth in visible field
            if (r > cmpMin && r < cmpMax)
                res = new Color(0, 0, 0, 1);

            pixels[i] = res;
        }
        resultTexture.SetPixels(pixels);
        resultTexture.Apply();

        bgImage.texture = resultTexture;
    }

    internal float DepthToMeters(float r)
    {
        return r * 0xffff * depthScale;
    }

    internal int IsInInterval(float r)
    {
        // Min and max visible valuable
        double cmpMax = Max / (0xffff * depthScale);
        double cmpMin = Min / (0xffff * depthScale);

        Debug.Log(r);

        // Is depth in visible field
        if (r > cmpMin && r < cmpMax)
            return 1;

        return 0;
    }
}
