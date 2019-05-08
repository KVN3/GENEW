using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    //public PlayerShip[] allShips { get; set; }
    //public PlayerShip[] shipOrder { get; set; }

    // Global variable
    public static bool raceStarted = false;

    public CountDownController countDownController;

    // Start is called before the first frame update
    void Start()
    {
        // TO DO: boolean use count down or not for testing
        Instantiate(countDownController, transform);

        //raceStarted = true;
    }

    private void Update()
    {
        //foreach (PlayerShip ship in allShips)
        //{
        //    shipOrder[ship.GetShipPos(allShips) - 1] = ship;
        //}
    }

    //public void SetPlayers(PlayerShip[] players)
    //{
    //    allShips = players;
    //    shipOrder = new PlayerShip[allShips.Length];
    //}
}
