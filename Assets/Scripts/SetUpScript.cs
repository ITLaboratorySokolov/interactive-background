using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.AssetVariables;

// TODO path to config file

/// <summary>
/// Script managing the set up of the application
/// - reads config file
/// </summary>
public class SetUpScript : MonoBehaviour
{
    [Header("Config")]
    /// <summary> Path to config file </summary>
    string pathToConfig;
    /// <summary> Minimum depth </summary>
    float minDepth = 0;
    /// <summary> Maximum depth </summary>
    float maxDepth = 0.69f;
    /// <summary> Horizontal pan </summary>
    int panHor = 0;
    /// <summary> Vertical pan </summary>
    int panVert = 0;
    /// <summary> Zoom </summary>
    float zoom = 1;

    [Header("Server connection")]
    /// <summary> Server url </summary>
    [SerializeField]
    private StringVariable serverUrl;

    [Header("Game scripts")]
    /// <summary> Depth processing script </summary>
    [SerializeField]
    DepthProcessing depthProc;
    /// <summary> Canvas manager script </summary>
    [SerializeField]
    CanvasController canvas;

    /// <summary>
    /// Set up configuration before application starts
    /// - read from config min and max recorded depth, horizontal and vertical pan, zoom and server url
    /// </summary>
    private void Awake()
    {
        pathToConfig = "./config.txt"; // Directory.GetCurrentDirectory() + "\\config.txt";
        Debug.Log(pathToConfig);

        // Set culture -> doubles are written with decimal dot
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        if (File.Exists(pathToConfig))
        {
            Debug.Log("Loading config file...");
            string[] lines = File.ReadAllLines(pathToConfig);
            if (lines.Length >= 5)
            {
                // Min depth
                float locMin = float.NaN;
                if (float.TryParse(lines[0].Trim(), out locMin))
                    minDepth = locMin;
                // Max depth
                float locMax = float.NaN;
                if (float.TryParse(lines[1].Trim(), out locMax))
                    maxDepth = locMax;
                // Horizontal pan
                int panH = 0;
                if (int.TryParse(lines[2].Trim(), out panH))
                    panHor = panH;
                // Vertical pan
                int panV = 0;
                if (int.TryParse(lines[3].Trim(), out panV))
                    panVert = panV;
                // Zoom
                float z = float.NaN;
                if (float.TryParse(lines[4].Trim(), out z))
                    zoom = z;
            }
            // Url
            if (lines.Length >= 6)
            {
                serverUrl.Value = lines[5].Trim();
            }
        }

        // Set values in depth processor, and canvas
        depthProc.min = minDepth;
        depthProc.max = maxDepth;
        canvas.ChangeDepthLevels(minDepth, maxDepth);
        canvas.ChangePanLevels(panHor, panVert);
        canvas.ChangeZoom(zoom);
    }


}
