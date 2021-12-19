using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public TMP_Text nearTXT;
    public TMP_Text farTXT;
    public TMP_Text scoreTXT;
    public TMP_Text connectionTXT;

    public void ChangeDepthLevels(float near, float far)
    {
        nearTXT.text = "Near = " + near;
        farTXT.text = "Far = " + far;
    }

    public void ChangeScone(int score)
    {
        scoreTXT.text = "Score = " + score;
    }

    public void ChangeConnection(string msg)
    {
        connectionTXT.text = msg;
    }
}
