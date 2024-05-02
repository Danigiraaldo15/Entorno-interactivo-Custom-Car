using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class CamMove_select_PJ : MonoBehaviourPunCallbacks
{
    public GameObject Cam;

    public TextMeshProUGUI nickname;

    public GameObject[] Luces;

    public GameObject RejaShacke;
    public GameObject RejaSubir;

    public iTween.EaseType SmoothCam;

    [Header("EasyTween")]
    public EasyTween Black;
    public EasyTween BannerName;

    [Header("Pos cam")]
    public Transform Pos1;
    public Transform Pos2;
    public Transform Pos3;

    [Header("Imagen Banner")]
    [SerializeField] public RawImage BannerImage;
    [SerializeField] public float _x, _y;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Obtener el nombre del jugador almacenado en PlayerPrefs
        string playerName = PlayerPrefs.GetString("nick");

        // Asignar el nombre del jugador a la casilla de texto
        nickname.text = playerName;

        Black.OpenCloseObjectAnimation();
        StartCoroutine(Cinematica());
    }

    private void Update()
    {
        foreach (GameObject luz in Luces) // Convierte la matris de luces en un GameObject que se puede usar con iTween para animar
        {
            iTween.ShakePosition(luz, new Vector3(0.04f, 0.04f, 0), 0.02f);
        }

        BannerImage.uvRect = new Rect(BannerImage.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, BannerImage.uvRect.size);
    }

    IEnumerator Cinematica() // Esta corrutina controla toda la cinematica inicial de selección de personaje
    {
        yield return new WaitForSeconds(0.5f);

        iTween.MoveTo(Cam, iTween.Hash("position", Pos1, "Time", 5f, "easetype", SmoothCam));

        yield return new WaitForSeconds(5);

        iTween.ShakePosition(RejaShacke, new Vector3(0.05f, 0, 0), 1.5f);

        iTween.MoveTo(Cam, iTween.Hash("position", Pos2, "Time", 5, "easetype", SmoothCam));
        iTween.RotateTo(Cam, iTween.Hash("rotation", Pos2, "Time", 5, "easetype", SmoothCam));

        yield return new WaitForSeconds(5f);

        iTween.MoveTo(Cam, iTween.Hash("position", Pos3, "Time", 5, "easetype", SmoothCam));
        iTween.RotateTo(Cam, iTween.Hash("rotation", Pos3, "Time", 5, "easetype", SmoothCam));

        iTween.MoveTo(RejaSubir, iTween.Hash("position", new Vector3(-5.898218f, 4.03499985f, -17.1325817f), "Time", 3, "easetype", SmoothCam));

        yield return new WaitForSeconds(3);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        BannerName.OpenCloseObjectAnimation();
    }
}
