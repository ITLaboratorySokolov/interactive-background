using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DepthProcessing : MonoBehaviour
{
    public Texture depthTexture { get; set; }
    public RawImage foregroundImage;
    public CanvasController canvas;

    internal float min;
    internal float max;
    private Texture2D resultTexture;
    
    //public RawImage red;

    // Start is called before the first frame update
    void Start()
    {
        canvas.ChangeDepthLevels(min, max);
    }

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

    /*
    private void Control()
    {
        Texture2D rx = TextureToTexture2D(red.texture);
        Texture2D px = new Texture2D(resultTexture.width, resultTexture.height); //, TextureFormat.RGBA32, false);

        int count = 0;
        int obstructed = 0;

        // width * height
        //resultTexture.GetPixels(minX, minY, (maxX - minX), (maxY - minY));

        Color[] tC = resultTexture.GetPixels();
        Color[] rC = rx.GetPixels();
        Color[] pC = new Color[tC.Length];

        for (int i = 0; i < tC.Length; i++)
        {
            if (rC[i].a > 0.1 && tC[i].a > 0.1)
                obstructed++;

            if (rC[i].a > 0.1)
                count++;

            pC[i] = rC[i] + tC[i];
        }

        px.SetPixels(pC);

        double perc = ((double)obstructed) / count;
        // Debug.Log(obstructed + " vs " + count);
        if (perc > 0.4)
        {
            Debug.Log("NOW RED");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            var b = px.EncodeToPNG();
            File.WriteAllBytes("D:/moje/school/05/PRJ/Projects/Test2.png", b);
            Debug.Log("Saved to image");
        }
    }
    */

    private void SetForeground()
    {
        Texture2D tx = TextureToTexture2D(depthTexture);
        resultTexture = new Texture2D(tx.width, tx.height); //, TextureFormat.RGBA32, false);

        int count = 0;
        for (int w = 0; w < tx.width; w++)
        {
            for (int h = 0; h < tx.height; h++)
            {
                // TODO stolen from shader
                Color d = tx.GetPixel(w, h);
                float r = d.r; //r is unscaled depth, normalized to [0-1]
                float distMeters = r * 65536 * 0.001f;

                Color res = new Color(0, 0, 0, 0);
                if (distMeters > min && distMeters < max)
                {
                    res = new Color(1, 1, 1, 1);
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
