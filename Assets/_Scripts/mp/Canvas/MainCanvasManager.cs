using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType
{
    LOBBY, ROOM, MAIN
}

public class MainCanvasManager : MonoBehaviour
{
    public static MainCanvasManager instance;

    [SerializeField]
    private MainMenuManager mainMenu;
    public MainMenuManager MainMenu
    {
        get { return mainMenu; }
    }


    [SerializeField]
    private LobbyCanvas _lobbyCanvas;
    public LobbyCanvas LobbyCanvas
    {
        get { return _lobbyCanvas; }
    }

    [SerializeField]
    private CurrentRoomCanvas _currentRoomCanvas;
    public CurrentRoomCanvas CurrentRoomCanvas
    {
        get { return _currentRoomCanvas; }
    }

    private void Awake()
    {
        instance = this;
    }

    #region PANEL MANAGEMENT
    public void ShowPanel(string panelName)
    {
        Enum.TryParse(panelName.ToUpper(), out PanelType panelType);
        RectTransform transform = SelectCorrectTransform(panelType);
        transform.localPosition = new Vector2(0, 0);
    }


    public void HidePanel(string panelName)
    {
        Enum.TryParse(panelName.ToUpper(), out PanelType panelType);
        RectTransform transform = SelectCorrectTransform(panelType);
        transform.localPosition = new Vector2(9999f, 0);
    }

    public void ShowPanel(PanelType panelType)
    {
        RectTransform transform = SelectCorrectTransform(panelType);
        transform.localPosition = new Vector2(0, 0);
    }


    public void HidePanel(PanelType panelType)
    {
        RectTransform transform = SelectCorrectTransform(panelType);
        transform.localPosition = new Vector2(9999f, 0);
    }

    // Get the correct panel's transform component by panel type.
    private RectTransform SelectCorrectTransform(PanelType panelType)
    {
        RectTransform transform = null;

        switch (panelType)
        {
            case PanelType.LOBBY:
                transform = LobbyCanvas.GetComponent<RectTransform>();
                break;
            case PanelType.ROOM:
                transform = CurrentRoomCanvas.GetComponent<RectTransform>();
                break;
            default:
                print("No panel type specified in MainCanvasManager.");
                break;
        }

        return transform;
    }
    #endregion
}
