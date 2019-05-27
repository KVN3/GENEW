using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownController : MonoBehaviour
{
    public int CountDown { get; set; }
    public string CountDownText { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        CountDown = 3;
        CountDownText = LocalizationManager.GetTextByKey("GET_A_FAST_TIME");
        StartCoroutine(StartCountDown());
    }

    IEnumerator StartCountDown()
    {
        //if () // if NOT in tut level 
        //{
            CountDownText = LocalizationManager.GetTextByKey("GET_A_FAST_TIME");
            yield return new WaitForSeconds(2f);
            CountDownText = CountDown.ToString();
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(1f);
                CountDown--;
                Debug.Log(CountDown);
                CountDownText = CountDown.ToString();
            }
            RaceManager.raceStarted = true;
            CountDownText = LocalizationManager.GetTextByKey("GO");
            yield return new WaitForSeconds(0.5f);
            CountDownText = "";
        //}
        //else
        //  Racemanager.raceStarted = true;
    }
}
