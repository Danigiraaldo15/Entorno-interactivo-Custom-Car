using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;


public class Timer : MonoBehaviourPunCallbacks
{
    public float timerValue = 180f;
    public bool isTimerOn = false;
    public TextMeshProUGUI UITimer;

    void Start()
    {
        // Iniciar el temporizador
        StartTimer();
        
        
    }

    private void Update()
    {
        if (photonView.IsMine && isTimerOn)
        {
            InitializeTimer();
        }
    }

    public void StartTimer()
    {
        isTimerOn = true;
    }
    public GameObject tiempoacabado;
    public void StopTimer()
    {
        isTimerOn = false;
        photonView.RPC("ActivatePanel", RpcTarget.All);
    }
    
    public void InitializeTimer()
    {
        if (timerValue > 0)
        {
            timerValue -= Time.deltaTime;
        }
        else
        {
            timerValue = 0;
            StopTimer();
            
        }
        
        ShowTime(timerValue);

        // Sincronizar el valor del temporizador a través de la red
        photonView.RPC("UpdateTimer", RpcTarget.All, timerValue);
    }
    [PunRPC]
    void ActivatePanel()
    {
        tiempoacabado.SetActive(true);
    }
    [PunRPC]
    void UpdateTimer(float newTimerValue)
    {
        timerValue = newTimerValue;
        ShowTime(timerValue);
    }

    void ShowTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }
        else if (timeToDisplay > 0)
        {
            timeToDisplay += 1;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        UITimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
    #region // viejo
    //public void TimerBegins()
    // {
    //     if (!isTimerOn)
    //     {
    //         isTimerOn = true;
    //         timeRemaining = timer; 

    //     }
    // }

    // public void ticks()
    // {

    // }
    #endregion

