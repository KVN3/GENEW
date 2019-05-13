using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalizationManager : LevelSingleton<LocalizationManager>
{
    public static Language preferredLanguage;
    private static Dictionary<string, string> NLDict;
    private static Dictionary<string, string> ENDict;

    protected override void Awake()
    {
        DontDestroyOnLoad(this); // Persistent

        preferredLanguage = Language.English;
        NLDict = new Dictionary<string, string>();
        ENDict = new Dictionary<string, string>();

        #region Dutch
        // Menutext
        NLDict.Add("MAIN_MENU", "HOOFDMENU");
        NLDict.Add("SHIPYARD", "Scheepswerf");
        NLDict.Add("PLAY","Start");
        NLDict.Add("SINGLEPLAYER", "Singleplayer");
        NLDict.Add("MULTIPLAYER", "Multiplayer");
        NLDict.Add("LEVEL", "Level");
        NLDict.Add("OPTIONS", "Opties");
        NLDict.Add("MUSIC", "Muziek");
        NLDict.Add("SOUND", "Geluid");
        NLDict.Add("SHIPCOLOR", "Schipkleur");
        NLDict.Add("EDIT_SHIP", "Bewerk schip");
        NLDict.Add("RETURN", "Terug");
        NLDict.Add("RETURN_TO_MAIN_MENU", "Terug naar hoofdmenu");
        NLDict.Add("ON", "AAN");
        NLDict.Add("OFF", "UIT");
        NLDict.Add("LOBBY", "Lobby");
        NLDict.Add("ROOM_NAME", "Kamernaam");
        NLDict.Add("CREATE_ROOM", "Creëer kamer");
        NLDict.Add("LEAVE_ROOM", "Verlaat kamer");
        NLDict.Add("START_MATCH", "Start potje");
        NLDict.Add("PUBLIC_ROOM", "Openbare kamer");
        NLDict.Add("LOADING", "Laden");
        NLDict.Add("QUIT_GAME", "Verlaat spel");
        NLDict.Add("VERSION", "Versie");
        NLDict.Add("ACHIEVEMENTS", "Prestaties");
        NLDict.Add("COMPLETED", "Compleet");
        NLDict.Add("UNCOMPLETED", "Niet compleet");

        // Account
        NLDict.Add("REGISTRATION", "Registratie");
        NLDict.Add("REGISTER", "Registreren");
        NLDict.Add("LOGIN", "Log in");
        NLDict.Add("LOGGED_IN_AS", "Ingelogd als");
        NLDict.Add("NOT_LOGGED_IN", "Niet ingelogd");
        NLDict.Add("HAVE_ACCOUNT", "Al een account?");
        NLDict.Add("DONT_HAVE_ACCOUNT", "Geen account?");
        NLDict.Add("USERNAME", "Gebruikersnaam");
        NLDict.Add("PASSWORD", "Wachtwoord");
        NLDict.Add("CONFIRM_PASSWORD", "Bevestig wachtwoord");
        NLDict.Add("FRIENDS", "Vrienden");

        // Help text
        NLDict.Add("HELP_REGISTRATION", "Vul deze velden hieronder in. \nDit account slaat je vrienden, voortgang en prestaties op.");
        NLDict.Add("HELP_LOGIN", "Log in om te spelen");

        // Validation messages
        NLDict.Add("PASSWORDS_NOT_MATCH", "Ingelogd als");
        NLDict.Add("PASSWORD_LONGER_1", "Wachtwoord moet langer zijn dan");
        NLDict.Add("PASSWORD_LONGER_2", "karakters!");
        NLDict.Add("FIELDS_EMPTY", "Er zijn lege velden!");
        NLDict.Add("ACCOUNT_EXISTS", "Account bestaat al!");
        NLDict.Add("ACCOUNT_NOT_EXIST", "Account bestaat niet!");
        NLDict.Add("WRONG_PASSWORD", "Wachtwoord is fout!");

        // Ship customisation
        NLDict.Add("RED", "Rood");
        NLDict.Add("BLUE", "Blauw");
        NLDict.Add("GREEN", "Groen");

        NLDict.Add("NAME", "Naam");

        // Explanation
        NLDict.Add("ESC_TO_QUIT", "Druk esc om het spel te verlaten");
        NLDict.Add("R_TO_RESTART", "Druk R om te herstarten");
        
        // In-Game HUD
        NLDict.Add("SPEEDUNIT", "KM/U");
        NLDict.Add("LAP", "Ronde");
        NLDict.Add("POS", "Pos");
        NLDict.Add("CURRENT_TIME", "HUIDIG");
        NLDict.Add("BEST", "BEST");

        // Popups
        NLDict.Add("GET_A_FAST_TIME", "Haal de snelste rondetijd!");
        NLDict.Add("GO", "START");
        NLDict.Add("WRONG_WAY", "VERKEERDE RICHTING");
        NLDict.Add("FINAL_LAP", "LAATSTE RONDE");

        // Items
        NLDict.Add("E_TO_USE_ITEM", "Druk 'E' om voorwerp te gebruiken");
        NLDict.Add("USE_ITEM_MINE", "Druk 'E' om een verlammende mine te plaatsen");
        NLDict.Add("USE_ITEM_MISSILE", "Druk 'E' om een verlammend projectiel te schieten");
        NLDict.Add("USE_ITEM_SMOKE", "Druk 'E' om een rookbom te laten vallen");
        NLDict.Add("USE_ITEM_BOOST", "Druk 'E' om speed burst item te gebruiken");
        NLDict.Add("USE_ITEM_BARRIER", "Druk 'E' om forcefield item te gebruiken");

        // End screen
        NLDict.Add("RACE_RESULTS", "RACE RESULTATEN");
        NLDict.Add("LAP_TIMES", "Rondetijden");
        NLDict.Add("BEST_LAPTIME", "Beste rondetijd");
        NLDict.Add("BEST_TIME", "Beste tijd");
        NLDict.Add("TOTAL_TIME", "Totale tijd");
        NLDict.Add("LEADERBOARD", "Leaderboard");
        NLDict.Add("TIME", "Tijd");
        NLDict.Add("USER", "Gebruiker");
        #endregion

        #region English

        // Menutext
        ENDict.Add("MAIN_MENU", "MAIN MENU");
        ENDict.Add("SHIPYARD", "Shipyard");
        ENDict.Add("PLAY", "Play");
        ENDict.Add("SINGLEPLAYER", "Singleplayer");
        ENDict.Add("MULTIPLAYER", "Multiplayer");
        ENDict.Add("LEVEL", "Level");
        ENDict.Add("OPTIONS", "Options");
        ENDict.Add("MUSIC", "Music");
        ENDict.Add("SOUND", "Sound");
        ENDict.Add("ON", "ON");
        ENDict.Add("OFF", "OFF");
        ENDict.Add("SHIPCOLOR", "Shipcolor");
        ENDict.Add("EDIT_SHIP", "Edit ship");
        ENDict.Add("RETURN", "Return");
        ENDict.Add("RETURN_TO_MAIN_MENU", "Return to main menu");
        ENDict.Add("LOBBY", "Lobby");
        ENDict.Add("CREATE_ROOM", "Create room");
        ENDict.Add("ROOM_NAME", "Room name");
        ENDict.Add("LEAVE_ROOM", "Leave room");
        ENDict.Add("START_MATCH", "Start match");
        ENDict.Add("PUBLIC_ROOM", "Public room");
        ENDict.Add("LOADING", "Loading");
        ENDict.Add("QUIT_GAME", "Quit game");
        ENDict.Add("VERSION", "Version");
        ENDict.Add("ACHIEVEMENTS", "ACHIEVEMENTS");
        ENDict.Add("COMPLETED", "Completed");
        ENDict.Add("UNCOMPLETED", "Uncompleted");

        // Account
        ENDict.Add("REGISTRATION", "Registration");
        ENDict.Add("LOGIN", "Login");
        ENDict.Add("REGISTER", "Register");
        ENDict.Add("LOGGED_IN_AS", "Logged in as");
        ENDict.Add("NOT_LOGGED_IN", "Not logged in");
        ENDict.Add("HAVE_ACCOUNT", "Have an account?");
        ENDict.Add("DONT_HAVE_ACCOUNT", "Don't have an account?");
        ENDict.Add("USERNAME", "Username");
        ENDict.Add("PASSWORD", "Password");
        ENDict.Add("CONFIRM_PASSWORD", "Confirm password");
        ENDict.Add("FRIENDS", "Friends");

        // Help text
        ENDict.Add("HELP_REGISTRATION", "Please fill in these fields to create an account. \nThis account saves your friends, progress and achievements.");
        ENDict.Add("HELP_LOGIN", "Please login to play");

        // Validation messages
        ENDict.Add("PASSWORDS_NOT_MATCH", "Passwords do no match!");
        ENDict.Add("PASSWORD_LONGER_1", "Password has to be longer than");
        ENDict.Add("PASSWORD_LONGER_2", "characters!");
        ENDict.Add("FIELDS_EMPTY", "You have empty fields!");
        ENDict.Add("ACCOUNT_EXISTS", "Account already exists!");
        ENDict.Add("ACCOUNT_NOT_EXIST", "Account does not exist!");
        ENDict.Add("WRONG_PASSWORD", "Wrong password!");

        // Ship customisation
        ENDict.Add("RED", "Red");
        ENDict.Add("BLUE", "Blue");
        ENDict.Add("GREEN", "Green");

        ENDict.Add("NAME", "Name");

        // Explanations
        ENDict.Add("ESC_TO_QUIT", "Press esc to quit");
        ENDict.Add("R_TO_RESTART", "Press R to restart");

        // In-Game HUD
        ENDict.Add("SPEEDUNIT", "KM/H");
        ENDict.Add("LAP", "Lap");
        ENDict.Add("POS", "Pos");
        ENDict.Add("CURRENT_TIME", "CURR");
        ENDict.Add("BEST", "BEST");

        // Popups
        ENDict.Add("GET_A_FAST_TIME", "Get the fastest laptime!");
        ENDict.Add("GO", "GO!");
        ENDict.Add("WRONG_WAY", "WRONG WAY");
        ENDict.Add("FINAL_LAP", "FINAL LAP");

        // Items
        ENDict.Add("E_TO_USE_ITEM", "Press 'E' to use item");
        ENDict.Add("USE_ITEM_MINE", "Press 'E' to place a jammer mine");
        ENDict.Add("USE_ITEM_MISSILE", "Press 'E' to shoot jammer projectile");
        ENDict.Add("USE_ITEM_SMOKE", "Press 'E' to drop a smoke bomb");
        ENDict.Add("USE_ITEM_BOOST", "Press 'E' to use speedburst item");
        ENDict.Add("USE_ITEM_BARRIER", "Press 'E' to use forcefield item");


        // End screen
        ENDict.Add("RACE_RESULTS", "RACE RESULTS");
        ENDict.Add("LAP_TIMES", "Lap times");
        ENDict.Add("BEST_LAPTIME", "Best laptime");
        ENDict.Add("BEST_TIME", "Best time");
        ENDict.Add("TOTAL_TIME", "Total time");
        ENDict.Add("LEADERBOARD", "Leaderboard");
        ENDict.Add("TIME", "Time");
        ENDict.Add("USER", "User");

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

    public void SetLanguage(int languageIndex)
    {
        preferredLanguage = (Language)languageIndex;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Abstract

    protected static LocalizationManager _Instance;

    public static bool Initialized => _Instance != null;

    public static LocalizationManager Instance
    {
        get
        {
            if (!Initialized)
            {
                GameObject gameObject = new GameObject("Localization Manager");

                _Instance = gameObject.AddComponent<LocalizationManager>();
            }

            return _Instance;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void ForceInit()
    {
        LocalizationManager GI = Instance;
    }
}
