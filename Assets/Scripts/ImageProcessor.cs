using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImageProcessor
{
    // TODO could be optimized into one cycle through all pixels
    public static Texture2D ScaleTextureImage(Texture2D source, float scale)
    {
        if (Mathf.Abs(scale - 1) < 0.001)
            return source;

        int targetWidth = (int)(source.width * scale);
        int targetHeight = (int)(source.height * scale);

        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        Color[] pixels = result.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            Color newColor = source.GetPixelBilinear((float)(i % result.width) / (float)result.width, (float)(i / result.width) / (float)result.height);
            pixels[i] = newColor;
        }
        result.SetPixels(pixels);
        result.Apply();

        // Graphics.Blit(result, )

        Color[] pixelsSrc = new Color[source.width * source.height];
        int startX = (source.width / 2 - targetWidth / 2);
        int endX = (source.width / 2 + targetWidth / 2);
        int startY = (source.height / 2 - targetHeight / 2);

        int index = 0, add = 0, addEnd = 0;

        // add empty rows
        if (startY < 0)
        {
            index += (-startY) * targetWidth;
            startY = 1;
        }
        // empty columns start
        if (startX < 0)
        {
            add = -startX;
            startX = 0;
        }
        // empty columns end
        if (endX > source.width)
        {
            addEnd = endX - source.width;
            endX = source.width;
        }

        for (int h = startY; h < source.height - 1; h++) {
            index += add;
            for (int w = startX; w < endX; w++)
            {
                if (w >= 0 && h * source.width + w < pixelsSrc.Length && h * source.width + w >= 0)
                {
                    pixelsSrc[h * source.width + w] = pixels[index];
                }
                index++;
            }
            index += addEnd;
        }
        source.SetPixels(pixelsSrc);
        source.Apply();

        return source;
    }
}
