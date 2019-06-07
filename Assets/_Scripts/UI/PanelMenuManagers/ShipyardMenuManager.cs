using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipyardMenuManager : MonoBehaviour
{
    public TextMeshProUGUI shipyardText;
    public TextMeshProUGUI baseColorText;
    public TextMeshProUGUI thrusterColorText;
    public TextMeshProUGUI bodyColorText;
    public TextMeshProUGUI returnText;

    // Update is called once per frame
    void Update()
    {
        shipyardText.text = LocalizationManager.GetTextByKey("SHIPYARD");
        baseColorText.text = LocalizationManager.GetTextByKey("BASE_COLOR");
        thrusterColorText.text = LocalizationManager.GetTextByKey("THRUSTER_COLOR");
        bodyColorText.text = LocalizationManager.GetTextByKey("BODY_COLOR");
        returnText.text = LocalizationManager.GetTextByKey("RETURN");
    }
}
