using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct ShipSkin
{
    public Color baseColor;
    public Color darkColor;
    public Color lightColor;

    public Color forcefieldColor;
}


public class SkinManager : MonoBehaviour
{
    public ShipSkin skin;
    public bool applySkin;

    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        //photonView = GetComponent<PhotonView>();

        // Get the owner of this ship's skin
        //skin = PlayerManager.Instance.GetShipSkin(photonView.Owner);

        //ApplySkin();
    }

    // Apply skin on ship
    public void ApplySkin(ShipSkin skin)
    {
        this.skin = skin;

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

    // Apply skin to forcefield
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

    // Apply skin to base
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


//if (PlayerPrefs.HasKey("Ship Color"))
//{
//    string color = PlayerPrefs.GetString("Ship Color");
//    switch (color)
//    {
//        case "Red":
//            skin.baseColor = new Color32(255, 0, 0, 0);
//            skin.darkColor = new Color32(0, 0, 0, 255);
//            skin.lightColor = new Color32(30, 0, 0, 255);
//            skin.forcefieldColor = new Color32(255, 0, 0, 255);
//            break;
//        case "Green":
//            skin.baseColor = new Color32(0, 255, 0, 0);
//            skin.darkColor = new Color32(0, 0, 0, 255);
//            skin.lightColor = new Color32(0, 30, 0, 255);
//            skin.forcefieldColor = new Color32(0, 255, 0, 255);
//            break;
//        case "Blue":
//            skin.baseColor = new Color32(0, 0, 255, 0);
//            skin.darkColor = new Color32(0, 0, 0, 255);
//            skin.lightColor = new Color32(0, 0, 30, 255);
//            skin.forcefieldColor = new Color32(0, 0, 255, 255);
//            break;
//    }
//}


//case "Metal_Blue (Instance)":
//    mat.color = PlayerPrefsX.GetColor("REGULAR_COLOR");
//    break;
//case "Metal_Blue_Light (Instance)":
//    mat.color = PlayerPrefsX.GetColor("LIGHT_COLOR");
//    break;
//case "Metal_Blue_Dark (Instance)":
//    mat.color = PlayerPrefsX.GetColor("DARK_COLOR");
//    break;