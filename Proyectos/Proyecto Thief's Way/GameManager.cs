using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   
    [Header("PowerUps")]
    public GameObject[] powerups;
    [Header("Spawn Position")]
    public Transform[] PowerSpawn;
    private void Start()
    {
        InstanciaPower();
    }
    public void InstanciaPower()
    {
        int randomIndex = Random.Range(0, PowerSpawn.Length);
        Transform selectedSpawnPoint = PowerSpawn[randomIndex];

        // Obtiene la posición del punto de spawn seleccionado
        Vector3 spawnPosition = selectedSpawnPoint.position;

        // Genera un índice aleatorio para seleccionar un power-up
        int randomPowerUpIndex = Random.Range(0, powerups.Length);
        GameObject newPowerUp = Instantiate(powerups[randomPowerUpIndex], spawnPosition, Quaternion.identity);
    }

}
