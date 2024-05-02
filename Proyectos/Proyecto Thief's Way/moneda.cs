using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class moneda : MonoBehaviour
{
    PhotonView pv;
    // Este es el codigo para ejecutar las particulas de destrucción de un objeto arrojadizo.

    //public ParticleSystem DestroyObject_Particle; // En este espacio van las particulas que se ejecutaran

    private void Update()
    {
        transform.Rotate(Vector3.right * 2.5f);

    }
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }
    public void MoveTowardsPlayer(Vector3 playerPosition, float speed)
    {
        Vector3 newPosition = Vector3.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);
        transform.position = newPosition;
        Debug.Log("Movimiento iniciado en " + gameObject.name);
        pv.RPC("SyncMovement", RpcTarget.All, newPosition);
    }

    [PunRPC]
    public void SyncMovement(Vector3 newPosition)
    {
        Debug.Log("RPC SyncMovement llamado en " + gameObject.name);
        transform.position = newPosition;
    }

}
