using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVida : MonoBehaviour
{
    
    public float Vida;
    public float maxVida;
    public Image ivida;
    private bool isDead;
    public GameManager gameManager;
    
    void Start()
    {
        maxVida = Vida;
    }

    
    void Update()
    { 
        ivida.fillAmount = Mathf.Clamp(Vida / maxVida, 0, 1);
        if (Vida <= 0 && !isDead)
        {

            isDead = true;
            gameObject.SetActive(false);
            gameManager.gameover();
            Debug.Log("Dead");
        }
    }
    
}
