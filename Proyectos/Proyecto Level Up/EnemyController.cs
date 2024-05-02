using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class EnemyController : MonoBehaviour
{
 public NavMeshAgent nav;
 Transform target;
 [Header ("Vidas")]
 public float vidas = 5;
 public Image amount;
 public GameObject [] powerUps;
 public Animator anim;
 float timeAttack;
    
    private void Awake()
 {
    target = GameObject.FindGameObjectWithTag("Player").transform;
    target.GetComponent<PlayeController>().SetEnemies();
 }
    void Update()
    {
        if(target)
        {
            nav.SetDestination(target.position);
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= 1)
            {
                anim.SetBool("ataque", true);
                nav.speed = 0.1f;
                timeAttack += Time.deltaTime;
                if (timeAttack >= 1.5f)
                {
                    target.GetComponent<PlayeController>().damage();
                    timeAttack = 0;
                }
            }
            else
            {
                timeAttack = 0;
                anim.SetBool("ataque", false);
                nav.speed = 2.8f;
            }
        }
    
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.transform.CompareTag("bala"))
        {
            vidas --;
            amount.fillAmount = vidas /5f;
            if  (vidas <=0 )
            {
               
                int ran = UnityEngine.Random.Range(0, 10);
                if(ran >=0 && ran <7)
                {
                    GameObject newbala = Instantiate(powerUps[0]);
                    newbala.transform.position = transform.position;
                }
                else{
                    GameObject newbala = Instantiate(powerUps[Random.Range(1,powerUps.Length)]);
                    newbala.transform.position = transform.position;
                }

                
                transform.tag = "Untagged";
                target.GetComponent<PlayeController>().SetEnemies();

                GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
                gm.EnemyCount();
                Destroy(this.gameObject);               
            }
            
        } 
    }
    
    

}
