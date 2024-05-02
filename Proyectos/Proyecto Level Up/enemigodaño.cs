using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class enemigodaño : MonoBehaviour
{
    private PlayerVida pVida;
    
    public float damage;
    
    void Start()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            pVida = jugador.GetComponent<PlayerVida>();
        }
        
    }

    
    void Update()
    {
        if (pVida != null)
        {
            float distance = Vector3.Distance(transform.position, pVida.transform.position);
            if (distance <= 1.5f)
            {
                pVida.Vida -= damage;
            }
        }
    }
    
}
