using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalizationManager : LevelSingleton<LocalizationManager>
{
    // PreferredLanguage is the language being used.
    // KEYS are CASE-SENSITIVE
    public static Language chosenLanguage;
    private static Dictionary<string, string> NLDict;
    private static Dictionary<string, string> ENDict;

    protected override void Awake()
    {
        DontDestroyOnLoad(this); // Persistent

        chosenLanguage = ClientConfigurationManager.Instance.clientConfiguration.preferredLanguage; // This is by default set in ClientConfigurationManager
        NLDict = new Dictionary<string, string>();
        ENDict = new Dictionary<string, string>();


        UpdateDutchDictionaryStatic();
        UpdateEnglishDictionaryStatic();
    }
    private static void UpdateDutchDictionaryStatic()
    {
        NLDict.Add("MAIN_MENU", "HOOFDMENU");
        NLDict.Add("SHIPYARD", "Scheepswerf");
        NLDict.Add("PLAY", "Start");
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
        NLDict.Add("CREATE_ROOM", "Maak kamer aan");
        NLDict.Add("LEAVE_ROOM", "Verlaat kamer");
        NLDict.Add("START_MATCH", "Start potje");
        NLDict.Add("IN_ROOM", "In kamer");
        NLDict.Add("CHAT", "Chat");
        NLDict.Add("CONNECTING", "Verbinding maken");
        NLDict.Add("ENTER_CHAT_MESSAGE", "Vul chatbericht in...");
        NLDict.Add("HAS_JOINED", " is toegetreden");
        NLDict.Add("PUBLIC_ROOM", "Openbare kamer");
        NLDict.Add("LOADING", "Laden...");
        NLDict.Add("LOGOUT", "Log uit");
        NLDict.Add("QUIT_GAME", "Verlaat spel");
        NLDict.Add("EXIT_GAME", "Verlaat spel");
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

        // Friends
        NLDict.Add("FRIENDS", "Vrienden");
        NLDict.Add("OPEN_FRIENDS", "Open vrienden");
        NLDict.Add("ADD_FRIEND", "Vriend toevoegen");
        NLDict.Add("DELETE", "Verwijderen");
        NLDict.Add("FRIEND_TO_ADD", "Vriend om toe te voegen...");

        // Leaderboard
        NLDict.Add("LEADERBOARD", "Leaderboard");
        NLDict.Add("RANK", "Rank");
        NLDict.Add("NAME", "Naam");
        NLDict.Add("TIME", "Tijd");

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
        NLDict.Add("BASE_COLOR", "Hoofdkleur");
        NLDict.Add("THRUSTER_COLOR", "Propulsorkleur");
        NLDict.Add("BODY_COLOR", "Dekkleur");

        // Explanation
        NLDict.Add("ESC_TO_QUIT", "Druk esc om het spel te verlaten"); // Not used
        NLDict.Add("R_TO_RESTART", "Druk R om te herstarten"); // Not used

        // In-Game HUD
        NLDict.Add("SPEEDUNIT", "KM/U");
        NLDict.Add("LAP", "Ronde");
        NLDict.Add("POS", "Pos");
        NLDict.Add("CURRENT_TIME", "HUIDIG");
        NLDict.Add("BEST", "BEST");

        // Popups
        NLDict.Add("GET_A_FAST_TIME", "Haal de snelste tijd!");
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
        NLDict.Add("USER", "Gebruiker");
        NLDict.Add("SPECTATING_NEXT", "AAN HET TOESCHOUWEN\n Druk op 'E' om de volgende speler te toeschouwen");
        NLDict.Add("TO_LOBBY_HELP", "Druk op 'P' om naar lobby te gaan");
        NLDict.Add("TO_LOBBY", "Naar lobby (P)");

        // Achievements
        NLDict.Add("A1_NAME", "Basis geleerd");
        NLDict.Add("A2_NAME", "Bronze wasteland");
        NLDict.Add("A3_NAME", "Zilveren wasteland");
        NLDict.Add("A4_NAME", "Gouden wasteland");
        NLDict.Add("A5_NAME", "Bronze highway");
        NLDict.Add("A6_NAME", "Zilveren highway");
        NLDict.Add("A7_NAME", "Gouden highway");
        NLDict.Add("A8_NAME", "Boosters voor losers");
        NLDict.Add("A9_NAME", "Kijk mam geen items");
        NLDict.Add("A10_NAME", "Sharing is caring");
        NLDict.Add("A11_NAME", "Onstunbaar");
        NLDict.Add("A12_NAME", "Snelheidsduivel");
        NLDict.Add("A13_NAME", "Zijn we er al?");
        NLDict.Add("A14_NAME", "Blokkeermeester");
        NLDict.Add("A15_NAME", "Beschermer");
        NLDict.Add("A16_NAME", "Kampieon");
        NLDict.Add("A17_NAME", "Items voor dagen");
        NLDict.Add("A18_NAME", "Boost naar oneindige");
        NLDict.Add("A19_NAME", "Item misselijkheid");
        NLDict.Add("A20_NAME", "Totaal geboost");

        NLDict.Add("A1_DESC", "Complete the tutorial level");
        NLDict.Add("A2_DESC", "Versla de bronze tijd opEraarlonium Wasteland");
        NLDict.Add("A3_DESC", "Versla de zilveren tijd op Eraarlonium Wasteland");
        NLDict.Add("A4_DESC", "Versla de gouden tijd op Eraarlonium Wasteland");
        NLDict.Add("A5_DESC", "Versla de bronze tijd op Elto Highway");
        NLDict.Add("A6_DESC", "Versla de zilveren tijd op Elto Highway");
        NLDict.Add("A7_DESC", "Versla de gouden tijd op Elto Highway");
        NLDict.Add("A8_DESC", "Haal een race zonder enige vorm van boost te gebruiken");
        NLDict.Add("A9_DESC", "Haal een race zonder items te gebruiken");
        NLDict.Add("A10_DESC", "Raak een persoon met een jammer raker of jammer mijn");
        NLDict.Add("A11_DESC", "Don't get stunned by anything");
        NLDict.Add("A12_DESC", "Behaal een snelheid van 600 km/h of hoger");
        NLDict.Add("A13_DESC", "Reis 50km (Ongeveer 17 laps) TOTAAL");
        NLDict.Add("A14_DESC", "Blokkeer 4 projectielen in één race");
        NLDict.Add("A15_DESC", "Blokkeer 20 projectielen TOTAAL");
        NLDict.Add("A16_DESC", "Wees nummer één op de leaderboard bij een level");
        NLDict.Add("A17_DESC", "Gebruik 4 items in a één race");
        NLDict.Add("A18_DESC", "Gebruik 10 boostpads in a één race");
        NLDict.Add("A19_DESC", "Gebruik 20 items TOTAAL");
        NLDict.Add("A20_DESC", "Gebruik 50 boostpads TOTAAL");
    }

    private static void UpdateEnglishDictionaryStatic()
    {

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
        ENDict.Add("CHAT", "Chat");
        ENDict.Add("CONNECTING", "Connecting");
        ENDict.Add("ENTER_CHAT_MESSAGE", "Enter chat message...");
        ENDict.Add("HAS_JOINED", " has joined");
        ENDict.Add("IN_ROOM", "In room");
        ENDict.Add("PUBLIC_ROOM", "Public room");
        ENDict.Add("LOADING", "Loading...");
        ENDict.Add("LOGOUT", "Logout");
        ENDict.Add("QUIT_GAME", "Quit game");
        ENDict.Add("EXIT_GAME", "Exit game");
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

        // Friends
        ENDict.Add("FRIENDS", "Friends");
        ENDict.Add("OPEN_FRIENDS", "Open friends");
        ENDict.Add("ADD_FRIEND", "Add Friend");
        ENDict.Add("DELETE", "Delete");
        ENDict.Add("FRIEND_TO_ADD", "Friend to add...");

        // Leaderboard
        ENDict.Add("LEADERBOARD", "Leaderboard");
        ENDict.Add("RANK", "Rank");
        ENDict.Add("NAME", "Name");
        ENDict.Add("TIME", "Time");

        // Help text
        ENDict.Add("HELP_REGISTRATION", "Please fill in these fields to create an account. \nThis account saves your friends, progress and achievements.");
        ENDict.Add("HELP_LOGIN", "Please login to play");

        // Validation messages
        ENDict.Add("PASSWORDS_NOT_MATCH", "Passwords do not match!");
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
        ENDict.Add("BASE_COLOR", "Base colour");
        ENDict.Add("THRUSTER_COLOR", "Thruster colour");
        ENDict.Add("BODY_COLOR", "Body colour");

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
        ENDict.Add("GET_A_FAST_TIME", "Get the fastest time!");
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
        ENDict.Add("USER", "User");
        ENDict.Add("SPECTATING_NEXT", "SPECTATING\n Press 'E' to spectate next player");
        ENDict.Add("TO_LOBBY_HELP", "Press 'P' to return to lobby");
        ENDict.Add("TO_LOBBY", "To lobby (P)");

        // Achievements
        ENDict.Add("A1_NAME", "Learned the basics");
        ENDict.Add("A2_NAME", "Bronze wasteland");
        ENDict.Add("A3_NAME", "Silver wasteland");
        ENDict.Add("A4_NAME", "Gold wasteland");
        ENDict.Add("A5_NAME", "Bronze highway");
        ENDict.Add("A6_NAME", "Silver highway");
        ENDict.Add("A7_NAME", "Gold highway");
        ENDict.Add("A8_NAME", "Boosters for losers");
        ENDict.Add("A9_NAME", "Look mom no items");
        ENDict.Add("A10_NAME", "Sharing is caring");
        ENDict.Add("A11_NAME", "Unstunnable");
        ENDict.Add("A12_NAME", "Speed demon");
        ENDict.Add("A13_NAME", "Are we there yet?");
        ENDict.Add("A14_NAME", "Blockmaster");
        ENDict.Add("A15_NAME", "Guardian");
        ENDict.Add("A16_NAME", "Champion");
        ENDict.Add("A17_NAME", "Items 4 days");
        ENDict.Add("A18_NAME", "Boosted to infinity");
        ENDict.Add("A19_NAME", "Item sickness");
        ENDict.Add("A20_NAME", "Totally boosted");


        ENDict.Add("A1_DESC", "Complete the tutorial level");
        ENDict.Add("A2_DESC", "Beat bronze time on Eraarlonium Wasteland");
        ENDict.Add("A3_DESC", "Beat silver time on Eraarlonium Wasteland");
        ENDict.Add("A4_DESC", "Beat gold time on Eraarlonium Wasteland");
        ENDict.Add("A5_DESC", "Beat bronze time on Elto Highway");
        ENDict.Add("A6_DESC", "Beat silver time on Elto Highway");
        ENDict.Add("A7_DESC", "Beat gold time on Elto Highway");
        ENDict.Add("A8_DESC", "Complete a race without any kind of boost");
        ENDict.Add("A9_DESC", "Complete a race without using any items");
        ENDict.Add("A10_DESC", "Hit a person with a jammer rocket or jammer mine");
        ENDict.Add("A11_DESC", "Don't get stunned by anything");
        ENDict.Add("A12_DESC", "Achieve a speed of 600 km/h or higher");
        ENDict.Add("A13_DESC", "Travel 50km (about 17 laps) TOTAL");
        ENDict.Add("A14_DESC", "Block 4 projectiles in a single race");
        ENDict.Add("A15_DESC", "Block 20 projectiles TOTAL");
        ENDict.Add("A16_DESC", "Be number one on the leaderboard on any level");
        ENDict.Add("A17_DESC", "Use 4 items in a single race");
        ENDict.Add("A18_DESC", "Use 10 boostpads in a single race");
        ENDict.Add("A19_DESC", "Use 20 items TOTAL");
        ENDict.Add("A20_DESC", "Use 50 boostpads TOTAL");
    }

    #region GetTextByKey

    public static string GetTextByKey(string key)
    {
        try
        {
            switch (chosenLanguage)
            {
                case Language.Dutch:
                    if (NLDict.ContainsKey(key))
                        return NLDict[key];
                    else
                        return key;
                default:
                case Language.English:
                    if (ENDict.ContainsKey(key))
                        return ENDict[key];
                    else
                        return key;
            }
        }
        catch (Exception ex)
        {
            print("KEY NOT FOUND IN A DICT: " + key);
            NLDict = new Dictionary<string, string>();
            ENDict = new Dictionary<string, string>();
            UpdateDutchDictionaryStatic();
            UpdateEnglishDictionaryStatic();
        }

        return key;
    }

    #endregion

    public void SetLanguage(int languageIndex)
    {
        chosenLanguage = (Language)languageIndex;
        // Change preference
        ClientConfigurationManager.Instance.clientConfiguration.preferredLanguage = (Language)languageIndex;
        ClientConfigurationManager.Instance.SavePlayerSettings();
    }

    #region  Singleton
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
    #endregion
}
