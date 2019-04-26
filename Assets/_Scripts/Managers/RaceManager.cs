using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    // Global variable
    public static bool raceStarted = false;

    public CountDownController countDownController;

    // Start is called before the first frame update
    void Start()
    {
        // TO DO: boolean use count down or not for testing
        //Instantiate(countDownController, transform);

        raceStarted = true;
    }
}
