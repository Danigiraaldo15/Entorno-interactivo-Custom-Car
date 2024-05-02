using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class EnemyBehaviour : MonoBehaviourPunCallbacks
{
    public PlayerMovement player;
    public Timer[] reftimerEnemy;
    public Transform[] PatrolPoints;

    [Header("Var - FLOAT / INT")]
    public int speed;
    public int TargetPoints;
    public int Damage = 2;
    public float lessTime = 10f;
    void Start()
    {
        TargetPoints = 0;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.GetComponent<PlayerMovement>();
            }

            if (transform.position == PatrolPoints[TargetPoints].position)
            {
                ChangePoint();
            }
            transform.position = Vector3.MoveTowards(transform.position, PatrolPoints[TargetPoints].position, speed * Time.deltaTime);

            Vector3 direction = PatrolPoints[TargetPoints].position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
    void ChangePoint()
    {
        if (photonView.IsMine)
        {
            TargetPoints++;
            if (TargetPoints >= PatrolPoints.Length)
            {
                TargetPoints = 0;
            }
        }
    }

    [PunRPC]
    void LessTimeAndPointsOnTouch(int playerActorNumber)
    {
        // Reducir el tiempo para todos los jugadores
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(tiempored());
        }
    }

    IEnumerator tiempored()
    {
        foreach (Timer REFTime in reftimerEnemy)
        {
            REFTime.timerValue -= lessTime;
        }

        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        yield return new WaitForSeconds(7);

        gameObject.GetComponent<CapsuleCollider>().enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                if (playerMovement.contador > 0)
                {
                    StartCoroutine(playerMovement.EfectoPerderMonedas());
                    playerMovement.contador = 0;
                    playerMovement.Puntos.text = playerMovement.contador.ToString();
                }
            }

            if (photonView.IsMine)
            {
                photonView.RPC("LessTimeAndPointsOnTouch", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }
}
