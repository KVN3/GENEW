using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ShipGun : ShipComponent
{
    public LocalSpawnPoint projectileSpawnPoint;

    //public JammerProjectile jammerProjectileClass;

    private bool coolDown;

    //[PunRPC]
    //public void Fire(PhotonMessageInfo info)
    //{
    //    float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

    //    Vector3 rot = transform.rotation.eulerAngles;
    //    rot = new Vector3(rot.x, rot.y + 180, rot.z);
    //    Quaternion rotation = Quaternion.Euler(rot);

    //    JammerProjectile projectile = Instantiate(jammerProjectileClass, projectileSpawnPoint.transform.position, rotation);
    //    projectile.owner = parentShip;

    //    shipSoundManager.PlaySound(SoundType.SHOOTING);
    //    StartCoroutine(ShootingCooldown(.5f));
    //}

    public void Shoot(JammerProjectile projectileClass)
    {
        // This object's rotation + 180y
        Vector3 rot = transform.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        Quaternion rotation = Quaternion.Euler(rot);

        JammerProjectile projectile = Instantiate(projectileClass, projectileSpawnPoint.transform.position, rotation);
        projectile.owner = parentShip;

        shipSoundManager.PlaySound(SoundType.SHOOTING);
        StartCoroutine(ShootingCooldown(.5f));

    }

    public void DropMine(JammerMine mineClass)
    {
        JammerMine mine = (JammerMine)Instantiate(mineClass, projectileSpawnPoint.transform.position, Quaternion.identity);
        mine.owner = parentShip;

        shipSoundManager.PlaySound(SoundType.DROPMINE);
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
