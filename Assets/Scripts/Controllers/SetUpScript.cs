using System;
using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;

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

    [Header("Server connection")]
    /// <summary> Server url </summary>
    [SerializeField]
    private StringVariable serverUrl;
    /// <summary> Client name </summary>
    [SerializeField]
    private StringVariable clientName;

    [Header("Filter settings")]
    /// <summary> Filter settings </summary>
    [SerializeField]
    private FilterSettingVariable settings;

    [Header("Game scripts")]
    /// <summary> Depth processing script </summary>
    [SerializeField]
    DepthProcessing depthProc;
    /// <summary> Canvas manager script </summary>
    [SerializeField]
    CanvasController canvas;
    /// <summary> Server connection </summary>
    [SerializeField]
    ServerConnection serverConnection;


    /// <summary>
    /// Set up configuration before application starts
    /// </summary>
    private void Awake()
    {
        pathToConfig = "./config.txt"; // Directory.GetCurrentDirectory() + "\\config.txt";
        Debug.Log(pathToConfig);

        // Set culture -> doubles are written with decimal dot
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        SetValues();
    }

    /// <summary>
    /// Reads from config min and max recorded depth, horizontal and vertical pan, zoom and server url
    /// </summary>
    private void ReadConfig()
    {
        if (File.Exists(pathToConfig))
        {
            Debug.Log("Loading config file...");
            string[] lines = File.ReadAllLines(pathToConfig);
            if (lines.Length >= 5)
            {
                // Min depth
                float locMin = float.NaN;
                if (float.TryParse(lines[0].Trim(), out locMin))
                    settings.MinDepth = locMin;
                // Max depth
                float locMax = float.NaN;
                if (float.TryParse(lines[1].Trim(), out locMax))
                    settings.MaxDepth = locMax;
                // Horizontal pan
                int panH = 0;
                if (int.TryParse(lines[2].Trim(), out panH))
                    settings.PanHor = panH;
                // Vertical pan
                int panV = 0;
                if (int.TryParse(lines[3].Trim(), out panV))
                    settings.PanVert = panV;
                // Zoom
                float z = float.NaN;
                if (float.TryParse(lines[4].Trim(), out z))
                    settings.Zoom = z;
            }
            // Url
            if (lines.Length >= 6)
            {
                serverUrl.Value = lines[5].Trim();
            }
            // ClientName
            if (lines.Length >= 7)
            {
                clientName.Value = lines[6].Trim();
            }
        }

        SetValues();
    }

    /// <summary>
    /// Set values in depth processor, and canvas
    /// </summary>
    private void SetValues()
    {
        depthProc.min = settings.MinDepth;
        depthProc.max = settings.MaxDepth;
        canvas.ChangeDepthLevels(settings.MinDepth, settings.MaxDepth);
        canvas.ChangePanLevels(settings.PanHor, settings.PanVert);
        canvas.ChangeZoom(settings.Zoom);
    }

    public void ResetSetUp()
    {
        Debug.Log("Reseting configuration");
        ReadConfig();
        serverConnection.ResetConnection();
    }

}
