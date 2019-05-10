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


public class SkinManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public ShipSkin skin;
    public bool applySkin;

    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            skin.baseColor = PlayerPrefsX.GetColor("REGULAR_COLOR");
            skin.lightColor = PlayerPrefsX.GetColor("LIGHT_COLOR");
            skin.darkColor = PlayerPrefsX.GetColor("DARK_COLOR");
        }

    }

    [PunRPC]
    private void RPC_ApplySkin()
    {
        ApplySkin();
    }

    private void Update()
    {
        photonView.RPC("RPC_ApplySkin", RpcTarget.AllBuffered);
    }

    // Send data if this is our ship, receive data if it is not
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            float[] baseColorArray = new float[4];
            baseColorArray[0] = skin.baseColor.r;
            baseColorArray[1] = skin.baseColor.g;
            baseColorArray[2] = skin.baseColor.b;
            baseColorArray[3] = skin.baseColor.a;
            stream.SendNext(baseColorArray);

            float[] lightColorArray = new float[4];
            lightColorArray[0] = skin.lightColor.r;
            lightColorArray[1] = skin.lightColor.g;
            lightColorArray[2] = skin.lightColor.b;
            lightColorArray[3] = skin.lightColor.a;
            stream.SendNext(lightColorArray);

            float[] darkColorArray = new float[4];
            darkColorArray[0] = skin.darkColor.r;
            darkColorArray[1] = skin.darkColor.g;
            darkColorArray[2] = skin.darkColor.b;
            darkColorArray[3] = skin.darkColor.a;
            stream.SendNext(darkColorArray);
        }
        else
        {
            float[] baseColorArray = (float[])stream.ReceiveNext();
            skin.baseColor = new Color(baseColorArray[0], baseColorArray[1], baseColorArray[2], baseColorArray[3]);

            float[] lightColorArray = (float[])stream.ReceiveNext();
            skin.lightColor = new Color(lightColorArray[0], lightColorArray[1], lightColorArray[2], lightColorArray[3]);

            float[] darkColorArray = (float[])stream.ReceiveNext();
            skin.darkColor = new Color(darkColorArray[0], darkColorArray[1], darkColorArray[2], darkColorArray[3]);
        }
    }

    // Apply skin on ship
    public void ApplySkin()
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