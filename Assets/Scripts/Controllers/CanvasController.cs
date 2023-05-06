using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZCU.TechnologyLab.Common.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;

/// <summary>
/// Script handling the input from canvas
/// - handles user input and delegates it to other scripts
/// - displays updated values on canvas
/// </summary>
public class CanvasController : MonoBehaviour
{
    [Header("Depth")]
    /// <summary> Near plane (min depth) input field </summary>
    [SerializeField]
    TMP_InputField nearFLD;
    /// <summary> Far plane (max depth) input field </summary>
    [SerializeField]
    TMP_InputField farFLD;

    [Header("Pan")]
    /// <summary> Horizontal pan input field </summary>
    [SerializeField]
    TMP_InputField horizontalFLD;
    /// <summary> Vertical pan input field </summary>
    [SerializeField]
    TMP_InputField verticalFLD;

    [Header("Zoom")]
    /// <summary> Zoom input field </summary>
    [SerializeField]
    TMP_InputField zoomFLD;

    [Header("Game")]
    /// <summary> Text displaying score </summary>
    [SerializeField]
    TMP_Text scoreTXT;
    /// <summary> Depth image </summary>
    [SerializeField]
    GameObject background;
    /// <summary> Exit window </summary>
    [SerializeField]
    GameObject exitWindow;
    /// <summary> Score </summary>
    int score;

    [Header("Connection")]
    /// <summary> Text displaying connection status </summary>
    [SerializeField]
    TMP_Text connectionTXT;

    [Header("Scripts")]
    /// <summary> Depth processing script </summary>
    [SerializeField]
    DepthProcessing depthProcessing;
    /// <summary> Fly swatter script </summary>
    [SerializeField]
    Swatter flySwatter;
    /// <summary> Language controller </summary>
    [SerializeField]
    LanguageController langController;

    [Header("Background images")]
    /// <summary> Colored background image </summary>
    [SerializeField]
    RawImage colorBg;
    /// <summary> Black and white background image </summary>
    [SerializeField]
    RawImage shadowBg;


    /// <summary> Zoom value </summary>
    float zoom;
    /// <summary> Horizontal and vertical pan value </summary>
    float horizontalPan, verticalPan;
    private bool depth;

    /// <summary>
    /// Handle exit
    /// </summary>
    public void OnExit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Start
    /// </summary>
    public void Start()
    {
        int screenW = Screen.width;
        int screenH = Screen.height;

        RectTransform rtfC = colorBg.GetComponent<RectTransform>();
        rtfC.sizeDelta = new Vector2(screenW, screenH);

        RectTransform rtfS = shadowBg.GetComponent<RectTransform>();
        rtfS.sizeDelta = new Vector2(screenW, screenH);

        zoom = 1;

        ChangeScore(0);
    }

    /// <summary>
    /// Display connection status
    /// </summary>
    /// <param name="connected"> Is connected to server </param>
    public void SetConnection(SignalRSessionWrapper session) // bool connected)
    {
        Debug.Log(session.State.ToString());

        string state = langController.GetSessionStateString(session.State);

        if (session.State == SessionState.Connected)
            ChangeConnection(state, Color.green);
        else if (session.State == SessionState.Reconnecting)
            ChangeConnection(state, Color.yellow);
        else
            ChangeConnection(state, Color.red);
    }

    /// <summary>
    /// Change displayed connection status
    /// </summary>
    /// <param name="msg"> Message </param>
    /// <param name="c"> Colour of text </param>
    public void ChangeConnection(string msg, Color c)
    {
        connectionTXT.text = msg;
        connectionTXT.color = c;
    }

    /// <summary>
    /// Set zoom of depth image
    /// </summary>
    public void SetZoom()
    {
        float zoom = -1;
        if (float.TryParse(zoomFLD.text.Trim(), out zoom))
        {
            if (zoom < 0)
            {
                zoomFLD.text = $"{this.zoom}";
                return;
            }

            background.transform.localScale = new Vector3(Mathf.Sign(background.transform.localScale.x) * zoom, Mathf.Sign(background.transform.localScale.y) * zoom, Mathf.Sign(background.transform.localScale.z) * zoom);
            this.zoom = zoom;
        }
        else
            zoomFLD.text = $"{this.zoom}";
    }

