using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// Friend UI item used to represent the friend status as well as message. 
/// It aims at showing how to share health for a friend that plays on a different room than you for example.
/// But of course the message can be anything and a lot more complex.
/// </summary>
/// 
public class Friend : MonoBehaviour, IPointerExitHandler
{
    [HideInInspector]
    public string FriendId
    {
        set
        {
            NameLabel.text = value;
        }
        get
        {
            return NameLabel.text;
        }
    }

    public TextMeshProUGUI NameLabel;
    public TextMeshProUGUI StatusLabel;
    public GameObject RemoveBtn;
    public TextMeshProUGUI DeleteText;

    private void Update()
    {
        DeleteText.text = LocalizationManager.GetTextByKey("DELETE");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RemoveBtn.SetActive(false);
    }

    public void ToggleMiniMenu()
    {
        RemoveBtn.SetActive(!RemoveBtn.activeSelf);
    }

    public void RemoveFriend()
    {
        Chat.instance.RemoveFriend(FriendId);
    }

    public void OnFriendStatusUpdate(int status, bool gotMessage, object message)
    {
        string _status;

        switch (status)
        {
            case 1:
                _status = "Invisible";
                break;
            case 2:
                _status = "Online";
                break;
            case 3:
                _status = "Away";
                break;
            case 4:
                _status = "Do not disturb";
                break;
            case 5:
                _status = "Looking For Game/Group";
                break;
            case 6:
                _status = "Playing";
                break;
            default:
                _status = "Offline";
                break;
        }

        StatusLabel.text = _status;
    }
}

