using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menumanager : MonoBehaviour
{
    public GameObject camara;

    [Header("Color Carro")]
    public Material ColorCarro;
    public Color[] colorcar;
    [Header("Color Llantas")]
    public Material ColorLlantas;
    public Color[] colorllantas;
    [Header("Color Interior")]
    public Material MaterialInterior;
    public Color[] colorinterior; 
    [Header("Color Vidrio")]
    public Material MaterialVidrio;
    public Slider colorvidrio;
    public void Start()
    {
        MoveCamera(0);
    }
    public void MoveCamera(int menuid)
    {   
        Vector3 pos;
        Vector3 rot;

        switch(menuid)
        {
            case 0:
                pos = new Vector3(21.4626369f, 15.9695492f, -8.35679722f);
                rot = new Vector3(17.2242184f, 20.0423851f, 0);
                iTween.MoveTo(camara, iTween.Hash("position", pos,"time", 5));
                iTween.RotateTo(camara, iTween.Hash("rotation", rot));
                break;

            case 1: 
                pos = new Vector3(21.5937424f, 15.5086412f, -7.73975182f);
                rot = new Vector3(7.42663717f, 51.4976883f, 0);
                iTween.MoveTo(camara, iTween.Hash("position", pos));
                iTween.RotateTo(camara, iTween.Hash("rotation", rot)); 
                break;
            case 2:

                pos = new Vector3(22.0310001f, 15.9757557f, -7.34776783f);
                rot = new Vector3(44.2105522f, 347.211945f, 0);
                iTween.MoveTo(camara, iTween.Hash("position", pos));
                iTween.RotateTo(camara, iTween.Hash("rotation", rot));
                break;
            case 3:
                pos = new Vector3(22.8473682f, 15.7213631f, -7.44189882f);
                rot = new Vector3(21.1777344f, 292.552002f, 0.000253623963f);
                iTween.MoveTo(camara, iTween.Hash("position", pos));
                iTween.RotateTo(camara, iTween.Hash("rotation", rot));
                break;
            case 4:
                pos = new Vector3(21.5784836f, 15.6697798f, -6.94599009f);
                rot = new Vector3(3.81697869f, 66.1762466f, 0.000635336211f);
                iTween.MoveTo(camara, iTween.Hash("position", pos));
                iTween.RotateTo(camara, iTween.Hash("rotation", rot));
                break;


        }
    }
    public void ChangeColocar_Car(int c)
    {
        ColorCarro.color = colorcar[c];
    }public void ChangeLlantas(int LL)
    {
        ColorLlantas.color = colorllantas[LL];
    }
    public void ChangeInterior(int IN)
    {
        MaterialInterior.color = colorinterior[IN];
    }
    public void ChangeVidrio()
    {
        MaterialVidrio.color = new Color(0, 0, 0, colorvidrio.value);
    }
}
