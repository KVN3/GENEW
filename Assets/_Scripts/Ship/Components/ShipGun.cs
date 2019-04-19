using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGun : ShipComponent
{
    public LocalSpawnPoint projectileSpawnPoint;

    public void Shoot(JammerProjectile projectileClass)
    {
        // This object's rotation + 180y
        Vector3 rot = transform.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        Quaternion rotation = Quaternion.Euler(rot);

        JammerProjectile projectile = (JammerProjectile)Instantiate(projectileClass, projectileSpawnPoint.transform.position, rotation);
        projectile.owner = parentShip;

        shipSoundManager.PlaySound(SoundType.SHOOTING);
    }

    #region GetSet

    #endregion
}
