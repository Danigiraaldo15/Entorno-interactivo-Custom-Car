using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    
    public enum powerUp {MUNICION, SPEED, ESCUDO};
    public powerUp powerUpType;
    

    
    void Update()
    {
    transform.Rotate(Vector3.forward * 1.5f);    
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            switch(powerUpType)
            {
                case powerUp.MUNICION:
                    other.GetComponent<PlayeController>().balaCount += 10;
                    break;
                case powerUp.ESCUDO:
                    other.GetComponent<PlayeController>().InitActieShield();
                    break;
            }
            GameObject.Find("GameManager").GetComponent<GameManager>().powerUp.Play();
            Destroy(this.gameObject);
            
        }
    }
}
