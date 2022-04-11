using UnityEngine;

/// <summary>
/// Static helping class for the editing of textures
/// </summary>
public static class ImageProcessor
{
    /// <summary>
    /// Converts Texture to Texture2D
    /// </summary>
    /// <param name="texture"> Source texture </param>
    /// <returns> Resulting texture </returns>
    public static Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);
        return texture2D;
    }

    /// <summary>
    /// Scales texture to new width and height using bilinear interpolation
    /// </summary>
    /// <param name="source"> Source texture </param>
    /// <param name="targetWidth"> Target width </param>
    /// <param name="targetHeight"> Target height </param>
    /// <returns> Edited texture </returns>
    private static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        // Create new empty Texture
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        
        // Interpolate
        Color[] pixels = result.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            Color newColor = source.GetPixelBilinear((float)(i % result.width) / (float)result.width, (float)(i / result.width) / (float)result.height);
            pixels[i] = newColor;
        }

        // Set new pixels
        result.SetPixels(pixels);
        result.Apply();
        return result;
    }

    /// <summary>
    /// Changes format of texture
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