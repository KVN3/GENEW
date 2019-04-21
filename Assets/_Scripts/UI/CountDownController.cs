using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownController : MonoBehaviour
{
    public int CountDown { get; set; }
    public string CountDownText { get; set; }
    PlayerShip playerShip;

    // Start is called before the first frame update
    void Start()
    {
        CountDown = 3;
        CountDownText = CountDown.ToString();
        StartCoroutine(StartCountDown());
    }

    IEnumerator StartCountDown()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(1f);
            CountDown--;
            CountDownText = CountDown.ToString();
        }
        CountDownText = LocalizationManager.GetTextByKey("GO");
        RaceManager.raceStarted = true;
        yield return new WaitForSeconds(0.5f);
        CountDownText = "";
    }
}
