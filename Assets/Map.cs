using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class Map : MonoBehaviour
{
    // Unity 3D worldspace units
    private readonly float mapX = 4096, mapZ = 4096;
    private float factorX, factorZ;

    // 3D world space origin of the 2D map
    private Vector3 mapOrigin = new Vector3(-280, 0, 950);

    private PlayerShip playerShip;
    public void SetPlayerShip(PlayerShip playerShip)
    {
        this.playerShip = playerShip;
    }

    private Image playerIcon;

    private void Awake()
    {
        playerIcon = transform.Find("PlayerIcon").GetComponent<Image>();
        Assert.IsNotNull(playerIcon);

        // Set the 3D worldspace to 2D image conversion factor
        Rect rect = GetComponent<RectTransform>().rect;
        factorX = mapX / rect.width;
        factorZ = mapZ / rect.height;
    }

    private void FixedUpdate()
    {
        // Compensate for different level/map origins
        float shipX = playerShip.transform.position.x - mapOrigin.x;
        float shipZ = playerShip.transform.position.z - mapOrigin.z;

        // Get the current map location
        float currentMapX = (shipX / factorX);
        float currentMapZ = (shipZ / factorZ);

        print($"ShipX {shipX}, MapX {currentMapX} || ShipZ {shipZ}, MapZ {currentMapZ} ");

        playerIcon.transform.localPosition = new Vector2(currentMapX, currentMapZ);
    }


}
