using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ShipGun : ShipComponent
{
    #region Assigned variables
    // Spawnpoints
    [SerializeField]
    private LocalSpawnPoint projectileSpawnPoint;
    [SerializeField]
    private LocalSpawnPoint mineSP;
    [SerializeField]
    private LocalSpawnPoint smokeSP;

    // Weapon classes
    [SerializeField]
    private JammerProjectile jammerProjectileClass;
    [SerializeField]
    private JammerMine jammerMineClass;
    [SerializeField]
    private SmokeScreenItem smokeScreenClass;
    #endregion

    private bool coolDown;
    private PhotonView photonView;

    public virtual void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    #region Projectile
    public void Shoot(JammerProjectile projectileClass)
    {
        photonView.RPC("RPC_Shoot", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void RPC_Shoot(PhotonMessageInfo info)
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        Quaternion rotation = Quaternion.Euler(rot);

        JammerProjectile projectile = Instantiate(jammerProjectileClass, projectileSpawnPoint.transform.position, rotation);
        projectile.owner = parentShip;

        shipSoundManager.PlaySound(SoundType.SHOOTING);
        StartCoroutine(C_ApplyCooldown(.5f));
    }
    #endregion

    #region Mine
    public void DropMine(JammerMine mineClass)
    {
        photonView.RPC("RPC_DropMine", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void RPC_DropMine(PhotonMessageInfo info)
    {
        JammerMine mine = Instantiate(jammerMineClass, mineSP.transform.position, Quaternion.identity);
        mine.owner = parentShip;

        shipSoundManager.PlaySound(SoundType.DROPMINE);
        StartCoroutine(C_ApplyCooldown(.5f));
    }
    #endregion

    #region Smokescreen
    public void DropSmokeScreen(SmokeScreenItem smokeScreenClass)
    {
        photonView.RPC("PhotonDropSmokeScreen", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void PhotonDropSmokeScreen(PhotonMessageInfo info)
    {
        // Rotation
        Vector3 rot = transform.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y, rot.z);
        Quaternion rotation = Quaternion.Euler(rot);

        Instantiate(smokeScreenClass, smokeSP.transform.position, rotation);

        StartCoroutine(C_ApplyCooldown(.5f));
    }
    #endregion

    // Applies weapon usage cooldown. Duration in given seconds.
    private IEnumerator C_ApplyCooldown(float seconds)
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
