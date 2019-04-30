using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ShipGun : ShipComponent
{
    public LocalSpawnPoint projectileSpawnPoint;
    public LocalSpawnPoint mineSP;
    public LocalSpawnPoint smokeSP;

    [SerializeField]
    private JammerProjectile jammerProjectileClass;

    [SerializeField]
    private JammerMine jammerMineClass;

    private bool coolDown;

    private PhotonView photonView;

    public virtual void Start()
    {
        photonView = PhotonView.Get(this);
    }

    public void Shoot(JammerProjectile projectileClass)
    {
        //This object's rotation + 180y
        Vector3 rot = transform.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        Quaternion rotation = Quaternion.Euler(rot);

        JammerProjectile projectile = Instantiate(projectileClass, projectileSpawnPoint.transform.position, rotation);
        projectile.owner = parentShip;

        shipSoundManager.PlaySound(SoundType.SHOOTING);
        StartCoroutine(ShootingCooldown(.5f));

        //photonView.RPC("Shoot", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void Shoot(PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        Vector3 rot = transform.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        Quaternion rotation = Quaternion.Euler(rot);

        JammerProjectile projectile = Instantiate(jammerProjectileClass, projectileSpawnPoint.transform.position, rotation);
        projectile.owner = parentShip;

        shipSoundManager.PlaySound(SoundType.SHOOTING);
        StartCoroutine(ShootingCooldown(.5f));
    }

    public void DropMine(JammerMine mineClass)
    {
        //JammerMine mine = Instantiate(mineClass, mineSP.transform.position, Quaternion.identity);
        //mine.owner = parentShip;

        //shipSoundManager.PlaySound(SoundType.DROPMINE);
        //StartCoroutine(ShootingCooldown(.5f));

        photonView.RPC("PhotonDropMine", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void PhotonDropMine(PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        JammerMine mine = Instantiate(jammerMineClass, projectileSpawnPoint.transform.position, Quaternion.identity);
        mine.owner = parentShip;

        shipSoundManager.PlaySound(SoundType.DROPMINE);
        StartCoroutine(ShootingCooldown(.5f));
    }

    public void DropSmokeScreen(SmokeScreenItem smokeScreenClass)
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y, rot.z);
        Quaternion rotation = Quaternion.Euler(rot);

        Instantiate(smokeScreenClass, smokeSP.transform.position, rotation);
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
