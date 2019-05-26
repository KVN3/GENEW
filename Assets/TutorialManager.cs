using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject playerShip;
    public GameObject playerCamera;


    // Start is called before the first frame update
    void Awake()
    {
        PhotonNetwork.OfflineMode = true;

       
    }

    void Start()
    {
        PlayerShip ps = playerShip.GetComponent<PlayerShip>();

        PlayerCamera camera = playerCamera.GetComponent<PlayerCamera>();

        PlayerController pc = playerShip.GetComponent<PlayerController>();
        pc.SetPlayerCamera(camera);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
