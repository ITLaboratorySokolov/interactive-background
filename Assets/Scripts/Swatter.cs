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
    /// <summary> Background image - processed depth </summary>
    [SerializeField]
    RawImage bgImage;

    [Header("Scripts")]
    /// <summary> Fly controller </summary>
    [SerializeField]
    FlyController flyController;
    /// <summary> Canvas controller </summary>
    [SerializeField]
    CanvasController canvas;
    
    /// <summary> Left top corner of fly </summary>
    private Transform flyMinLoc;
    /// <summary> Right bottom corner of fly </summary>
    private Transform flyMaxLoc;
    /// <summary> Number of hits </summary>
    private int hits = 0;

    /// <summary>
    /// Performes once upon start
    /// </summary>
    private void Start()
    {
        flyMinLoc = fly.transform.Find("min");
        flyMaxLoc = fly.transform.Find("max");
    }

    /// <summary>
    /// Tests if the fly was hit
    /// </summary>
    public void TestHit()
    {
        int screenW = Screen.width;
        int screenH = Screen.height;

        int horMove = (int)bgImage.transform.localPosition.x;
        int vertMove = (int)bgImage.transform.localPosition.y;

        // Get textures
        Texture2D rx = ImageProcessor.TextureToTexture2D(bgImage.texture);
        Texture2D fx = ImageProcessor.TextureToTexture2D(fly.texture);

        // How big is the scaled texture on screen
        float scale = bgImage.transform.localScale.x;
        int newWidth = (int)(screenW * scale);
        int newHeight = (int)(screenH * scale);

        // How much of the scaled texture is out of visible scope
        float outOfScopeW = newWidth - screenW;
        float outOfScopeH = newHeight - screenH;

        // How much it is in percentage
        float xPaddingPerc = (outOfScopeW / newWidth);
        float yPaddingPerc = (outOfScopeH / newHeight);

        // Padding on each side
        int xPadding = (int) (xPaddingPerc/2 * rx.width);
        int yPadding = (int) (yPaddingPerc/2 * rx.height);

        // Relevant width and height displayed on screen
        int relWidth = (int) ( (1 - xPaddingPerc) * rx.width);
        int relHeight = (int) ( (1 - yPaddingPerc) * rx.height);

        // Position of fly in background texture
        int minX = xPadding + Mathf.FloorToInt((-horMove + flyMinLoc.transform.position.x) / screenW * relWidth);  
        int maxY = yPadding + Mathf.FloorToInt((-vertMove + flyMinLoc.transform.position.y) / screenH * relHeight); 

        int maxX = xPadding + Mathf.FloorToInt((-horMove + flyMaxLoc.transform.position.x) / screenW * relWidth);
        int minY = yPadding + Mathf.FloorToInt((-vertMove + flyMaxLoc.transform.position.y) / screenH * relHeight); 

        int temp = maxY;
        maxY = rx.height - minY;
        minY = rx.height - temp;

        // Could be outside of texture if scale < 1
        if ((minY < 0 || maxY >= rx.height) || (minX < 0 || maxX >= rx.width))
            return;

        // Get pixels under fly
        Color[] rC = rx.GetPixels(minX, minY, (maxX - minX), (maxY - minY));

        // How many of them are object
        int count = (maxX - minX) * (maxY - minY);
        int obstructed = 0;
        for (int i = 0; i < rC.Length; i++)
        {
            if (rC[i].a > 0.1)
                obstructed++;
        }

        // If more than 40% of fly covered -> hit
        double perc = ((double)obstructed) / count;
        if (perc > 0.4)
        {
            // Fly was hit more than one frame ago = it found an empty place to sit on
            if (!flyController.oneFrame)
            {
                hits++;
                canvas.ChangeScore(hits);
            }
            flyController.MoveToNewLocation();
        }
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
            TestHit();

        // Fly has moved
        if (flyController.moved)
            flyController.moved = false;
        // Fly was still for at least one frame
        else
            flyController.oneFrame = false;
    }
}
