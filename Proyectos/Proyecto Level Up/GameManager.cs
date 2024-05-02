using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class GameManager : MonoBehaviour
{
    #region //niveles
    [Serializable]
    class Level
    {
        public Setting[] setting;
    }
    [Serializable]

    class Setting
    {
        public GameObject obj1;
        public Coordenadas coordenadasObj1;
        public GameObject obj2;
        public Coordenadas coordenadasObj2;
        public GameObject obj3;
        public Coordenadas coordenadasObj3;
        public GameObject obj4;
        public Coordenadas coordenadasObj4;
    }
    [Serializable]
    public class Coordenadas {
        public Vector3 pos;
        public Quaternion rot;
    }
    [SerializeField] Level level = new Level();

    #endregion 

    [Header("paneles")]
    public GameObject panelGameOver;
    public GameObject panelWin;

    [Header("PowerUps")]
    public GameObject[] powerups;

    [Header("Timer")]
    public TextMeshProUGUI text_Timer;
    float timer = 20;
    [Header("Level Counter")]
    public int levelCounter = 0;

    [Header("Enemy Setting")]
    public Transform[] enemySpawn;
    public GameObject[] enemyPrefab;
    public int[] livesEnemyForLevel;
    int enemycount;
    public GameObject meta;
    [Header ("audios")]
    public AudioSource fire;
    public AudioSource powerUp;
    public AudioSource dontshoot;
    private void Start()
    {


        if (PlayerPrefs.HasKey("Level"))
        {
            levelCounter = PlayerPrefs.GetInt("Level");
            if (levelCounter >= 3)
            {
                levelCounter = 1;
            }
        }

        InstaObstacle();
        IntaEnemies();
    }
    private void Timer()
    {
        if (timer > 0 && panelWin.activeSelf == false && gameOverUI.activeSelf == false)
        {
            timer -= Time.deltaTime;
            text_Timer.text = timer.ToString("F2");
        }
        if (timer <= 0)
        {
            gameover();


        }

    }
    public void Reaload()
    {
        SceneManager.LoadScene("Game");
    }
    public void InstaObstacle()
    {
       
        GameObject newObstacle1 = Instantiate(level.setting[levelCounter].obj1);
        newObstacle1.transform.position = level.setting[levelCounter].coordenadasObj1.pos;
        newObstacle1.transform.rotation = level.setting[levelCounter].coordenadasObj1.rot;

        GameObject newObstacle2 = Instantiate(level.setting[levelCounter].obj2);
        newObstacle2.transform.position = level.setting[levelCounter].coordenadasObj2.pos;
        newObstacle2.transform.rotation = level.setting[levelCounter].coordenadasObj2.rot;

        GameObject newObstacle3 = Instantiate(level.setting[levelCounter].obj3);
        newObstacle3.transform.position = level.setting[levelCounter].coordenadasObj3.pos;
        newObstacle3.transform.rotation = level.setting[levelCounter].coordenadasObj3.rot;

        GameObject newObstacle4 = Instantiate(level.setting[levelCounter].obj4);
        newObstacle4.transform.position = level.setting[levelCounter].coordenadasObj4.pos;
        newObstacle4.transform.rotation = level.setting[levelCounter].coordenadasObj4.rot;
    }
    public void IntaEnemies()
    {
        for (int i = 0; i < 4; i++)
        {
            int ran = UnityEngine.Random.Range(0, enemyPrefab.Length);
            GameObject newEnemy = Instantiate(enemyPrefab[ran]);
            newEnemy.transform.position = enemySpawn[i].position;
            newEnemy.GetComponent<EnemyController>().vidas = livesEnemyForLevel[levelCounter];
        }
    }
    public void EnemyCount()
    {
        enemycount++;
        if (enemycount >= 3) {
            
            meta.SetActive(true);
            enemycount = 0;
            
        }
    }
   
    private void Update()
    {
        if (meta.activeSelf) {
            
            GameObject.FindGameObjectWithTag("Player").transform.LookAt(meta.transform);
            
        }
         if (panelWin.activeSelf == false && gameOverUI.activeSelf == false)
        {
            Timer();
        }
    }
    
    public GameObject gameOverUI;

    public void gameover()
    {
        gameOverUI.SetActive(true);
    }
    public void restart()
    {
        SceneManager.LoadScene("jueho");
    }
    public void Nextlevel()
    {
        timer = 20;
        levelCounter++;
        if (levelCounter >= level.setting.Length)
        {
            levelCounter = 0;
        }
        enemycount = 0;
        meta.gameObject.SetActive(false);
        GameObject.Find("player").transform.position = new Vector3(0, 1, 0);
        PlayerPrefs.SetInt("Level", levelCounter);
        PlayerPrefs.Save();
        GameObject[] obstacle = GameObject.FindGameObjectsWithTag("obstacle");
        for (int i = 0; i < obstacle.Length; i++)
        {
            Destroy(obstacle[i].gameObject);
        }
        InstaObstacle();
        IntaEnemies();

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayeController>().enabled = true;
    }
}

