using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script managing the interaction of depth background and the foreground object
/// - allows the user to kill the fly
/// </summary>
public class Swatter : MonoBehaviour
{
    [Header("Game objects")]
    /// <summary> Foreground image - fly </summary>
    [SerializeField]
    RawImage fly;
    /// <summary> Parent object of possible backgrounds </summary>
    [SerializeField]
    GameObject backgroundGroup;
    /// <summary> Background image - processed depth </summary>
    [SerializeField]
    internal RawImage bgImage;
    /// <summary> Debug image </summary>
    [SerializeField]
    internal RawImage ted;

    [Header("Scripts")]
    /// <summary> Fly controller </summary>
    [SerializeField]
    FlyController flyController;
    /// <summary> Canvas controller </summary>
    [SerializeField]
    CanvasController canvas;
    [SerializeField]
    DepthProcessing depthProcessing;

    [Header("Sound")]
    /// <summary> Fly buzz sound - https://www.youtube.com/watch?v=AtbWRz4gdzU </summary>
    [SerializeField]
    AudioSource swatSound;
    /// <summary> Swatting sound - https://youtu.be/_BQbK3vm0AA </summary>
    [SerializeField]
    AudioSource flySound;

    [Header("Fly information")]
    /// <summary> Left top corner of fly </summary>
    private Transform flyMinLoc;
    /// <summary> Right bottom corner of fly </summary>
    private Transform flyMaxLoc;
    /// <summary> Number of hits </summary>
    private int hits = 0;

    [Header("Helper variables")]
    /// <summary> Texture2D used for manipulation </summary>
    Texture2D rx;

    /// <summary>
    /// Performes once upon start
    /// </summary>
    private void Start()
    {
        flyMinLoc = fly.transform.Find("min");
        flyMaxLoc = fly.transform.Find("max");

    }

    /// <summary>
    /// Debug method - detecting the position of fly in texture
    /// </summary>
    private void TestDetection()
    {
        float scale = backgroundGroup.transform.localScale.x;

        // creates "padding" around the borders that needs to be added -> work against the "movement" of fly

        // 1920 x 1080 scaled to
        int newWidht = (int)(1920 * scale);
        int newHeight = (int)(1080 * scale);

        // how much of image is out of scope
        int padX = ((newWidht - 1920) / 2);
        int padY = ((newHeight - 1080) / 2);
        
        // this poisition into percentage according to new size of img
        Vector2 flyPos = (flyMinLoc.position + (flyMaxLoc.position - flyMinLoc.position) / 2) + new Vector3(padX, padY);
        float percX = flyPos.x / newWidht;
        float percY = flyPos.y / newHeight;

        // -> now this percentage needs to be scaled back onto the texture
        Vector2 flyPosInTXT = new Vector2(percX * 1920, percY * 1080);
        
        // -> pan also needs to be affected by scale
        Vector2 pan = backgroundGroup.transform.localPosition * 1/scale;

        Vector2 posInTex = - pan + flyPosInTXT;

        Texture2D nT = new Texture2D(1920, 1080);

        // -> size also affected by scale
        int widht = (int)(Mathf.Abs((flyMaxLoc.position - flyMinLoc.position).x) * 1/scale);
        int height = (int)(Mathf.Abs((flyMaxLoc.position - flyMinLoc.position).y) * 1/scale);

        int startX = (int)posInTex.x - widht / 2;
        int startY = (int)(nT.height - posInTex.y) - height / 2;

        Color[] nc = new Color[widht * height];
        for (int i = 0; i < nc.Length; i++)
            nc[i] = new Color(0.5f, 0, 0);

        if (startX >= 0 && startY >= 0 && startX + widht < nT.width && startY + height < nT.height) {
            nT.SetPixels(startX, startY, widht, height, nc);
            nT.Apply();
        }

        if (ted.texture != null)
            Destroy(ted.texture);
        ted.texture = nT;
    }

