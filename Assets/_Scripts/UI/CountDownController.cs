using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CountDownController : MonoBehaviour
{
    public int CountDown { get; set; }
    public string CountDownText { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name != ScenesInformation.sceneNames[SceneTitle.TUTORIAL])
        {
            CountDown = 3;
            CountDownText = LocalizationManager.GetTextByKey("GET_A_FAST_TIME");
            StartCoroutine(StartCountDown());
        }
        else
        {
            RaceManager.raceStarted = true;
            CountDownText = "";
        }
    }

    IEnumerator StartCountDown()
    {
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
        CountDownText = LocalizationManager.GetTextByKey("GO");
        RaceManager.raceStarted = true;
        yield return new WaitForSeconds(0.5f);
        CountDownText = "";
    }
}
