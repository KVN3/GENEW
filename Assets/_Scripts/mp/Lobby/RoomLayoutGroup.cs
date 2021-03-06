﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class RoomLayoutGroup : MonoBehaviourPunCallbacks
{
    public static RoomLayoutGroup Instance;

    [SerializeField]
    private GameObject _roomListingPrefab;
    private GameObject RoomListingPrefab
    {
        get { return _roomListingPrefab; }
    }

    [SerializeField]
    private List<RoomListing> _roomListingButtons = new List<RoomListing>();
    private List<RoomListing> RoomListingButtons
    {
        get { return _roomListingButtons; }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            print(PhotonNetwork.CountOfRooms);
        }
    }

    public bool RoomExists(string roomName)
    {
        bool exists = false;

        foreach (RoomListing roomListing in RoomListingButtons)
        {
            if (roomListing.RoomName.Equals(roomName))
            {
                exists = true;
                break;
            }
        }

        return exists;
    }

    // When room list gets updated, this is called
    public override void OnRoomListUpdate(List<RoomInfo> rooms)
    {
        print($"Updating room list... received {rooms.Count} rooms from Phtoon");

        List<RoomListing> removeRooms = new List<RoomListing>();

        foreach (RoomInfo room in rooms)
        {
            // Delete
            if (room.MaxPlayers == 0 && room.PlayerCount == 0)
            {
                int index = RoomListingButtons.FindIndex(i => i.RoomName == rooms[0].Name);

                if (Method.IndexFound(index))
                {
                    removeRooms.Add(RoomListingButtons[index]);
                }
            }

            // Add/Update
            else
            {
                RoomReceived(room);
            }
        }

        PerformRemovingRoomListings(removeRooms);
    }

    // If found, updates name
    // If not found and room is set to visible and has free slots left, create room and add to list
    private void RoomReceived(RoomInfo room)
    {


        // Go through all rooms, return room index with same name
        int index = RoomListingButtons.FindIndex(i => i.RoomName == room.Name);

        // If index not found, make listing
        if (!Method.IndexFound(index))
        {
            if (room.IsVisible && room.PlayerCount < room.MaxPlayers)
            {
                // Instantiate the button as a child of the Layout Group
                GameObject roomListingObj = Instantiate(RoomListingPrefab);
                roomListingObj.transform.SetParent(transform, false);

                // Add to RoomListing
                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                RoomListingButtons.Add(roomListing);

                index = (RoomListingButtons.Count - 1);
            }
        }

        // If index found, set updated
        if (Method.IndexFound(index))
        {

            if (room.MaxPlayers == 0 && room.PlayerCount == 0)
            {

            }
            else
            {
                // Update room name
                RoomListing roomListing = RoomListingButtons[index];
                roomListing.SetRoomNameText(room.Name);
                roomListing.SetPlayerCountText(room.PlayerCount, room.MaxPlayers);
                roomListing.Updated = true;
            }
        }
    }

    // Removes old rooms, those that haven't been updated
    private void RemoveOldRooms()
    {
        List<RoomListing> removeRooms = new List<RoomListing>();

        // If not updated, add to removeRooms list, else set updated to false for next sweep
        foreach (RoomListing roomListing in RoomListingButtons)
        {
            if (!roomListing.Updated)
                removeRooms.Add(roomListing);
            else
                roomListing.Updated = false;
        }

        // Remove all rooms marked for removal from list
        PerformRemovingRoomListings(removeRooms);
    }

    public void RemoveAllRooms()
    {
        List<RoomListing> removeRooms = new List<RoomListing>();

        foreach (RoomListing roomListing in RoomListingButtons)
        {
            removeRooms.Add(roomListing);
        }

        // Remove all rooms marked for removal from list
        PerformRemovingRoomListings(removeRooms);
    }

    private void PerformRemovingRoomListings(List<RoomListing> removeRooms)
    {
        foreach (RoomListing roomListing in removeRooms)
        {
            GameObject roomListingObj = roomListing.gameObject;
            RoomListingButtons.Remove(roomListing);
            Destroy(roomListingObj);
        }
    }
}



//// Photon bug calling this twice
//// If not my client

//// Received 1 room
//if (rooms.Count == 1)
//{
//    // This 1 room has the same name as this client's last room
//    if (CurrentRoomCanvas.instance.RoomName == rooms[0].Name)
//    {
//        // Do nothing with this update
//        return;
//    }

//    // This is a foreign update
//    else
//    {
//        // If the maxplayers and players received in the update for this room are both 0, remove this room and return
//        if (rooms[0].MaxPlayers == 0 && rooms[0].PlayerCount == 0)
//        {
//            List<RoomListing> removeRooms = new List<RoomListing>();

//            int index = RoomListingButtons.FindIndex(i => i.RoomName == rooms[0].Name);
//            removeRooms.Add(RoomListingButtons[index]);

//            PerformRemovingRoomListings(removeRooms);

//            return;
//        }
//    }
//}


//print($"Updating room list... received {rooms.Count} rooms from Phtoon");

//foreach (RoomInfo room in rooms)
//{

//    RoomReceived(room);
//}

//RemoveOldRooms();