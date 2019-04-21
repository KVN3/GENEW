using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct ShipSkin
{
    public Color32 baseColor;
    public Color32 darkColor;
    public Color32 lightColor;

    public Color forcefieldColor;
}


public class SkinManager : MonoBehaviour
{
    public ShipSkin skin;
    public bool applySkin;

    // Start is called before the first frame update
    void Start()
    {
        if (applySkin)
        {
            foreach (Transform shipKid in transform)
            {
                if (shipKid.name.Equals("Components"))
                {
                    ApplyForceFieldSkin(shipKid);
                }
                else if (shipKid.name.Equals("Mesh"))
                {
                    ApplyShipSkin(shipKid);
                }
            }
        }
    }

    private void ApplyForceFieldSkin(Transform componentsTransform)
    {
        foreach (Transform componentTransform in componentsTransform)
        {
            if (componentTransform.name.Equals("Forcefield"))
            {
                Renderer renderer = componentTransform.GetComponent<Renderer>();

                Material[] mats = renderer.materials;
                foreach (Material mat in mats)
                {
                    mat.SetColor("_TintColor", skin.forcefieldColor);
                }

            }
        }
    }

    private void ApplyShipSkin(Transform meshTransform)
    {
        foreach (Transform childTransform in meshTransform)
        {
            foreach (Transform partTransform in childTransform)
            {
                foreach (Transform singleObject in partTransform)
                {
                    Renderer renderer = singleObject.GetComponent<Renderer>();

                    Material[] mats = renderer.materials;

                    foreach (Material mat in mats)
                    {
                        switch (mat.name)
                        {
                            case "Metal_Blue (Instance)":
                                mat.color = skin.baseColor;
                                break;
                            case "Metal_Blue_Light (Instance)":
                                mat.color = skin.lightColor;
                                break;
                            case "Metal_Blue_Dark (Instance)":
                                mat.color = skin.darkColor;
                                break;
                        }
                    }
                }
            }
        }
    }

}
