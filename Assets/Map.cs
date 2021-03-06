﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Photon.Pun;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{
    // 3D worldspace to 2D factor
    private float factorX, factorZ;

    // 3D world space origin of the 2D map
    private Vector3 mapOrigin;

    private PlayerShip[] playerShips;
    private Image[] playerShipIcons;
    private Dictionary<PlayerShip, Image> iconDict = new Dictionary<PlayerShip, Image>();

    [SerializeField]
    private Image playerIconClass;
    [SerializeField]
    private float mainPlayerIconMultiplier = 1.5f;

    private bool iconsSpawned = false;

    private void Awake()
    {

    }

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Equals("Tutorial"))
            return;

        StartCoroutine(C_Initialize());
    }

    private void FixedUpdate()
    {
        if (!iconsSpawned)
            return;

        // Update icon position
        for (int i = 0; i < playerShips.Length; i++)
        {
            PlayerShip playerShip = playerShips[i];

            try
            {
                // Compensate for different level/map origins
                float shipX = playerShip.transform.position.x - mapOrigin.x;
                float shipZ = playerShip.transform.position.z - mapOrigin.z;

                // Get the current map location
                float currentMapX = (shipX / factorX);
                float currentMapZ = (shipZ / factorZ);

                Image playerIcon = iconDict[playerShip];
                playerIcon.transform.localPosition = new Vector2(currentMapX, currentMapZ);
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException || ex is MissingReferenceException)
                    UpdateList(playerShip);
            }

        }
    }

    private void UpdateList(PlayerShip playerShip)
    {
        if (playerShip == null)
        {
            int shipIndex = Array.IndexOf(playerShips, playerShip);
            playerShips = playerShips.Where((val, idx) => idx != shipIndex).ToArray();
        }
        else
        {
            print("Catching wrong error in Map !!!");
        }
    }

    #region INIT
    // Init references needed
    private IEnumerator C_Initialize()
    {
        SetLevelDependentSettings();

        // Find all player ship game objects first
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Ship");
        while (gameObjects.Length != PhotonNetwork.PlayerList.Length)
        {
            gameObjects = GameObject.FindGameObjectsWithTag("Ship");
            yield return new WaitForSeconds(.1f);
        }

        // Get the playership scripts from the objects
        playerShips = new PlayerShip[gameObjects.Length];
        for (int i = 0; i < gameObjects.Length; i++)
        {
            playerShips[i] = gameObjects[i].GetComponent<PlayerShip>();
        }

        SpawnIcons();
    }
    #endregion

    #region LEVELS
    private void SetLevelDependentSettings()
    {
        float mapX = 0f, mapZ = 0f;

        // Get the scenename
        string sceneName = SceneManager.GetActiveScene().name;

        // Turn the correct background on
        transform.Find(sceneName).gameObject.SetActive(true);

        // Set scene specific settings
        if (sceneName.Equals(ScenesInformation.sceneNames[SceneTitle.WASTELAND]))
        {
            //transform.Rotate(new Vector3(0, 0, -90f));

            // Conversion settings
            mapOrigin = new Vector3(-280, 0, 950);
            mapX = 4096;
            mapZ = 4096;
        }
        else if (sceneName.Equals(ScenesInformation.sceneNames[SceneTitle.HIGHWAY]))
        {


            // Conversion settings
            mapOrigin = new Vector3(200, 0, -680);
            mapX = 6144;
            mapZ = 6144;
        }

        // Set the 3D worldspace to 2D image conversion factor
        Rect rect = GetComponent<RectTransform>().rect;
        factorX = mapX / rect.width;
        factorZ = mapZ / rect.height;
    }
    #endregion


    #region ICONS
    // Spawn player ship icons
    private void SpawnIcons()
    {
        foreach (PlayerShip playerShip in playerShips)
        {
            // Spawn icon and link to ship
            Image playerIcon = Instantiate(playerIconClass, this.transform);
            iconDict.Add(playerShip, playerIcon);

            // Manage appearance for this icon
            ManageIconAppearance(playerIcon, playerShip);

            // Icons always in front of background
            playerIcon.transform.SetAsLastSibling();
        }

        iconsSpawned = true;
    }

    private void ManageIconAppearance(Image playerIcon, PlayerShip playerShip)
    {
        ShipSkin skin = playerShip.GetComponent<SkinManager>().GetShipSkin();

        // Set base color based on wing color of ship
        Color color = skin.baseColor;
        color.a = 1f;
        playerIcon.color = color;

        if (playerShip.isMine)
        {
            // Bigger main player
            playerIcon.rectTransform.localScale = new Vector2(playerIcon.rectTransform.localScale.x * mainPlayerIconMultiplier,
                playerIcon.rectTransform.localScale.y * mainPlayerIconMultiplier);
        }
        else
        {
            // Disable glow for other players
            playerIcon.transform.Find("Glow").gameObject.SetActive(false);
        }
    }
    #endregion
}
