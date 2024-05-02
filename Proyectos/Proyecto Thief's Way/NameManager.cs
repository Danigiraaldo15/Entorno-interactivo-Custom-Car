using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField NickNamePlayer;
    [SerializeField] private TMP_Text ButtonText;
    public string A_Escena;
    public static string PlayerNickname;
    public GameObject VerifNick;
    public GameObject BlackgroundOut;
    public AudioSource BotonPlay;

    [Header("UiTween")]
    public EasyTween FadeOut;
    public EasyTween InputName;
    public EasyTween BotonInicio;
    public EasyTween StartImagen;

    [Header("Imagen NameMove")]
    [SerializeField] private RawImage NameImage;
    [SerializeField] private float x, y;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            ConnectToServer();
        }

        NameImage.uvRect = new Rect(NameImage.uvRect.position + new Vector2(x, y) * Time.deltaTime, NameImage.uvRect.size);
    }

    public void ConnectToServer()
    {
        if (NickNamePlayer.text != "" && NickNamePlayer.text.Length >= 4)
        {
            BotonPlay.Play();
            PlayerPrefs.SetString("nick", NickNamePlayer.text);
            PlayerPrefs.Save();
            PlayerNickname = NickNamePlayer.text;
            
            ButtonText.text = "INICIANDO....";
            

            SceneManager.LoadScene(A_Escena);
        }
        else
        {
            VerifNick.SetActive(true);
        }
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(1f);

        StartImagen.OpenCloseObjectAnimation();

        yield return new WaitForSeconds(1f);

        InputName.OpenCloseObjectAnimation();

        yield return new WaitForSeconds(0.5f);

        BotonInicio.OpenCloseObjectAnimation();
    }
}
