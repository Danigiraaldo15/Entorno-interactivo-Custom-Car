using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviourPunCallbacks
{
    private List<GameObject> previousHits = new List<GameObject>();
    public CharacterController characterController;
    public Animator animator;

    [Header("GAMEOBJECTS / TRANSFORMS")]
    public GameObject camara;
    public GameObject ui_Game_Script;
    public GameObject TimeCowntDown_Play;
    public GameObject TimeCowntDown_Pausa;
    public GameObject alphaJoints;
    public GameObject alphaSurface;
    public GameObject Inst_Particulas;
    public GameObject TeclaE;
    public GameObject CanvasTeclaE;
    public GameObject powerprefab;
    public GameObject powerprefab2;
    public GameObject prefablentitud;
    public GameObject modelopower;
    public GameObject modelopower2;
    public GameObject modelolentitud;
    public Transform powerspawn;
    
    [Header("FLOATS")]
    public float MoveSpeed = 5f;
    public float rotationspeed = 2f;
    public float fuerzalanzamiento = 50;
    public float gravity = -9;
    public float invertir = 1;
    public float imanradio;
    public float imanvelocidad;

    [Header("INT")]
    public int contador = 0;
    public int contador_SAFE;

    [Header("BOOLS")]
    public bool desmayo = false;
    public bool powerupactivo = false;
    public bool borracho = false;
    public bool iman = false;
    public bool lanzado = false;
    public bool destruido = false;

    [Header("TEXTOS")]
    public TextMeshPro PlayerName;
    public TextMeshProUGUI Puntos;
    public TextMeshProUGUI PuntosGuardados;

    [Header("MATERIALES")]
    public Material InvisibleWall;
    public Material OriginalWall;

    [Header("SONIDOS")]
    private AudioSource TomarMoneda;

    [Header("PARTICULAS")]
    public GameObject Velocidad, Lentitud, Borracho, Invisibiladad, Desmayo, PuntosMenos, x2, IMAN;
    powercollision powers;
    private void Start()
    {
        
        powers = GetComponent<powercollision>();
        animator = GetComponent<Animator>();
        camara = GameObject.Find("Main Camera");
        alphaSurface = GameObject.Find("Mesh");
        alphaJoints = GameObject.Find("Mesh");
        
        if (photonView.IsMine) //Jugador Local
        {
            ui_Game_Script = GameObject.Find("Game Manager");
            camara.GetComponent<CameraFollow>().target = gameObject.transform;
            camara.GetComponent<CameraFollow>().StartFollow();
            camara.SetActive(true);
            photonView.Owner.NickName = PlayerPrefs.GetString("nick");
            Puntos = GameObject.Find("Puntos").GetComponent<TextMeshProUGUI>();
            PuntosGuardados = GameObject.Find("Puntos Guardados").GetComponent<TextMeshProUGUI>();
            TomarMoneda = GameObject.Find("MonedasSonido").GetComponent<AudioSource>();
            Inst_Particulas = GameObject.Find("Particulas");
        }
        else
        {
            PlayerName.text = photonView.Owner.NickName;
        }
        
    }
    
    

    private void Update()
    {
        Movimiento();
        RayCastCam();
        PlayerName.transform.parent.transform.LookAt(camara.transform.position);
        TeclaE.transform.parent.transform.LookAt(camara.transform.position);
        moneda[] coins = FindObjectsOfType<moneda>();
        if (coins.Length == 0 )
        {
            panelmonedas();
        }
    }
    [PunRPC]
    public void panelmonedas()
    {
        GameObject.Find("UI").transform.GetChild(3).gameObject.SetActive(true);
    }
    public void Desmayar()
    {
        desmayo = true;
    }


    public void Animo()
    {
        desmayo = false;
    }

    void RayCastCam() // Aquí se establece el raycast que sera lanzado para identificar los objetos en la escena que se pueden hacer invisibles para que no estorben al jugador (Muros)
    {
        if(photonView.IsMine)
        {
            Vector3 direction = transform.position - camara.transform.position;
            Ray ray = new Ray(camara.transform.position, direction.normalized);
            RaycastHit[] hits = Physics.RaycastAll(ray, direction.magnitude);

            // Iterar sobre los objetos impactados por el raycast
            foreach (RaycastHit hit in hits)
            {
                // Verificar si el objeto impactado tiene el tag que queremos ignorar
                if (hit.collider.CompareTag("Muros"))
                {
                    // Obtener el renderer del objeto impactado
                    Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();

                    // Si se encontró un renderer, cambiar su material
                    if (renderer != null)
                    {
                        renderer.material = InvisibleWall;
                        previousHits.Add(hit.collider.gameObject); // Agregar el objeto a la lista de objetos impactados
                    }
                }
            }

            // Iterar sobre los objetos impactados en el fotograma anterior
            foreach (GameObject obj in previousHits)
            {
                // Verificar si el objeto sigue siendo impactado por el raycast en el fotograma actual
                bool isStillHit = false;
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject == obj)
                    {
                        isStillHit = true;
                        break;
                    }
                }

                // Si el objeto ya no está siendo impactado, cambiar su material a OriginalWall
                if (!isStillHit)
                {
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = OriginalWall;
                    }
                }
            }
        }
    }

    void Movimiento() // Movimiento del personaje segun ciertos estados, como pausa o powerUp borracho.
    {
        if (photonView.IsMine && ui_Game_Script.GetComponent<UI_Game>().CanMove == true)
        {

            if (!desmayo)
            {
                float h = Input.GetAxis("Horizontal") * invertir;
                float v = Input.GetAxis("Vertical") * invertir;

                Vector3 movementDirection = new Vector3(h, 0, v);
                movementDirection.Normalize();

                // Aplica la velocidad de movimiento
                Vector3 movement = movementDirection * MoveSpeed * Time.deltaTime;

                // Rota al personaje hacia la dirección de movimiento si hay movimiento
                if (movementDirection != Vector3.zero)
                {
                    transform.forward = movementDirection;
                }

                // Aplica la gravedad al movimiento en el eje y
                movement.y += gravity * Time.deltaTime;

                // Mueve al personaje usando CharacterController.Move()
                characterController.Move(movement);

                // Controla las animaciones de caminar
                if (v != 0 || h != 0)
                {
                    animator.SetBool("camina", true);

                    if ((v != 0 || h != 0) && borracho == true)
                    {
                        animator.SetBool("Desmayo", false);
                        animator.SetBool("Borracho", true);
                    }
                }
                else
                {
                    animator.SetBool("camina", false);
                    animator.SetBool("Desmayo", false);

                    if ((v == 0 || h == 0) && borracho == true)
                    {
                        animator.SetBool("Desmayo", true);
                    }
                }
            }
        }
        else if(photonView.IsMine && ui_Game_Script.GetComponent<UI_Game>().CanMove == false)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                animator.SetBool("camina", false);
            }
        }
    }

    //inicio del power up de invisibilidad
    [PunRPC] //se hace que el power up sea para todos
    public void MakeInvisible(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);
        if (pv != null && pv.IsMine)
        {
            StartCoroutine(InvisibilidadRoutine(pv));
        }
    }

    IEnumerator InvisibilidadRoutine(PhotonView pv)
    {
        string alphaSurfaceName = "Mesh";
        string alphaJointsName = "Mesh";

        pv.RPC("SetVisibility", RpcTarget.AllBuffered, false, alphaSurfaceName, alphaJointsName);
        yield return new WaitForSeconds(10);
        // Restauras la visibilidad del jugador correspondiente
        pv.RPC("SetVisibility", RpcTarget.AllBuffered, true, alphaSurfaceName, alphaJointsName);
    }

    [PunRPC]
    public void SetVisibility(bool isVisible, string alphaSurfaceName, string alphaJointsName)
    {
        GameObject alphaSurface = transform.Find(alphaSurfaceName).gameObject;
        GameObject alphaJoints = transform.Find(alphaJointsName).gameObject;

        if (alphaSurface != null && alphaJoints != null)
        {
            alphaSurface.SetActive(isVisible);
            alphaJoints.SetActive(isVisible);
        }
    }
     //finalizacion del power up de invisibilidad
    private void OnTriggerEnter(Collider other)
    {
        #region //Sumar Puntos
        if (other.gameObject.CompareTag("puntos") && photonView.IsMine) // Toma los puntos y se los suma al jugador correspondiente.
        {
            iTween.ShakePosition(camara, iTween.Hash("amount", new Vector3(0.1f, 0.1f, 0.1f), "time", 0.02f));
            TomarMoneda.Play();
            contador += powerupactivo ? 20 : 10;
            Puntos.text = contador.ToString();
            StartCoroutine(EfectoSumarMonedas());
            PhotonNetwork.Instantiate("TomarCoins-Particulas", Inst_Particulas.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }
        #endregion

        #region //Activar Particulas

        if (photonView.IsMine)
        {
            if (other.gameObject.CompareTag("Particula_X2"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    x2.SetActive(true);
                    ui_Game_Script.GetComponent<UI_Game>().X2.OpenCloseObjectAnimation();

                    yield return new WaitForSeconds(10);

                    x2.SetActive(false);
                    ui_Game_Script.GetComponent<UI_Game>().X2.OpenCloseObjectAnimation();
                }
            }

            if (other.gameObject.CompareTag("Particula_Velocidad"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    Velocidad.SetActive(true);
                    ui_Game_Script.GetComponent<UI_Game>().Velocidad.OpenCloseObjectAnimation();

                    yield return new WaitForSeconds(6.5f);

                    Velocidad.SetActive(false);
                    ui_Game_Script.GetComponent<UI_Game>().Velocidad.OpenCloseObjectAnimation();
                }
            }

            if (other.gameObject.CompareTag("Particula_Invisibilidad"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    Invisibiladad.SetActive(true);
                    ui_Game_Script.GetComponent<UI_Game>().Invisibilidad.OpenCloseObjectAnimation();

                    yield return new WaitForSeconds(10);

                    Invisibiladad.SetActive(false);
                    ui_Game_Script.GetComponent<UI_Game>().Invisibilidad.OpenCloseObjectAnimation();
                }
            }

            if (other.gameObject.CompareTag("Particula_Iman"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    IMAN.SetActive(true);
                    ui_Game_Script.GetComponent<UI_Game>().Iman.OpenCloseObjectAnimation();

                    yield return new WaitForSeconds(10);

                    IMAN.SetActive(false);
                    ui_Game_Script.GetComponent<UI_Game>().Iman.OpenCloseObjectAnimation();
                }
            }

            if (other.gameObject.CompareTag("Particula_Menos"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    PuntosMenos.SetActive(true);

                    yield return new WaitForSeconds(10);

                    PuntosMenos.SetActive(false);
                }
            }

            if (other.gameObject.CompareTag("Particula_Lentitud"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    Lentitud.SetActive(true);
                    ui_Game_Script.GetComponent<UI_Game>().Lentitud.OpenCloseObjectAnimation();

                    yield return new WaitForSeconds(5);

                    Lentitud.SetActive(false);
                    ui_Game_Script.GetComponent<UI_Game>().Lentitud.OpenCloseObjectAnimation();
                }
            }

            if (other.gameObject.CompareTag("Particula_Borracho"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    Borracho.SetActive(true);
                    ui_Game_Script.GetComponent<UI_Game>().Borracho.OpenCloseObjectAnimation();

                    yield return new WaitForSeconds(10);

                    Borracho.SetActive(false);
                    ui_Game_Script.GetComponent<UI_Game>().Borracho.OpenCloseObjectAnimation();
                }
            }

            if (other.gameObject.CompareTag("Particula_Desmayo"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    Desmayo.SetActive(true);
                    ui_Game_Script.GetComponent<UI_Game>().Desmayo.OpenCloseObjectAnimation();

                    yield return new WaitForSeconds(5f);

                    ui_Game_Script.GetComponent<UI_Game>().Desmayo.OpenCloseObjectAnimation();
                    Desmayo.SetActive(false);
                }
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Particula_X2"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    x2.SetActive(true);

                    yield return new WaitForSeconds(11);

                    x2.SetActive(false);
                }
            }

            if (other.gameObject.CompareTag("Particula_Velocidad"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    Velocidad.SetActive(true);

                    yield return new WaitForSeconds(6.5f);

                    Velocidad.SetActive(false);
                }
            }

            if (other.gameObject.CompareTag("Particula_Invisibilidad"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    Invisibiladad.SetActive(true);

                    yield return new WaitForSeconds(11);

                    Invisibiladad.SetActive(false);
                }
            }

            if (other.gameObject.CompareTag("Particula_Iman"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    IMAN.SetActive(true);

                    yield return new WaitForSeconds(11);

                    IMAN.SetActive(false);
                }
            }

            if (other.gameObject.CompareTag("Particula_Menos"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    PuntosMenos.SetActive(true);

                    yield return new WaitForSeconds(11);

                    PuntosMenos.SetActive(false);
                }
            }

            if (other.gameObject.CompareTag("Particula_Lentitud"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    Lentitud.SetActive(true);

                    yield return new WaitForSeconds(6.5f);

                    Lentitud.SetActive(false);
                }
            }

            if (other.gameObject.CompareTag("Particula_Borracho"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    Borracho.SetActive(true);

                    yield return new WaitForSeconds(11);

                    Borracho.SetActive(false);
                }
            }

            if (other.gameObject.CompareTag("Particula_Desmayo"))
            {
                StartCoroutine(activar());

                IEnumerator activar()
                {
                    Desmayo.gameObject.SetActive(true);

                    yield return new WaitForSeconds(7f);

                    Desmayo.gameObject.SetActive(false);
                }
            }
        }

        #endregion
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SafePoint") && photonView.IsMine)
        {
            TeclaE.SetActive(false);
            CanvasTeclaE.SetActive(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(contador >= 1)
        {
            IEnumerator AsegurarMonedas()
            {
                yield return new WaitForSeconds(5);

                StartCoroutine(EfectoAsegurarMonedas());
                contador_SAFE += contador;
                PuntosGuardados.text = contador_SAFE.ToString();
                contador = 0;
                Puntos.text = contador.ToString();
                CanvasTeclaE.SetActive(true);
                ui_Game_Script.GetComponent<UI_Game>().AsegurandoMonedas = false;
            }

            if (other.gameObject.CompareTag("SafePoint") && photonView.IsMine)
            {
                TeclaE.SetActive(true);
                TeclaE.GetComponent<Image>().color = Color.white;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    CanvasTeclaE.SetActive(false);
                    animator.SetBool("camina", false);
                    ui_Game_Script.GetComponent<UI_Game>().AsegurandoMonedas = true;
                    iTween.ShakePosition(camara, iTween.Hash("amount", new Vector3(0.05f, 0.05f, 0.05f), "time", 5f, "islocal", true));

                    StartCoroutine(AsegurarMonedas());
                }
                else
                {

                }
            }
        }
        else if (contador <= 0)
        {
            if (other.gameObject.CompareTag("SafePoint") && photonView.IsMine)
            {
                TeclaE.SetActive(true);
                TeclaE.GetComponent<Image>().color = Color.grey;
            }
        }
    }

    #region //Efectos Textos

    IEnumerator EfectoSumarMonedas()
    {
        // Aumentar el tamaño del texto y cambiar el color
        Puntos.color = Color.green;

        float startTime = Time.time;
        while (Time.time < startTime + 0.1f)
        {
            float t = (Time.time - startTime) / 0.2f;
            Puntos.fontSize = Mathf.Lerp(60, 100, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        while (Time.time < startTime + 0.1f)
        {
            float t = (Time.time - startTime) / 0.2f;
            Puntos.fontSize = Mathf.Lerp(100, 60, t);
            yield return null;
        }

        //Disminuir el tamaño del texto y restaurar el color original
        Puntos.fontSize = 60;
        Puntos.color = Color.white;
    }

    IEnumerator EfectoAsegurarMonedas()
    {
        // Aumentar el tamaño del texto y cambiar el color
        Puntos.color = Color.cyan;
             
        float startTime = Time.time;
        while (Time.time < startTime + 0.1f)
        {
            float t = (Time.time - startTime) / 0.2f;
            Puntos.fontSize = Mathf.Lerp(60, 100, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        while (Time.time < startTime + 0.1f)
        {
            float t = (Time.time - startTime) / 0.2f;
            Puntos.fontSize = Mathf.Lerp(100, 60, t);
            yield return null;
        }

        //Disminuir el tamaño del texto y restaurar el color original
        Puntos.fontSize = 60;
        Puntos.color = Color.white;
    }

    public IEnumerator EfectoPerderMonedas()
    {
        // Aumentar el tamaño del texto y cambiar el color
        Puntos.color = Color.red;

        float startTime = Time.time;
        while (Time.time < startTime + 0.1f)
        {
            float t = (Time.time - startTime) / 0.2f;
            Puntos.fontSize = Mathf.Lerp(60, 100, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        while (Time.time < startTime + 0.1f)
        {
            float t = (Time.time - startTime) / 0.2f;
            Puntos.fontSize = Mathf.Lerp(100, 60, t);
            yield return null;
        }

        // Disminuir el tamaño del texto y restaurar el color original
        Puntos.fontSize = 60;
        Puntos.color = Color.white;
    }

    #endregion
    //inicio del power up borracho
    [PunRPC]
    public void SetBorrachoState(bool state, int playerId)
    {
        // Obtiene el PhotonView del jugador
        PhotonView photonView = PhotonView.Find(playerId);

        // Comprueba si el PhotonView existe
        if (photonView != null)
        {
            // Llama a la función Borracho
            photonView.RPC("ApplyBorracho", RpcTarget.All, state);
        }
    }

    [PunRPC]
    public void ApplyBorracho(bool state)
    {
        StartCoroutine(vorracho(state));
    }
    IEnumerator vorracho(bool state)
    {
        invertir = state ? -1 : 1;
        borracho = true;
        animator.SetBool("Borracho", true);
        yield return new WaitForSeconds(10); // duration es la duración del efecto
        borracho = false;
        animator.SetBool("Borracho", false);
        animator.SetBool("camina", true);
        invertir = 1;
    }
    //Finalizacion del power up borracho

    // Llamada para activar el power-up. Esta función se llama en el cliente.
    public void ActivarIman()
    {
        photonView.RPC("ImanRPC", RpcTarget.All);
    }

    // RPC para iniciar la corrutina en todos los clientes.
    [PunRPC]
    public void ImanRPC()
    {
        StartCoroutine(Iman());
    }

    // Corrutina para el comportamiento del imán.
    public IEnumerator Iman()
    {
        float startTime = Time.time;
        while (Time.time - startTime < imanradio)
        {
            Collider[] puntosrango = Physics.OverlapSphere(transform.position, imanradio);
            moneda[] monedasEnEscena = FindObjectsOfType<moneda>();
            foreach (Collider puntos in puntosrango)
            {
                if (puntos.gameObject.CompareTag("puntos"))
                {
                    
                    foreach (var moneda in monedasEnEscena)
                    {
                        moneda.MoveTowardsPlayer(transform.position, 0.5f);
                    }
                    

                }
            }
            yield return null;
        }
    }
    // inicio del power up desmayo
    public void desmaio()
    {

        if (Input.GetButton("Fire1"))
        {
            GameObject nuevopower = PhotonNetwork.Instantiate(powerprefab.name, powerspawn.position, powerspawn.rotation);
            
            Rigidbody rb = nuevopower.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                float fuerza = 10.0f;
                GameObject player = GameObject.FindWithTag("Player");
                Vector3 direccionadelante = player.transform.forward;
                Vector3 force = new Vector3(direccionadelante.x, 0, direccionadelante.z) * fuerza;
                rb.AddForce(force, ForceMode.Impulse);
                StartCoroutine(caida(rb, 0.5f));
                
                destruido = true;
                Destroy(nuevopower, 8);
                

            }
        }

    } 
    public IEnumerator desmatos(Collider playerCollider)
    {
        modelopower.SetActive(true);
        yield return new WaitUntil(() => Input.GetButton("Fire1"));
        desmaio();

        
        PhotonNetwork.RaiseEvent(1, true, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        modelopower.SetActive(false);
        yield return new WaitForSeconds(2);
        
        PhotonNetwork.RaiseEvent(1, false, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable); 
        

    }

    public void puntosMenos()
    {
        if (Input.GetButton("Fire1"))
        {
            GameObject nuevopower = PhotonNetwork.Instantiate(powerprefab2.name, powerspawn.position, powerspawn.rotation);
            
            Rigidbody rb = nuevopower.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                float fuerza = 10.0f;
                GameObject player = GameObject.FindWithTag("Player");
                Vector3 direccionadelante = player.transform.forward;
                Vector3 force = new Vector3(direccionadelante.x, 0, direccionadelante.z) * fuerza;
                rb.AddForce(force, ForceMode.Impulse);
                StartCoroutine(caida(rb, 0.5f));
                
                Destroy(nuevopower, 5);
                destruido = true;
                Debug.Log("Objeto lanzado con éxito.");
            }
        }
    }
    public IEnumerator puntosmenos(Collider playercollider)
    {
        modelopower2.SetActive(true);
        yield return new WaitUntil(() => Input.GetButton("Fire1"));
        puntosMenos();
        PhotonNetwork.RaiseEvent(1, true, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        modelopower2.SetActive(false);
        yield return new WaitForSeconds(2);
        PhotonNetwork.RaiseEvent(1, false, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable); ;
    }

    IEnumerator caida(Rigidbody rb, float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        rb.useGravity = true;
    }
    
    public void lentitud()
    {
        if (Input.GetButton("Fire1"))
        {
            GameObject nuevopower = PhotonNetwork.Instantiate(prefablentitud.name, powerspawn.position, powerspawn.rotation);
            
            Rigidbody rb = nuevopower.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                float fuerza = 10.0f;
                GameObject player = GameObject.FindWithTag("Player");
                Vector3 direccionadelante = player.transform.forward;
                Vector3 force = new Vector3(direccionadelante.x, 0, direccionadelante.z) * fuerza;
                rb.AddForce(force, ForceMode.Impulse);
                StartCoroutine(caida(rb, 0.5f));
                
                destruido=true;

                Destroy(nuevopower, 8);
                
                    
                
                Debug.Log("Objeto lanzado con éxito.");
            }
        }
    }
    public IEnumerator lentitud(Collider playercollider)
    {
        modelolentitud.SetActive(true);
        yield return new WaitUntil(() => Input.GetButton("Fire1"));
        lentitud();
        PhotonNetwork.RaiseEvent(1, true, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        modelolentitud.SetActive(false);
        yield return new WaitForSeconds(2);
        PhotonNetwork.RaiseEvent(1, false, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable); ;
    }
    
   //se actualiza la variable lanzado para todos los jugadores presentes
    protected virtual void OnEnable()
    {

        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    protected virtual void OnDisable()
    {

        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    private void NetworkingClient_EventReceived(EventData obj)
    {
        if (obj.Code == 1)
        {
            lanzado = (bool)obj.CustomData;
            Debug.Log("Valor de lanzado actualizado: " + lanzado);
        }
    }
    [PunRPC]
    public void ActualizarLanzado(bool nuevoValor)
    {
        lanzado = nuevoValor;
        Debug.Log("Valor de lanzado actualizado: " + lanzado);

    }

    
}


