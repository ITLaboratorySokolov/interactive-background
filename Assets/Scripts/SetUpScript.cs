using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;

public class SetUpScript : MonoBehaviour
{
    [Header("Config")]
    public string pathToConfig = "D:/moje/school/05/PRJ/Projects/Realsense projection/Assets/config.txt";
    float minDepth = 0;
    float maxDepth = 0.69f;
    int panHor = 0;
    int panVert = 0;
    float zoom = 1;

    [Header("Server")]
    public ServerConnection conn;

    [Header("Game")]
    public DepthProcessing imgProc;
    public CanvasController canvas;

    /// <summary>
    /// Set up configuration before application starts
    /// - read from config min and max recorded depth and port number
    /// </summary>
    private void Awake()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        if (File.Exists(pathToConfig))
        {
            string[] lines = File.ReadAllLines(pathToConfig);
            if (lines.Length >= 5)
            {
                float locMin = float.NaN;
                if (float.TryParse(lines[0].Trim(), out locMin))
                    minDepth = locMin;
                float locMax = float.NaN;
                if (float.TryParse(lines[1].Trim(), out locMax))
                    maxDepth = locMax;
                int panH = 0;
                if (int.TryParse(lines[2].Trim(), out panH))
                    panHor = panH;
                int panV = 0;
                if (int.TryParse(lines[3].Trim(), out panV))
                    panVert = panV;
                float z = float.NaN;
                if (float.TryParse(lines[4].Trim(), out z))
                    zoom = z;
            }
            if (lines.Length >= 6)
            {
                conn.url = lines[5].Trim();
            }
        }

        imgProc.min = minDepth;
        imgProc.max = maxDepth;
        canvas.ChangeDepthLevels(minDepth, maxDepth);
        canvas.ChangePanLevels(panHor, panVert);
        canvas.ChangeZoom(zoom);
    }


}
