using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayeController : MonoBehaviour
{
    [Header("Player anim")]
    public float speed = 5;
    public Animator anim;
    private bool estavivo = true;
    [Header("Arma")]
    public GameObject balaPrefab;
    public Transform balaSpawn;
    public int balaCount = 10;
    public GameObject escudo;
    

    [SerializeField] GameObject[] enemies;
    private void Start()
    {
        
    }

    void Update()
    {
        MovePlayer();
        MoveWithRay();
        Fire();
        
    }
    void MovePlayer()
    {
        float V = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        float H = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        transform.position = new Vector3(transform.position.x + H, 0, transform.position.z + V);
        if (V != 0 || H != 0)
        {
            anim.SetBool("run", true);
        }
        else
        {
            anim.SetBool("run", false);
        }
    }
    void MoveWithRay()
    {
        if (enemies.Length > 0)
        {
            Vector3 targetPostition = new Vector3(GetClosestEnemy(enemies).position.x, this.transform.position.y, GetClosestEnemy(enemies).position.z);
            transform.LookAt(targetPostition);
        }
    }
    void Fire()
    {
        if (Input.GetButtonDown("Fire1") && balaCount > 0)
        {
            anim.SetBool("Fire", true);
            GameObject newBala = Instantiate(balaPrefab, balaSpawn.position, balaSpawn.rotation);
            newBala.GetComponent<Rigidbody>().velocity = balaSpawn.TransformDirection(Vector3.forward * 20);
            Destroy(newBala, 3);
            //Falta particulas para cañon de arma
            balaCount--;
            GameObject.Find("GameManager").GetComponent<GameManager>().fire.Play();
        }
        if (balaCount <= 0 && Input.GetButtonDown("Fire1"))
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().dontshoot.Play();
            
        }
        if (Input.GetButtonUp("Fire1"))
        {
            anim.SetBool("Fire", false);
        }
    }
    public void SetEnemies()
    {
        enemies = null;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }
    Transform GetClosestEnemy(GameObject[] enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in enemies)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("meta"))
        {
        
            GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            gm.panelWin.SetActive(true);
            other.transform.gameObject.SetActive(false);
            this.GetComponent<PlayeController>().enabled = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            estavivo = false;
            damage();
            
        }
    }
    public void InitActieShield()
    {
        StartCoroutine(ActiveShield());
    }
    IEnumerator ActiveShield()
    {
        escudo.SetActive(true);
        yield return new WaitForSeconds(5);
        escudo.SetActive(false);

    }

    public void damage()
    {
        if (!escudo.activeSelf)
        {
            GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            gm.panelGameOver.SetActive(true);
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].gameObject.SetActive(false);
            }
            this.GetComponent<PlayeController>().enabled = false;
        }
    }
}