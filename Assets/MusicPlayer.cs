using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    private AudioClip[] wasteLandClips;
    private AudioClip[] menuClips;

    private AudioSource audioSource;

    private string lastSceneName = "";


    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.priority = 32;
        audioSource.volume = 0.02f;

        ReadyClipsForUse();

        DontDestroyOnLoad(this);

        audioSource.Stop();
        StartCoroutine(PlayBackgroundTrack());
    }

    private void ReadyClipsForUse()
    {
        Object[] objects = Resources.LoadAll("Sounds/Wasteland");
        wasteLandClips = new AudioClip[objects.Length];

        int i = 0;
        foreach (Object obj in objects)
        {
            wasteLandClips[i] = (AudioClip)obj;
            i++;
        }

        objects = Resources.LoadAll("Sounds/Main");
        menuClips = new AudioClip[objects.Length];

        i = 0;
        foreach (Object obj in objects)
        {
            menuClips[i] = (AudioClip)obj;
            i++;
        }
    }

    private IEnumerator PlayBackgroundTrack()
    {
        string wastelandSceneName = ScenesInformation.sceneNames[SceneTitle.Wasteland];
        string mainSceneName = ScenesInformation.sceneNames[SceneTitle.Main];
        string shipyardSceneName = ScenesInformation.sceneNames[SceneTitle.Shipyard];

        while (true)
        {
            Scene currentScene = SceneManager.GetActiveScene();

            string sectionName = currentScene.name;
            if (currentScene.name.Equals(shipyardSceneName))
                sectionName = mainSceneName;

            if (ClientConfigurationManager.Instance.clientConfiguration.MusicOn)
            {
                audioSource.mute = false;
                if (!audioSource.isPlaying || !sectionName.Equals(lastSceneName))
                {
                
                    if (sectionName == wastelandSceneName)
                        audioSource.clip = wasteLandClips[Random.Range(0, wasteLandClips.Length)];
                    else if (sectionName == mainSceneName)
                        audioSource.clip = menuClips[Random.Range(0, menuClips.Length)];

                    audioSource.Play();
                }
            }
            else
                audioSource.mute = true;

            lastSceneName = sectionName;

            yield return new WaitForSeconds(0.1f);
        }
    }

    // Abstract

    protected static MusicPlayer _Instance;

    public static bool Initialized => _Instance != null;

    public static MusicPlayer Instance
    {
        get
        {
            if (!Initialized)
            {
                GameObject GameObject = new GameObject("Music Player");

                _Instance = GameObject.AddComponent<MusicPlayer>();
            }

            return _Instance;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void ForceInit()
    {
        MusicPlayer GI = Instance;
    }

}
