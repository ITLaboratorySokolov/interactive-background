using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZCU.TechnologyLab.Common.Connections.Client.Session;

/// <summary>
/// Class used for translating displayed labels
/// Aplication has a Czech mode and an English mode
/// </summary>
public class LanguageController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField()]
    TMP_Text pointsTXT;
    [SerializeField()]
    TMP_Text zoomTitleTXT;
    [SerializeField()]
    TMP_Text panTitleTXT;
    [SerializeField()]
    TMP_Text depthTitleTXT;
    [SerializeField()]
    TMP_Text nearTXT;
    [SerializeField()]
    TMP_Text farTXT;
    [SerializeField()]
    TMP_Text depthColorTitleTXT;
    [SerializeField()]
    TMP_Text colorsTXT;
    [SerializeField()]
    TMP_Text quitTXT;
    [SerializeField()]
    TMP_Text horizontalFlipTXT;

    [Header("Buttons")]
    [SerializeField()]
    Button langBT;
    [SerializeField()]
    Button yesBT;
    [SerializeField()]
    Button noBT;

    [Header("Input fields")]
    [SerializeField()]
    TMP_InputField nearFLD;
    [SerializeField()]
    TMP_InputField farFLD;
    [SerializeField()]
    TMP_InputField horizontalFLD;
    [SerializeField()]
    TMP_InputField verticalFLD;
    [SerializeField()]
    TMP_InputField zoomFLD;

    [Header("Language")]
    internal string lang;

    [Header("Strings")]
    string pointsCZ = "Body: ";
    string pointsEN = "Points: ";
    string zoomTCZ = "P�ibl�it/Odd�lit";
    string zoomTEN = "Zoom in/out";
    string panTCZ = "Posun obr�zku";
    string panTEN = "Pan image";
    string depthTCZ = "Sn�m�n� v hloubce";
    string depthTEN = "Detecting in depth";
    string nearCZ = "Od:";
    string nearEN = "Near:";
    string farCZ = "Do:";
    string farEN = "Far:";
    string colorTCZ = "Hloubka barvou";
    string colorTEN = "Depth colors";
    string colorCZ = "Zobrazit barvy";
    string colorEN = "Display colors";
    string horizontalCZ = "Zrcadlit";
    string horizontalEN = "Horizontal flip";

    string inputPromptCZ = "Vlo�te ��slo...";
    string inputPromptEN = "Enter number...";

    string quitCZ = "Ukon�it aplikaci?";
    string quitEN = "Do you want to quit?";
    string yesCZ = "Ano";
    string yesEN = "Yes";
    string noCZ = "Ne";
    string noEN = "No";

    string langCZ = "EN";
    string langEN = "CZ";

    // Start is called before the first frame update
    void Start()
    {
        lang = "CZ";
        SetLabels(0);
    }

    /// <summary>
    /// Swap languages between CZ and EN
    /// </summary>
    public void SwapLanguage(int score)
    {
        if (lang == "CZ")
            lang = "EN";
        else if (lang == "EN")
            lang = "CZ";

        SetLabels(score);
    }

    /// <summary>
    /// Set labels to czech or english texts
    /// </summary>
    private void SetLabels(int score)
    {
        if (lang == "CZ")
        {
            pointsTXT.text = pointsCZ + score;
            zoomTitleTXT.text = zoomTCZ;
            panTitleTXT.text = panTCZ;
            depthTitleTXT.text = depthTCZ;
            nearTXT.text = nearCZ;
            farTXT.text = farCZ;
            depthColorTitleTXT.text = colorTCZ;
            colorsTXT.text = colorCZ;
            quitTXT.text = quitCZ;
            horizontalFlipTXT.text = horizontalCZ;

            langBT.GetComponentInChildren<TMP_Text>().text = langCZ;

            nearFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptCZ;
            farFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptCZ;
            horizontalFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptCZ;
            verticalFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptCZ;
            zoomFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptCZ;
            yesBT.GetComponentInChildren<TMP_Text>().text = yesCZ;
            noBT.GetComponentInChildren<TMP_Text>().text = noCZ;
        }

        else if (langCZ == "EN")
        {
            pointsTXT.text = pointsEN + score;
            zoomTitleTXT.text = zoomTEN;
            panTitleTXT.text = panTEN;
            depthTitleTXT.text = depthTEN;
            nearTXT.text = nearEN;
            farTXT.text = farEN;
            depthColorTitleTXT.text = colorTEN;
            colorsTXT.text = colorEN;
            quitTXT.text = quitEN;
            horizontalFlipTXT.text = horizontalEN;

            langBT.GetComponentInChildren<TMP_Text>().text = langEN;

            nearFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;
            farFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;
            horizontalFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;
            verticalFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;
            zoomFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;
            yesBT.GetComponentInChildren<TMP_Text>().text = yesEN;
            noBT.GetComponentInChildren<TMP_Text>().text = noEN;
        }
    }

    /// <summary>
    /// Translate session state to string
    /// </summary>
    /// <param name="state"> Session state </param>
    /// <returns></returns>
    internal string GetSessionStateString(SessionState state)
    {
        if (lang == "CZ")
        {
            if (state == SessionState.Closed)
                return "Odpojeno";
            if (state == SessionState.Connected)
                return "P�ipojeno";
            if (state == SessionState.Reconnecting)
                return "P�ipojov�n�";
            if (state == SessionState.Closing)
                return "Odpojov�n�";
            if (state == SessionState.Starting)
                return "Za��n�";
        }

        return state.ToString();
    }

    /// <summary>
    /// Get "title" displayed next to the score
    /// </summary>
    /// <returns> Score title </returns>
    internal string GetPointsTitle()
    {
        if (lang == "CZ")
            return pointsCZ;
        return pointsEN;

    }
}
