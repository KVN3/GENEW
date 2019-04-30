using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class RoomLayoutGroup : MonoBehaviourPunCallbacks
{
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            print(PhotonNetwork.CountOfRooms);
        }
    }

    // When room list gets updated, this is called
    public override void OnRoomListUpdate(List<RoomInfo> rooms)
    {
        foreach (RoomInfo room in rooms)
        {
            RoomReceived(room);
        }

        RemoveOldRooms();

        print("Updated room list");
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
            // Update room name
            RoomListing roomListing = RoomListingButtons[index];
            roomListing.SetRoomNameText(room.Name);
            roomListing.Updated = true;
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
        foreach (RoomListing roomListing in removeRooms)
        {
            GameObject roomListingObj = roomListing.gameObject;
            RoomListingButtons.Remove(roomListing);
            Destroy(roomListingObj);
        }
    }
}
