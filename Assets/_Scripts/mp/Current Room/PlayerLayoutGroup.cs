﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerLayoutGroup : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _playerListingPrefab;
    private GameObject PlayerListingPrefab
    {
        get { return _playerListingPrefab; }
    }

    [SerializeField]
    private LevelPreview levelPreview;

    private List<PlayerListing> _playerListings = new List<PlayerListing>();
    private List<PlayerListing> PlayerListings
    {
        get { return _playerListings; }
    }

    public Image roomState;
    public Sprite unlockedSprite;
    public Sprite lockedSprite;


    #region PhotonCallbacks
    // I joined room
    public override void OnJoinedRoom()
    {
        if (GameConfiguration.tutorial)
            return;

        // Clear player list
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // Toggles the current room window
        MainCanvasManager.instance.ShowPanel(PanelType.ROOM);
        MainCanvasManager.instance.HidePanel(PanelType.LOBBY);


        // Update player list
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            PlayerJoinedRoom(players[i]);
        }

        // Disable/enable host specific buttons
        CurrentRoomCanvas.instance.ManageRoomButtonAccess();
    }

    // When host leaves room
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //PhotonNetwork.LeaveRoom();

        // Disable/enable host specific buttons
        CurrentRoomCanvas.instance.ManageRoomButtonAccess();
    }

    // Other joined room
    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        PlayerJoinedRoom(otherPlayer);

        // New player joined, send update
        if (PhotonNetwork.IsMasterClient)
            levelPreview.UpdateLevelPreview();
    }

    // Other left room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerLeftRoom(otherPlayer);

        // New player joined, send update
        if (PhotonNetwork.IsMasterClient)
            levelPreview.UpdateLevelPreview();
    }
    #endregion

    private void PlayerJoinedRoom(Player player)
    {
        if (player == null)
            return;

        // Prevention from duplicates if lag and player joins when already full
        PlayerLeftRoom(player);

        // Instantiate the button as a child of the Layout Group
        GameObject playerListingObj = Instantiate(PlayerListingPrefab);
        playerListingObj.transform.SetParent(transform, false);
        // Adds onClick to add friend
        playerListingObj.GetComponent<Button>().onClick.AddListener(delegate { Chat.instance.AddFriend(player.UserId); });

        // Add to listing
        PlayerListing playerListing = playerListingObj.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(player);
        PlayerListings.Add(playerListing);
    }

    private void PlayerLeftRoom(Player player)
    {
        int index = PlayerListings.FindIndex(i => i.PhotonPlayer == player);

        // Remove from listing and destroy button
        if (IndexFound(index))
        {
            Destroy(PlayerListings[index].gameObject);
            PlayerListings.RemoveAt(index);
        }
    }

    public void OnClickRoomState()
    {
        // Only room owner may change state
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (roomState.sprite == unlockedSprite)
            roomState.sprite = lockedSprite;
        else
            roomState.sprite = unlockedSprite;

        PhotonNetwork.CurrentRoom.IsOpen = !PhotonNetwork.CurrentRoom.IsOpen;
        PhotonNetwork.CurrentRoom.IsVisible = PhotonNetwork.CurrentRoom.IsOpen;
    }

    public void OnClickLeaveRoom()
    {
        Chat.instance.LeaveChat(Chat.instance.channelsToJoinOnConnect[2]);
        PhotonNetwork.LeaveRoom();
    }

    // If index == -1, index hasn't been found
    private bool IndexFound(int index)
    {
        if (index == -1)
            return false;
        else
            return true;
    }


}
