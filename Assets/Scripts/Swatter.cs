using UnityEngine;
using UnityEngine.UI;

public class Swatter : MonoBehaviour
{
    public RawImage fly;
    public RawImage foregroundImage;
    public FlyController flyController;

    public CanvasController canvas;
    private int hits = 0;

    private Transform flyMinLoc;
    private Transform flyMaxLoc;

    private void Start()
    {
        flyMinLoc = fly.transform.Find("min");
        flyMaxLoc = fly.transform.Find("max");
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

    public void TestHit()
    {
        Texture2D rx = TextureToTexture2D(foregroundImage.texture);
        Texture2D fx = TextureToTexture2D(fly.texture);

        // pozice ve scéně - střed
        // -> jaké pixely v celém obrázku zasahuje

        // TODO widht and height of screen!!
        int minX = Mathf.FloorToInt(flyMinLoc.transform.position.x / 1920 * rx.width);  
        int maxY = Mathf.FloorToInt(flyMinLoc.transform.position.y / 1080 * rx.height); 

        int maxX = Mathf.FloorToInt(flyMaxLoc.transform.position.x / 1920 * rx.width); 
        int minY = Mathf.FloorToInt(flyMaxLoc.transform.position.y / 1080 * rx.height); 

        int temp = maxY;
        maxY = rx.height - minY;
        minY = rx.height - temp;

        // width * height
        Color[] rC = rx.GetPixels(minX, minY, (maxX - minX), (maxY - minY));

        int count = (maxX - minX) * (maxY - minY);
        int obstructed = 0;
        for (int i = 0; i < rC.Length; i++)
        {
            if (rC[i].a > 0.1)
                obstructed++;
        }

        double perc = ((double)obstructed) / count;
        // Debug.Log(obstructed + " vs " + count);
        if (perc > 0.4)
        {
            // flyController.hit = true;
            if (!flyController.oneFrame)
            {
                hits++;
                canvas.ChangeScone(hits);
            }
            flyController.MoveToNewLocation();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (foregroundImage.texture == null)
            return;
        // get fly

        // test if it hit with currently rendered texture

        if (fly.enabled)
        {
            TestHit();
        }

        if (flyController.moved)
        {
            flyController.moved = false;
        } else
        {
            flyController.oneFrame = false;
        }

    }
}
