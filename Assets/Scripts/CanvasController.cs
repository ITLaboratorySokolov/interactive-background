using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using ZCU.TechnologyLab.Common.Connections.Session;
using ZCU.TechnologyLab.Common.Unity.Connections.Session;

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
    
    [Header("Connection")]
    /// <summary> Text displaying connection status </summary>
    [SerializeField]
    TMP_Text connectionTXT;

    [Header("Scripts")]
    /// <summary> Depth processing script </summary>
    [SerializeField]
    DepthProcessing depthProcessing;
    /// <summary> Session script </summary>
    [SerializeField]
    SignalRSessionWrapper session;

    /// <summary> Zoom value </summary>
    float zoom;
    /// <summary> Horizontal and vertical pan value </summary>
    float horizontalPan, verticalPan;

    /// <summary>
    /// Handle exit
    /// </summary>
    public void OnExit()
    {
        Debug.Log("Quitting!");
        Application.Quit();
    }

    /// <summary>
    /// Update - performs every frame
    /// </summary>
    private void Update()
    {
        SetConnection(session.SessionState == SessionState.Connected);
    }

    /// <summary>
    /// Display connection status
    /// </summary>
    /// <param name="connected"> Is connected to server </param>
    private void SetConnection(bool connected)
    {
        // If connected to server
        if (connected)
            ChangeConnection("Connected", Color.green);
        // If not connected to server
        else
            ChangeConnection("Not connected", Color.red);
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
            depthProcessing.min = min;
        else
            nearFLD.text = "" + depthProcessing.min;
    }

    /// <summary>
    /// Set furthest detected depth
    /// </summary>
    public void SetMax()
    {
        float max = -1;
        if (float.TryParse(farFLD.text.Trim(), out max))
        {
            depthProcessing.max = max;
        }
        else
            farFLD.text = "" + depthProcessing.max;
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
        scoreTXT.text = "Points: " + score;
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
}
