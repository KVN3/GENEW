using UnityEngine;

public class ColorPickerTester : MonoBehaviour
{

    public new Renderer renderer;
    public ColorPicker picker;

    public Color Color = Color.red;

    void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();

        Material[] mats = renderer.materials;

        foreach (Material mat in mats)
        {
            switch (mat.name)
            {
                case "Metal_Blue (Instance)":
                    picker.CurrentColor = PlayerPrefsX.GetColor("REGULAR_COLOR");
                    break;
                case "Metal_Blue_Light (Instance)":
                    picker.CurrentColor = PlayerPrefsX.GetColor("LIGHT_COLOR");
                    break;
                case "Metal_Blue_Dark (Instance)":
                    picker.CurrentColor = PlayerPrefsX.GetColor("DARK_COLOR");
                    break;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        picker.onValueChanged.AddListener(color =>
        {
            renderer.material.color = color;
            Color = PlayerPrefsX.GetColor("LIGHT_COLOR");
        });

        renderer.material.color = PlayerPrefsX.GetColor("LIGHT_COLOR");


    }

    // Update is called once per frame
    void Update()
    {

    }
}
