using Photon.Pun;
using UnityEngine;

public class DDOL : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    protected static DDOL _Instance;

    public static bool Initialized => _Instance != null;

    public static DDOL Instance
    {
        get
        {
            if (!Initialized)
            {
                GameObject gameObject = new GameObject("DDOL");

                // Player Network
                GameObject playerNetwork = new GameObject("PlayerNetwork");
                playerNetwork.AddComponent<PlayerNetwork>();
                PhotonView pv = playerNetwork.AddComponent<PhotonView>();
                pv.ViewID = 999;

                // Player Manager
                GameObject playerManager = new GameObject("PlayerManager");
                playerManager.AddComponent<PlayerManager>();

                // Set parents
                playerNetwork.transform.parent = gameObject.transform;
                playerManager.transform.parent = gameObject.transform;

                _Instance = gameObject.AddComponent<DDOL>();
            }

            return _Instance;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void ForceInit()
    {
        DDOL DDOL = Instance;
    }
}
