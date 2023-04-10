using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;

/// <summary>
/// Class used to control Canvas in the menu scene
/// </summary>
public class ConfigController : MonoBehaviour
{
    string pathToConfig;

    [SerializeField]
    string nextScene;

    [SerializeField]
    FilterSettingVariable settings;

    [SerializeField]
    StringVariable url;

    [SerializeField]
    StringVariable clientName;

    [SerializeField]
    ConfigLanguageController langController;

    [Header("Input fields")]
    [SerializeField()]
    TMP_InputField nameFLD;
    [SerializeField()]
    TMP_InputField urlFLD;

    [Header("ErrorIcons")]
    [SerializeField()]
    GameObject errorNM;
    [SerializeField()]
    GameObject errorURL;

    // Start is called before the first frame update
    void Start()
    {
        // set language
        langController.SwapLabels();

        // read config
        pathToConfig = Directory.GetCurrentDirectory() + "\\config.txt";
        Debug.Log(pathToConfig);
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        ReadConfig();

        // display values
        DisplayValues();
    }

    /// <summary>
    /// Display config values
    /// - url of server
    /// - client name
    /// </summary>
    private void DisplayValues()
    {
        nameFLD.text = clientName.Value;
        urlFLD.text = url.Value;
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
                url.Value = lines[5].Trim();
            }
            // ClientName
            if (lines.Length >= 7)
            {
                clientName.Value = lines[6].Trim();
            }
        }
    }

    /// <summary>
    /// On Play button clicked
    /// </summary>
    public void Play()
    {
        url.Value = urlFLD.text.Trim();
        clientName.Value = nameFLD.text.Trim();

        urlFLD.text = url.Value;
        nameFLD.text = clientName.Value;

        // if no client text display error
        bool noClient = false;
        if (clientName.Value == null || clientName.Value.Length == 0)
            noClient = true;
        errorNM.SetActive(noClient);

        // if no url text display error
        bool noUrl = false;
        if (url.Value == null || url.Value.Length == 0)
            noUrl = true;
        errorURL.SetActive(noUrl);

        if (noUrl || noClient)
            return;

        SceneManager.LoadScene(nextScene);
    }

    /// <summary>
    /// Filter username
    /// - only a-zA-Z0-9_- allowed
    /// </summary>
    public void FilterName()
    {
        nameFLD.text = Regex.Replace(nameFLD.text, "[^a-zA-Z0-9_-]+", "", RegexOptions.Compiled);
    }

    /// <summary>
    /// On exit button clicked
    /// </summary>
    public void OnExit()
    {
        Application.Quit();
    }
}
