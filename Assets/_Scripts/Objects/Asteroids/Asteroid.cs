using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public SmokeScreen smokeScreenClass;
    public Explosion explosionClass;
    public AsteroidStormManager manager;

    public int lifeTimeInSeconds;
    public float floatSpeed = 10f;

    public virtual void Start()
    {
        //List<Component> components = new List<Component>();
        //components.Add(GetComponent<Floater>());

        //GetComponent<PhotonView>().ObservedComponents = components;

        if (lifeTimeInSeconds != 0)
            StartCoroutine(WaitAndDestroy());
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(8);
        DestroyThisAsteroid();
    }



    void OnCollisionEnter(Collision other)
    {
        Explode();
    }

    private void Explode()
    {
        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC("RPC_ExplodeAsteroid", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void RPC_ExplodeAsteroid()
    {
        Instantiate(explosionClass, this.transform.position, Quaternion.identity);
        Instantiate(smokeScreenClass, this.transform.position, Quaternion.identity);
        DestroyThisAsteroid();
    }



    private void DestroyThisAsteroid()
    {

        Destroy(gameObject);

        if (PhotonNetwork.IsMasterClient)
            manager.RemoveAsteroidFromObjectList(this);
    }
}
