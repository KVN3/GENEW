using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CountDownController : MonoBehaviour
{
    public int CountDown { get; set; }
    public string CountDownText { get; set; }

    public int photonId;
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        photonView.ViewID = photonId;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name != ScenesInformation.sceneNames[SceneTitle.TUTORIAL])
        {
            RaceManager.raceStarted = false;
            CountDown = 3;
            CountDownText = LocalizationManager.GetTextByKey("GET_A_FAST_TIME");

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(C_StartCountdown());
            }
        }
        else
        {
            RaceManager.raceStarted = true;
            CountDownText = "";
        }
    }

    IEnumerator C_StartCountdown()
    {
        // Find all player ship game objects first
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Ship");
        while (gameObjects.Length != PhotonNetwork.PlayerList.Length)
        {
            gameObjects = GameObject.FindGameObjectsWithTag("Ship");
            yield return new WaitForSeconds(1);
        }

        // Init text wait
        yield return new WaitForSeconds(1f);

        // Countdown
        photonView.RPC("RPC_Countdown", RpcTarget.AllBufferedViaServer, CountDown);
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(1f);

            CountDown--;
            Debug.Log(CountDown);
            photonView.RPC("RPC_Countdown", RpcTarget.AllBufferedViaServer, CountDown);  
        }

        // GO
        photonView.RPC("RPC_ShowText", RpcTarget.AllBufferedViaServer, "GO");

        yield return new WaitForSeconds(0.5f);
        photonView.RPC("RPC_ShowText", RpcTarget.AllBufferedViaServer, "");
    }

    [PunRPC]
    public void RPC_Countdown(int count)
    {
        CountDownText = count.ToString();
    }

    [PunRPC]
    public void RPC_ShowText(string keyString)
    {
        CountDownText = LocalizationManager.GetTextByKey(keyString);

        if (keyString.Equals("GO"))
        {
            RaceManager.raceStarted = true;
        }
        else if (keyString.Equals(""))
        {
            CountDownText = "";
        }
    }
}