    /// <summary>
    /// Tests if the fly was hit
    /// - detects the space occupied by the fly in the image
    /// - if enough pixels covered, then the fly was hit
    /// </summary>
    public void TestHit()
    {
        // Get textures
        rx = ImageProcessor.TextureToTexture2D(bgImage.texture);

        int screenW = 1920; // Screen.width;
        int screenH = 1080; // Screen.height;

        // SCALE
        float scale = backgroundGroup.transform.localScale.x;

        // 1920 x 1080 scaled to
        Vector2 newSize = new Vector2(screenW * scale, screenH * scale);

        // how much of image is out of scope
        Vector2 padSize = new Vector2((newSize.x - screenW) / 2, (newSize.y - screenH) / 2);

        // this poisition into percentage according to new size of img
        Vector2 flyPos = (Vector2)((flyMinLoc.position + (flyMaxLoc.position - flyMinLoc.position) / 2)) + padSize;
        Vector2 perc = new Vector2(flyPos.x / newSize.x, flyPos.y / newSize.y);

        // PAN
        // -> pan also needs to be affected by scale
        Vector2 pan = (backgroundGroup.transform.localPosition * 1 / scale);
        pan = new Vector2((pan.x / screenW) * rx.width, (pan.y / screenH) * rx.height);

        // POSITION
        // -> now this percentage needs to be scaled back onto the texture
        Vector2 flyPosInTXT = new Vector2(perc.x * rx.width, perc.y * rx.height);
        Vector2 posInTex = -pan + flyPosInTXT;

        // -> size also affected by scale
        int width = (int)(((Mathf.Abs((flyMaxLoc.position - flyMinLoc.position).x) * 1/scale) / screenW) * rx.width);
        int height = (int)(((Mathf.Abs((flyMaxLoc.position - flyMinLoc.position).y) * 1/scale) / screenH) * rx.height);

        int startX = (int)posInTex.x - width / 2;
        int startY = (int)(rx.height - posInTex.y) - height / 2;

        startX = Mathf.Clamp(startX, 0, rx.width);
        startY = Mathf.Clamp(startY, 0, rx.height);
        width = Mathf.Clamp(width, 0, rx.width - startX);
        height = Mathf.Clamp(height, 0, rx.height - startY);

        if (startX >= 0 && startY >= 0 && startX < rx.width && startY < rx.height) // && startX + widht < rx.width && startY + height < rx.height)
        {
            var underFly = rx.GetPixels(startX, startY, width, height);

            // How many of them are object
            int count = underFly.Length;
            int obstructed = TestRelevantCutout(underFly);

            // If more than 40% of fly covered -> hit
            double pVal = ((double)obstructed) / count;
            if (pVal > 0.4)
                SwatFly();

            Debug.Log(pVal);
        }
    }

    /// <summary>
    /// Swat the fly
    /// - play sounds
    /// - add points
    /// - move fly
    /// </summary>
    private void SwatFly()
    {
        // Fly was hit more than one frame ago = it found an empty place to sit on
        if (!flyController.oneFrame)
        {
            // Play sound
            swatSound.Play();
            flySound.Play();

            // Add points
            hits++;
            canvas.ChangeScore(hits);
        }
        // Move fly
        flyController.MoveToNewLocation();
    }

    /// <summary>
    /// Count number of ocluded valid depth values
    /// </summary>
    /// <param name="rC"> Texture cutout to test </param>
    /// <returns> Number of ocluded valid depth values </returns>
    private int TestRelevantCutout(Color[] rC)
    {
        int obstructed = 0;

        for (int i = 0; i < rC.Length; i++)
        {
            if (rC[i].a > 0.1)
                obstructed++;
        }

        return obstructed;
    }

    /// <summary>
    /// Updates once per frame
    /// </summary>
    void Update()
    {
        // No background image texture
        if (bgImage.texture == null)
            return;

        // Test if fly is hit with currently rendered texture
        if (fly.enabled)
        {
            TestHit();
        }

        // Fly has moved
        if (flyController.moved)
            flyController.moved = false;
        // Fly was still for at least one frame
        else
            flyController.oneFrame = false;
    }

    
}
