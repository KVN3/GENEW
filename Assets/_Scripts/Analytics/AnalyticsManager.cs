using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Assertions;

public class AnalyticsManager : MyMonoBehaviour
{
    public static Dictionary<string, object> itemUsedDict = new Dictionary<string, object>();
    public static Dictionary<string, object> playerStunnedDict = new Dictionary<string, object>();

    [NonSerialized]
    public PlayerShip playerShip;

    public void Start()
    {
        if (playerShip != null)
        {
            playerShip.OnItemUsedDelegate = (Collectable item, int itemAmount) =>
            {
                ItemUsedEvent(item, itemAmount);
            };

            playerShip.OnPlayerStunnedDelegate = (int duration, string cause, bool playerWasProtected) =>
            {
                PlayerStunnedEvent(duration, cause, playerWasProtected);
            };
        }
    }

    private void ItemUsedEvent(Collectable item, int itemAmount)
    {
        itemUsedDict["item"] = item.ToString();
        Analytics.CustomEvent("ItemUsed", itemUsedDict);
    }

    private void PlayerStunnedEvent(int duration, string cause, bool wasProtected)
    {
        playerStunnedDict["duration"] = duration;
        playerStunnedDict["cause"] = cause;
        playerStunnedDict["wasProtected"] = wasProtected;
        Analytics.CustomEvent("PlayerStunned", playerStunnedDict);
    }
}