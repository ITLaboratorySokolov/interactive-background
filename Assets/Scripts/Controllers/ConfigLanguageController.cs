using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class used for translating displayed labels in title scene
/// Aplication has a Czech mode and an English mode
/// </summary>
public class ConfigLanguageController : MonoBehaviour
{
    [Header("String")]
    string urlCZ = "URL serveru:";
    string urlEN = "Server URL:";

    string nameCZ = "Jméno klienta:";
    string nameEN = "Client name:";

    string setCZ = "Hrát";
    string setEN = "Play";

    string inputPromptCZ = "Napište text...";
    string inputPromptEN = "Enter text...";

    string noNameMSG = "No client name provided";
    public string NoNameMSG { get => noNameMSG; set => noNameMSG = value; }

    string langCZ = "EN";
    string langEN = "CZ";

    string lang = "EN";

    [Header("Text")]
    [SerializeField()]
    TMP_Text nameTXT;
    [SerializeField()]
    TMP_Text urlTXT;

    [Header("Buttons")]
    [SerializeField()]
    Button langBT;
    [SerializeField()]
    Button playBT;

    [Header("Input fields")]
    [SerializeField()]
    TMP_InputField nameFLD;
    [SerializeField()]
    TMP_InputField urlFLD;

    /// <summary>
    /// Swap language
    /// </summary>
    public void SwapLanguage()
    {
        if (lang == "EN")
            lang = "CZ";
        else
            lang = "EN";

        SwapLabels();
    }

    /// <summary>
    /// Swap displayed labels
    /// </summary>
    public void SwapLabels()
    {
        if (lang == "EN")
        {
            nameTXT.text = nameEN;
            urlTXT.text = urlEN;

            langBT.GetComponentInChildren<TMP_Text>().text = langEN;
            playBT.GetComponentInChildren<TMP_Text>().text = setEN;

            nameFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;
            urlFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;

            NoNameMSG = "No client name provided";

        }
        else if (lang == "CZ")
        {
            nameTXT.text = nameCZ;
            urlTXT.text = urlCZ;

            langBT.GetComponentInChildren<TMP_Text>().text = langCZ;
            playBT.GetComponentInChildren<TMP_Text>().text = setCZ;

            nameFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptCZ;
            urlFLD.placeholder.GetComponent<TMP_Text>().text = inputPromptCZ;

            NoNameMSG = "Prázdné uživatelské jméno";
        }
    }
}
