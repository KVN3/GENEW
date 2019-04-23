using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleplayerManager : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI PlayText;
    public TextMeshProUGUI BackText;
    public TextMeshProUGUI VersionText;

    // Update is called once per frame
    void Update()
    {
        TitleText.text = LocalizationManager.GetTextByKey("SINGLEPLAYER");
        PlayText.text = LocalizationManager.GetTextByKey("PLAY");
        BackText.text = LocalizationManager.GetTextByKey("RETURN");
        VersionText.text = LocalizationManager.GetTextByKey("VERSION") + ": " + "Alpha 1";
    }
}
