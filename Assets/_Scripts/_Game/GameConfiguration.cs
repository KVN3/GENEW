using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConfiguration
{
    /// Game specific
    // Height (Y) at which the level will be played. Ships and enemies will generally aim to be at or around this height.
    public static float PlayingHeight = 7;


    /// 
    /// Player settings specific
    /// Standard values that should get overwritten once you played the game
    /// 
    public static bool SawChangelog = false;
    public static bool SawNewAchievement = true;

    // Music & sound 
    public static bool MusicOn = true;
    public static bool SoundOn = true;
}
