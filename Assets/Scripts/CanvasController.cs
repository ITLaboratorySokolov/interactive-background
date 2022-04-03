using TMPro;
using UnityEngine;

/// <summary>
/// Handle input from canvas, display new values to canvas
/// </summary>
public class CanvasController : MonoBehaviour
{
    [Header("Depth")]
    public TMP_InputField nearFLD;
    public TMP_InputField farFLD;

    [Header("Pan")]
    public TMP_InputField horizontalFLD;
    public TMP_InputField verticalFLD;

    [Header("Zoom")]
    public TMP_InputField zoomFLD;

    [Header("Game")]
    public TMP_Text scoreTXT;
    public GameObject background;
    
    [Header("Connection")]
    public TMP_Text connectionTXT;

    [Header("Scripts")]
    public DepthProcessing depthProcessing;

    float zoom;
    float horizontalPan, verticalPan;

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
    /// <param name="near"> Near </param>
    /// <param name="far"> Far </param>
    public void ChangeDepthLevels(float near, float far)
    {
        nearFLD.text = "" + near;
        farFLD.text = "" + far;
    }

    /// <summary>
    /// Change displayed pan values
    /// </summary>
    /// <param name="hor"> Horizontal pan </param>
    /// <param name="vert"> Vertical pan </param>
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
    /// Change displayed zoom value
    /// </summary>
    /// <param name="zoom"> Zoom </param>
    public void ChangeZoom(float zoom)
    {
        zoomFLD.text = "" + zoom;
        background.transform.localScale = new Vector3(Mathf.Sign(background.transform.localScale.x) * zoom, Mathf.Sign(background.transform.localScale.y) * zoom, Mathf.Sign(background.transform.localScale.z) * zoom);
    }
}
