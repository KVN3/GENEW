﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConfiguration
{
    /// Game specific
    // Height (Y) at which the level will be played. Ships and enemies will generally aim to be at or around this height.
    public static float PlayingHeight = 7;

    public static bool tutorial;
    public static bool isPaused = false;
}
