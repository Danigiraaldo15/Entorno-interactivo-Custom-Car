using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class powercollision : MonoBehaviour
{
    public float expandir = 5;
    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            CapsuleCollider collider = GetComponent<CapsuleCollider>();
            if (collider != null)
            {
                collider.radius = expandir;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            PhotonView playerPhotonView = other.gameObject.GetComponent<PhotonView>();

            if (playerMovement != null && playerMovement.lanzado)
            {
                Debug.Log("asdasd");
                // Solo aplicar el efecto al jugador que pasa por el trigger
                if (playerPhotonView != null)
                {
                    photonView.RPC("Aplicar", RpcTarget.All, playerPhotonView.ViewID);
                }
                else
                {
                    Debug.Log("playerPhotonView is null");
                }
            }
            else
            {
                Debug.Log("llorelo");
            }
        }
        
    }
    public enum powerup { Desmayo, Puntosmenos, lentitud };
    public powerup poweruptype;
    [PunRPC]
    public void Aplicar(int viewid)
    {
        Debug.Log("Método RPC 'Aplicar' llamado con viewid: " + viewid);
        PlayerMovement playerMovement = PhotonView.Find(viewid).GetComponent<PlayerMovement>();
        switch (poweruptype)
        {
            case powerup.Desmayo:
                StartCoroutine(efecto(playerMovement));
                break;
            case powerup.Puntosmenos:
                StartCoroutine(puntosmenos(playerMovement));
                break;
            case powerup.lentitud:
                StartCoroutine (lentitud(playerMovement));
                break;
        }
    }
    //aplica el efecto y despues de ciertos segundos vuelve a la normalidad
    public IEnumerator efecto(PlayerMovement playerMovement)
    {
        playerMovement.desmayo = true;
        yield return new WaitForSeconds(5);
        if(playerMovement.destruido) 
        {
            playerMovement.lanzado = false;
            playerMovement.desmayo = false;
            playerMovement.destruido = false;
        }
        else
        {
            Debug.Log("malo");
        }
        
    }
    //aplica el efecto y despues de ciertos segundos vuelve a la normalidad
    IEnumerator puntosmenos(PlayerMovement playerMovement)
    {
        playerMovement.powerupactivo = true;
        playerMovement.contador = playerMovement.contador -= 10;
        playerMovement.Puntos.text = playerMovement.contador.ToString();
        StartCoroutine(playerMovement.EfectoPerderMonedas());
        yield return new WaitForSeconds(10);
        if(playerMovement.destruido)
        {
            playerMovement.powerupactivo = false;
            playerMovement.destruido = false;
        }
        
    }
    //aplica el efecto y despues de ciertos segundos vuelve a la normalidad
    IEnumerator lentitud(PlayerMovement playerMovement)
    {
        float originalSpeed = playerMovement.MoveSpeed;
        playerMovement.MoveSpeed = 2;
        yield return new WaitForSeconds(5);
        if (playerMovement.destruido)
        {
            playerMovement.MoveSpeed = originalSpeed;
            playerMovement.destruido = false;
        }
    }
}
