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
    public Color minimapBlipColor;
}


public class SkinManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private bool applySkin;

    [SerializeField]
    private ShipSkin skin;

    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // If my ship, use my playerprefs to set color
        if (photonView.IsMine)
        {
            skin.baseColor = PlayerPrefsX.GetColor("REGULAR_COLOR");
            skin.lightColor = PlayerPrefsX.GetColor("LIGHT_COLOR");
            skin.darkColor = PlayerPrefsX.GetColor("DARK_COLOR");
            skin.minimapBlipColor = Color.gray;
        }
        else
        {
            skin.minimapBlipColor = Color.red;
        }

        StartCoroutine(C_ApplySkins());
    }

    // Refresh skins
    private IEnumerator C_ApplySkins()
    {
        while (true)
        {
            photonView.RPC("RPC_ApplySkins", RpcTarget.AllBuffered);

            yield return new WaitForSeconds(60);
        }
    }

    [PunRPC]
    private void RPC_ApplySkins()
    {
        ApplySkins();
    }

    // Apply skin on ship
    private void ApplySkins()
    {
        if (applySkin)
        {
            // Go through all child transforms to this ship transform
            foreach (Transform shipChildTransform in transform)
            {
                // Apply forcefield skin to the forcefield shipcomponent
                if (shipChildTransform.name.Equals("Components"))
                    ApplyForceFieldSkin(shipChildTransform);

                // Apply skin to the mesh
                else if (shipChildTransform.name.Equals("Mesh"))
                    ApplyShipSkin(shipChildTransform);

                else if (shipChildTransform.name.Equals("RadarBlip"))
                    ApplyRadarColor(shipChildTransform);
            }
        }
    }

    private void ApplyRadarColor(Transform radarBlipTransform)
    {
        Renderer renderer = radarBlipTransform.GetComponent<Renderer>();
        Material[] mats = renderer.materials;

        foreach (Material mat in mats)
        {
            mat.color = skin.minimapBlipColor;

            print("Set radar blip color");

        }
            
    }

    // Apply skin to forcefield
    private void ApplyForceFieldSkin(Transform componentsTransform)
    {
        // Go through each component, looking for the forcefield
        foreach (Transform componentTransform in componentsTransform)
        {
            if (componentTransform.name.Equals("Forcefield"))
            {
                Renderer renderer = componentTransform.GetComponent<Renderer>();
                Material[] mats = renderer.materials;

                // Apply skin color
                foreach (Material mat in mats)
                    mat.color = skin.forcefieldColor;
            }
        }
    }

    // Apply skin to base
    private void ApplyShipSkin(Transform meshTransform)
    {
        // Go through each grouping
        foreach (Transform meshGroupingTransform in meshTransform)
        {
            // Go through each part
            foreach (Transform partTransform in meshGroupingTransform)
            {
                // Go through each single mesh object that comprises the part
                foreach (Transform singleObject in partTransform)
                {
                    Renderer renderer = singleObject.GetComponent<Renderer>();
                    Material[] mats = renderer.materials;

                    // Apply the colors
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
}
