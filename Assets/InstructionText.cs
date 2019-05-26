using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionText : MonoBehaviour
{
    public static InstructionText Instance;

    public Text textComponent;

    void Awake()
    {
        Instance = this;

        textComponent = GetComponent<Text>();
    }

    public void SetText(string textString)
    {
        textComponent.text = textString;
    }
}
