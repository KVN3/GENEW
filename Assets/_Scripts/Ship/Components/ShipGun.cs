using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGun : ShipComponent
{
    public LocalSpawnPoint projectileSpawnPoint;

    private bool coolDown;

    public void Shoot(JammerProjectile projectileClass)
    {
        // This object's rotation + 180y
        Vector3 rot = transform.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        Quaternion rotation = Quaternion.Euler(rot);

        JammerProjectile projectile = (JammerProjectile)Instantiate(projectileClass, projectileSpawnPoint.transform.position, rotation);
        projectile.owner = parentShip;

        shipSoundManager.PlaySound(SoundType.SHOOTING);
        StartCoroutine(ShootingCooldown(.5f));

    }

    private IEnumerator ShootingCooldown(float seconds)
    {
        coolDown = true;
        yield return new WaitForSeconds(seconds);
        coolDown = false;
    }

    

    #region GetSet
    public bool OnCooldown()
    {
        return coolDown;
    }
    #endregion
}
