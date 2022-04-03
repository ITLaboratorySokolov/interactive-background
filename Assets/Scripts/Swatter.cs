using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Swatter : MonoBehaviour
{
    public RawImage fly;
    public RawImage foregroundImage;
    public FlyController flyController;

    public CanvasController canvas;
    private int hits = 0;

    public GameObject leftMenu;
    public GameObject flyObj;

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

        // TODO widht and height of screen!!
        int screenW = 1920;
        int screenH = 1080;

        int horMove = (int)foregroundImage.transform.localPosition.x;
        int vertMove = (int)foregroundImage.transform.localPosition.y;

        // how big is the scaled texture on screen
        float scale = foregroundImage.transform.localScale.x;
        int newWidth = (int)(screenW * scale);
        int newHeight = (int)(screenH * scale);

        // how much of the scaled texture is out of visible scope
        float outOfScopeW = newWidth - screenW;
        float outOfScopeH = newHeight - screenH;

        // how much it is in percentage
        float xPaddingPerc = (outOfScopeW / newWidth);
        float yPaddingPerc = (outOfScopeH / newHeight);

        // padding on each side
        int xPadding = (int) (xPaddingPerc/2 * rx.width);
        int yPadding = (int) (yPaddingPerc/2 * rx.width);

        // relevant width and height displayed on screen
        int relWidth = (int) ( (1 - xPaddingPerc) * rx.width);
        int relHeight = (int) ( (1 - yPaddingPerc) * rx.height);

        // position of fly in background texture
        int minX = xPadding + Mathf.FloorToInt((-horMove + flyMinLoc.transform.position.x) / screenW * relWidth);  
        int maxY = yPadding + Mathf.FloorToInt((-vertMove + flyMinLoc.transform.position.y) / screenH * relHeight); 

        int maxX = xPadding + Mathf.FloorToInt((-horMove + flyMaxLoc.transform.position.x) / screenW * relWidth);
        int minY = yPadding + Mathf.FloorToInt((-vertMove + flyMaxLoc.transform.position.y) / screenH * relHeight); 

        int temp = maxY;
        maxY = rx.height - minY;
        minY = rx.height - temp;

        // could be outside of texture if scale < 1
        if ((minY < 0 || maxY >= rx.height) || (minX < 0 || maxX >= rx.width))
            return;

        // get pixels under fly
        Color[] rC = rx.GetPixels(minX, minY, (maxX - minX), (maxY - minY));

        // how many of them are object
        int count = (maxX - minX) * (maxY - minY);
        int obstructed = 0;
        for (int i = 0; i < rC.Length; i++)
        {
            if (rC[i].a > 0.1)
                obstructed++;
        }

        // if more than 40% of fly covered -> hit
        double perc = ((double)obstructed) / count;
        if (perc > 0.4)
        {
            if (!flyController.oneFrame)
            {
                hits++;
                canvas.ChangeScore(hits);
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
