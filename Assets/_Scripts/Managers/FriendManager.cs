using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FriendManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void AddFriend(string name)
    {
        // Load account
        Account account = Registration.GetCurrentAccount();
        
        // Add if doesnt contain friend
        if (!account.friendList.Contains(name) && name != account.username)
            account.friendList.Add(name);

        // Update
        Registration.SaveAccount(account);
    }

    public void RemoveFriend(string name)
    {
        // Load account
        Account account = Registration.GetCurrentAccount();

        // Add if doesnt contain friend
        if (account.friendList.Contains(name))
            account.friendList.Remove(name);

        // Update
        Registration.SaveAccount(account);
    }

    #region Singleton

    // Abstract

    protected static FriendManager _Instance;

    public static bool Initialized => _Instance != null;

    public static FriendManager Instance
    {
        get
        {
            if (!Initialized)
            {
                GameObject gameObject = new GameObject("Friend Manager");

                _Instance = gameObject.AddComponent<FriendManager>();
            }

            return _Instance;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void ForceInit()
    {
        FriendManager GI = Instance;
    }
    #endregion
}