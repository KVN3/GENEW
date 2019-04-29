using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasManager : MonoBehaviour
{
    public static MainCanvasManager instance;

    [SerializeField]
    private LobbyCanvas _lobbyCanvas;
    private LobbyCanvas LobbyCanbas
    {
        get { return _lobbyCanvas; }
    }



    private void Awake()
    {
        instance = this;
    }
}
