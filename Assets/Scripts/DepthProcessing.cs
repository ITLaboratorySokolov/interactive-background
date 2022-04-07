using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DepthProcessing : MonoBehaviour
{
    public Texture depthTexture { get; set; }
    public RawImage foregroundImage;

    internal float min;
    internal float max;

    private Texture2D resultTexture;

    private Texture2D TextureToTexture2D(Texture texture)
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

    // Update is called once per frame
    void Update()
    {
        if (depthTexture == null)
            return;

        SetForeground();
    }

    private void SetForeground()
    {
        Texture2D tx = TextureToTexture2D(depthTexture);
        resultTexture = new Texture2D(tx.width, tx.height); //, TextureFormat.RGBA32, false);

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
        foregroundImage.texture = resultTexture;

        if (Input.GetKeyDown(KeyCode.S))
        {
            var b = resultTexture.EncodeToPNG();
            File.WriteAllBytes("D:/moje/school/05/PRJ/Projects/Test.png", b);
            Debug.Log("Saved to image");
        }
    }
}
