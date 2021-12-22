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

    [Header("Server")]
    public ServerConnection conn;

    [Header("Game")]
    public DepthProcessing imgProc;

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
            if (lines.Length >= 2)
            {
                float locMin = float.NaN;
                if (float.TryParse(lines[0].Trim(), out locMin))
                    minDepth = locMin;
                float locMax = float.NaN;
                if (float.TryParse(lines[1].Trim(), out locMax))
                    maxDepth = locMax;
            }
            if (lines.Length >= 3)
            {
                conn.url = lines[2].Trim();
            }
        }

        imgProc.min = minDepth;
        imgProc.max = maxDepth;
    }


}
