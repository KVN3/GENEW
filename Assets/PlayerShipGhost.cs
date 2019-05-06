using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerShipGhost : MonoBehaviour
{
    List<float> posx;
    List<float> posy;
    List<float> posz;
    List<Quaternion> rotations;

    public ReplayManager replayManager;

    int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        //ReplayManager replayManager = new ReplayManager();
        List<Replay> replays = replayManager.GetReplaysByStage(SceneManager.GetActiveScene().name);
        // Select random replay if there is a replay
        if (replays.Count > 0)
        {
            int i = Random.Range(0, replays.Count);
            Replay replay = replays[i];
            posx = replay.posx;
            posy = replay.posy; 
            posz = replay.posz; 
            rotations = replay.rotations; 
            StartCoroutine(startReplay());
        }
    }

    IEnumerator startReplay()
    {
        yield return new WaitForSeconds(0.1f);
        while(i < posx.Count)
        {
            if (Time.frameCount % 3 == 0) 
            {
                transform.rotation = rotations[i];
                //transform.position = Vector3.Lerp(transform.position, new Vector3(posx[i], posy[i], posz[i]), Time.deltaTime*3); // Time.deltaTime * frames measured
                transform.position = new Vector3(posx[i], posy[i], posz[i]);
                i++;
            }
            yield return null;
            //yield return new WaitForEndOfFrame(); // makes replay slightly faster
        }
    }
}
