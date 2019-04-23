using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleplayerManager : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI PlayText;
    public TextMeshProUGUI RedText;
    public TextMeshProUGUI BlueText;
    public TextMeshProUGUI GreenText;
    public TextMeshProUGUI VersionText;

    // Update is called once per frame
    void Update()
    {
        TitleText.text = LocalizationManager.GetTextByKey("SINGLEPLAYER");
        PlayText.text = LocalizationManager.GetTextByKey("PLAY");
        //RedText.text = LocalizationManager.GetTextByKey("RED");
        //BlueText.text = LocalizationManager.GetTextByKey("BLUE");
        //GreenText.text = LocalizationManager.GetTextByKey("GREEN");
        VersionText.text = LocalizationManager.GetTextByKey("VERSION") + ": " + "Alpha 1";
    }
}
