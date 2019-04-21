using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : LevelSingleton<LocalizationManager>
{
    public static Language preferredLanguage;
    private static Dictionary<string, string> NLDict;
    private static Dictionary<string, string> ENDict;

    protected override void Awake()
    {
        DontDestroyOnLoad(this); // Persistent

        preferredLanguage = Language.Dutch;
        NLDict = new Dictionary<string, string>();
        ENDict = new Dictionary<string, string>();

        #region Dutch
        // Menutext
        NLDict.Add("MAIN_MENU", "HOOFDMENU");
        NLDict.Add("PLAY","Start");
        NLDict.Add("SINGLEPLAYER", "Eén speler");
        NLDict.Add("MULTIPLAYER", "Meerdere spelers");
        NLDict.Add("LEVEL", "Level");
        NLDict.Add("OPTIONS", "Opties");
        NLDict.Add("MUSIC", "Muziek");
        NLDict.Add("SOUND", "Geluid");
        NLDict.Add("SKIN", "Skin");
        NLDict.Add("ON", "AAN");
        NLDict.Add("OFF", "UIT");
        NLDict.Add("CREATE_LOBBY", "Creëer lobby");
        NLDict.Add("JOIN_LOBBY", "Verbind met lobby");
        NLDict.Add("EXIT_LOBBY", "Verlaat lobby");
        NLDict.Add("LOADING", "Laden");
        NLDict.Add("QUIT_GAME", "Verlaat spel");
        NLDict.Add("VERSION", "Versie");

        NLDict.Add("NAME", "Naam");

        // Explanation
        NLDict.Add("ESC_TO_QUIT", "Druk esc om het spel te verlaten");
        NLDict.Add("R_TO_RESTART", "Druk R om te herstarten");
        
        // In-Game HUD
        NLDict.Add("SPEEDUNIT", "KM/U");
        NLDict.Add("LAP", "Ronde");
        NLDict.Add("POS", "Pos");
        NLDict.Add("CURRENT", "HUIDIG");
        NLDict.Add("BEST", "BEST");

        // Popups
        NLDict.Add("GO", "START");
        NLDict.Add("WRONG_WAY", "VERKEERDE RICHTING");
        NLDict.Add("FINAL_LAP", "LAATSTE RONDE");

        NLDict.Add("RACE_RESULTS", "RACE RESULTATEN");
        
        NLDict.Add("BEST_LAPTIME", "Beste rondetijd");
        NLDict.Add("BEST_TIME", "Beste tijd");
        NLDict.Add("TOTAL_TIME", "Totale tijd");
        #endregion

        #region English

        // Menutext
        ENDict.Add("MAIN_MENU", "MAIN MENU");
        ENDict.Add("PLAY", "Play");
        ENDict.Add("SINGLEPLAYER", "Singleplayer");
        ENDict.Add("MULTIPLAYER", "Multiplayer");
        ENDict.Add("LEVEL", "Level");
        ENDict.Add("OPTIONS", "Options");
        ENDict.Add("MUSIC", "Music");
        ENDict.Add("SOUND", "Sound");
        ENDict.Add("ON", "ON");
        ENDict.Add("OFF", "OFF");
        ENDict.Add("JOIN_LOBBY", "Join lobby");
        ENDict.Add("CREATE_LOBBY", "Create lobby");
        ENDict.Add("EXIT_LOBBY", "Leave lobby");
        ENDict.Add("LOADING", "Loading");
        ENDict.Add("QUIT_GAME", "Quit game");
        ENDict.Add("VERSION", "Version");

        ENDict.Add("NAME", "Name");

        // Explanations
        ENDict.Add("ESC_TO_QUIT", "Press esc to quit");
        ENDict.Add("R_TO_RESTART", "Press R to restart");

        // In-Game HUD
        ENDict.Add("SPEEDUNIT", "KM/H");
        ENDict.Add("LAP", "Lap");
        ENDict.Add("POS", "Pos");
        ENDict.Add("CURRENT", "CURR");
        ENDict.Add("BEST", "BEST");

        // Popups
        ENDict.Add("GO", "GO!");
        ENDict.Add("WRONG_WAY", "WRONG WAY");
        ENDict.Add("FINAL_LAP", "FINAL LAP");

        ENDict.Add("RACE_RESULTS", "RACE RESULTS");
        ENDict.Add("BEST_LAPTIME", "Best laptime");
        ENDict.Add("BEST_TIME", "Best time");
        ENDict.Add("TOTAL_TIME", "Total time");

        #endregion
    }

    public static string GetTextByKey(string key)
    {
        switch (preferredLanguage)
        {
            case Language.Dutch:
                if (NLDict.ContainsKey(key))
                    return NLDict[key];
                else
                    return "ERROR";
            default:
            case Language.English:
                if (ENDict.ContainsKey(key))
                    return ENDict[key];
                else
                    return "ERROR";
        }
    }
}