    /// <summary>
    /// Set nearest detencted depth
    /// </summary>
    public void SetMin()
    {
        float min = -1;
        if (float.TryParse(nearFLD.text.Trim(), out min))
        {
            if (min >= 0)
                depthProcessing.Min = min;
            else
                nearFLD.text = "" + depthProcessing.Min;
        }
        else
            nearFLD.text = "" + depthProcessing.Min;
    }

    /// <summary>
    /// Set furthest detected depth
    /// </summary>
    public void SetMax()
    {
        float max = -1;
        if (float.TryParse(farFLD.text.Trim(), out max))
        {
            if (max >= 0)
                depthProcessing.Max = max;
            else
               farFLD.text = "" + depthProcessing.Max;
        }
        else
            farFLD.text = "" + depthProcessing.Max;
    }

    /// <summary>
    /// Set horizontal pan of depth image
    /// </summary>
    public void SetHorizontalPan()
    {
        int hor = -1;
        if (int.TryParse(horizontalFLD.text.Trim(), out hor))
        {
            background.transform.localPosition = new Vector3(hor, background.transform.localPosition.y, 0);
            horizontalPan = hor;
        }
        else
            horizontalFLD.text = $"{this.horizontalPan}";
    }

    /// <summary>
    /// Set vertical pan of depth image
    /// </summary>
    public void SetVerticalPan()
    {
        int ver = -1;
        if (int.TryParse(verticalFLD.text.Trim(), out ver))
        {
            background.transform.localPosition = new Vector3(background.transform.localPosition.x, ver, 0);
            verticalPan = ver;
        }
        else
            verticalFLD.text = $"{this.verticalPan}";
    }

    /// <summary>
    /// Change displayed depth levels
    /// </summary>
    /// <param name="near"> Near value </param>
    /// <param name="far"> Far value </param>
    public void ChangeDepthLevels(float near, float far)
    {
        nearFLD.text = "" + near;
        farFLD.text = "" + far;
    }

    /// <summary>
    /// Change displayed pan values
    /// </summary>
    /// <param name="hor"> Horizontal pan value </param>
    /// <param name="vert"> Vertical pan value </param>
    public void ChangePanLevels(int hor, int vert)
    {
        horizontalFLD.text = "" + hor;
        verticalFLD.text = "" + vert;

        background.transform.localPosition = new Vector3(hor, vert, 0);
    }

    /// <summary>
    /// Change displayed score
    /// </summary>
    /// <param name="score"> Score </param>
    public void ChangeScore(int score)
    {
        this.score = score;
        scoreTXT.text = langController.GetPointsTitle() + score;
    }

    /// <summary>
    /// Change displayed zoom value
    /// </summary>
    /// <param name="zoom"> Zoom value </param>
    public void ChangeZoom(float zoom)
    {
        zoomFLD.text = "" + zoom;
        background.transform.localScale = new Vector3(Mathf.Sign(background.transform.localScale.x) * zoom, Mathf.Sign(background.transform.localScale.y) * zoom, Mathf.Sign(background.transform.localScale.z) * zoom);
    }

    /// <summary>
    /// Toggle colorOn
    /// </summary>
    public void ToggleColor()
    {
        depthProcessing.colorOn = !depthProcessing.colorOn;

        // Switch active background images
        if (depthProcessing.colorOn)
        {
            shadowBg.gameObject.SetActive(false);
            colorBg.gameObject.SetActive(true);

            // Set shader variables
            colorBg.material.SetFloat("_MinRange", depthProcessing.Min);
            colorBg.material.SetFloat("_MaxRange", depthProcessing.Max);
        }
        else
        {
            shadowBg.gameObject.SetActive(true);
            colorBg.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Flip background image horizontally
    /// </summary>
    public void HorizontalFlip()
    {
        Vector3 shadowScale = shadowBg.gameObject.transform.localScale;
        shadowBg.gameObject.transform.localScale = new Vector3(-shadowScale.x, shadowScale.y, shadowScale.z);

        Vector3 colorScale = colorBg.gameObject.transform.localScale;
        colorBg.gameObject.transform.localScale = new Vector3(-colorScale.x, colorScale.y, colorScale.z);

        flySwatter.invertedX = !flySwatter.invertedX;
    }

    /// <summary>
    /// Switch languages
    /// </summary>
    public void SwitchLanguages()
    {
        langController.SwapLanguage(score);
    }

    /// <summary>
    /// Toggle exit window
    /// </summary>
    /// <param name="val"> Turn exit window on if true, off if false </param>
    public void ToggleExitWindow(bool val)
    {
        exitWindow.SetActive(val);
    }

}
