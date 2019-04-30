using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundPlayer : MonoBehaviour
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
        audioSource.volume = 0.1f;

        ReadyClipsForUse();

        DontDestroyOnLoad(this);

        audioSource.Stop();
        // TO DO UNCOMMENT
        //StartCoroutine(PlayBackgroundTrack());
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
        while (true)
        {
            Scene currentScene = SceneManager.GetActiveScene();

            string sectionName = currentScene.name;
            if (currentScene.name.Equals("Ship Customization"))
                sectionName = "Main Menu";

            if (!audioSource.isPlaying || !sectionName.Equals(lastSceneName))
            {
                switch (sectionName)
                {
                    case "Wasteland":
                        audioSource.clip = wasteLandClips[Random.Range(0, wasteLandClips.Length)];
                        break;
                    case "Main Menu":
                        audioSource.clip = menuClips[Random.Range(0, menuClips.Length)];
                        break;
                }

                audioSource.Play();
            }

            lastSceneName = sectionName;

            yield return new WaitForSeconds(2);
        }
    }

    // Abstract

    protected static BackgroundPlayer _Instance;

    public static bool Initialized => _Instance != null;

    public static BackgroundPlayer Instance
    {
        get
        {
            if (!Initialized)
            {
                GameObject GameObject = new GameObject("Background Player");

                _Instance = GameObject.AddComponent<BackgroundPlayer>();
            }

            return _Instance;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void ForceInit()
    {
        BackgroundPlayer GI = Instance;
    }

}
