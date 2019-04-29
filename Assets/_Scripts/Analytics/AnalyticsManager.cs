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
    public static Dictionary<string, object> dict = new Dictionary<string, object>();

    [NonSerialized]
    public PlayerShip playerShip;

    public void Start()
    {
        playerShip.OnItemUsedDelegate = (Collectable item, int itemAmount) =>
        {
            ItemUsedEvent(item, itemAmount);
        };
    }

    private void ItemUsedEvent(Collectable item, int itemAmount)
    {
        //dict["item_amount"] = itemAmount;
        Analytics.CustomEvent("ItemUsed", dict);
    }
}

