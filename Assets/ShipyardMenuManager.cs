using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipyardMenuManager : MonoBehaviour
{
    public TextMeshProUGUI ShipyardText;
    public TextMeshProUGUI ReturnText;

    // Update is called once per frame
    void Update()
    {
        ReturnText.text = LocalizationManager.GetTextByKey("RETURN");
    }
}
