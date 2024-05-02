using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class menu : MonoBehaviour
{
    public GameObject buttonPlay;
    public GameObject buttonSetting;
    public GameObject buttonTutotial;
    public GameObject buttonexit;
    public iTween.EaseType easeType;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        iTween.MoveTo(buttonPlay, iTween.Hash("position", new Vector3(50, 429, 0), "islocal", true, "easeType",easeType));
        iTween.MoveTo(buttonSetting, iTween.Hash("position", new Vector3(50, 129.5f, 0), "islocal", true, "easeType", easeType));
        iTween.MoveTo(buttonTutotial, iTween.Hash("position", new Vector3(50, -129, 0),"islocal", true, "easeType", easeType));
        iTween.MoveTo(buttonexit, iTween.Hash("position", new Vector3(50, -229, 0), "islocal", true, "easeType", easeType));
        iTween.ScaleTo(buttonPlay, iTween.Hash("scale", new Vector3(1.2f, 1.2f, 1.2f), "looptype", "pingpong"));
    }
    public void GoGame()
    {
        SceneManager.LoadScene("jueho");
    }
    public void salir()
    {
        Application.Quit();
    }
}