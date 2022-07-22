using UnityEngine;

/// <summary>
/// Static helping class for the editing of textures
/// </summary>
public static class ImageProcessor
{
    /// <summary> Texture2D used for transformations </summary>
    public static Texture2D texture2D;
    /// <summary> Current render texture </summary>
    static RenderTexture currentRT;
    /// <summary> Render texture used for transformations </summary>
    static RenderTexture renderTexture;

    /// <summary>
    /// Converts Texture to Texture2D
    /// Uses static class's transformation texture as a holding variable
    /// </summary>
    /// <param name="texture"> Source texture </param>
    /// <returns> Resulting texture </returns>
    public static Texture2D TextureToTexture2D(Texture texture)
    {
        texture2D.Reinitialize(texture.width, texture.height);

        currentRT = RenderTexture.active;
        renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.ReleaseTemporary(renderTexture);
        RenderTexture.active = currentRT;

        UnityEngine.Object.Destroy(currentRT);
        return texture2D;
    }

    /// <summary>
    /// Creates a new texture used for transformations
    /// </summary>
    internal static void NewT()
    {
        if (texture2D != null)
            UnityEngine.Object.Destroy(texture2D);

        texture2D = new Texture2D(3, 3, TextureFormat.RGBA32, false);
    }

    /// <summary>
    /// Creates a new texture with new width and height using bilinear interpolation
    /// Uses static class's transformation texture as a holding variable
    /// </summary>
    /// <param name="source"> Source texture </param>
    /// <param name="targetWidth"> Target width </param>
    /// <param name="targetHeight"> Target height </param>
    /// <returns> Edited texture </returns>
    private static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        texture2D.Reinitialize(targetWidth, targetHeight);
        // Create new empty Texture
        //Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

        // Interpolate
        Color[] pixels = texture2D.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            Color newColor = source.GetPixelBilinear((float)(i % targetWidth) / (float)targetWidth, (float)(i / targetWidth) / (float)targetHeight);
            pixels[i] = newColor;
        }

        // Set new pixels
        texture2D.SetPixels(pixels);
        texture2D.Apply();
        return texture2D;
    }

    /// <summary>
    /// Creates a new texture with same data but different format
    /// </summary>
    /// <param name="oldTexture"> Source texture </param>
    /// <param name="newFormat"> New texture format </param>
    /// <returns> Edited texture </returns>
    private static Texture2D ChangeFormat(Texture2D oldTexture, TextureFormat newFormat)
    {
        //Create new empty Texture
        Texture2D newTex = new Texture2D(oldTexture.width, oldTexture.height, newFormat, false);
        
        //Copy old texture pixels into new one
        newTex.SetPixels(oldTexture.GetPixels());
        newTex.Apply();
        return newTex;
    }
}