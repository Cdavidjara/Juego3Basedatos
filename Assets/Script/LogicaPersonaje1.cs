using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class LogicaPersonaje1 : MonoBehaviourPunCallbacks
{
    public float velocidadMovimiento = 5.0f;
    public float velocidadRotacion = 200.0f;
    private Animator anim;
    public float x, y;
    public Rigidbody rb;
    public float fuerzaDeSalto = 8f;
    public bool puedoSaltar;
    public int nivelPersonaje;
    public PhotonView personPrefab;
    public Transform puntoReferencia;
    public float velocidadInicial;
    public float velocidadAgachado;
    public int fuerzaExtra = 0;
    private bool gameOver = false;
    private TextMeshProUGUI gameOverText;
    private GameObject panelObjetivos;
    private TextMeshProUGUI textoMision;

    void Start()
    {
        puedoSaltar = false;
        anim = GetComponent<Animator>();
        velocidadInicial = velocidadMovimiento;
        velocidadAgachado = 0.5f;
        rb = GetComponent<Rigidbody>();
        CameraWork _cameraWork = GetComponent<CameraWork>();

        GameObject textoM = GameObject.FindWithTag("textomision");
        textoMision = textoM.GetComponent<TextMeshProUGUI>();

        if (_cameraWork != null)
        {
            if (photonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("CameraWork component not found or null.");
        }

        if (photonView.IsMine)
        {
            anim.SetFloat("VelX", 0);
            anim.SetFloat("VelY", 0);
            anim.SetBool("tocoSuelo", true);
            anim.SetBool("salte", false);
        }

        panelObjetivos = GameObject.FindWithTag("textomision");
        if (panelObjetivos != null)
        {
            panelObjetivos.SetActive(true);
        }
        else
        {
            Debug.LogError("Panel with tag 'textomision' not found in the scene.");
        }
    }

    void FixedUpdate()
    {
        if (!gameOver && photonView.IsMine)
        {
            transform.Rotate(0, x * Time.deltaTime * velocidadRotacion, 0);
            transform.Translate(0, 0, y * Time.deltaTime * velocidadMovimiento);
        }

        if (gameOver && Input.GetKeyDown(KeyCode.X))
        {
            RestartGame();
        }
    }

    void Update()
    {
        if (!gameOver && photonView.IsMine)
        {
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");

            anim.SetFloat("VelX", x);
            anim.SetFloat("VelY", y);

            if (puedoSaltar)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    anim.SetBool("salte", true);
                    rb.AddForce(new Vector3(0, fuerzaDeSalto, 0), ForceMode.Impulse);
                }
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    anim.SetBool("agachado", true);
                    velocidadMovimiento = velocidadInicial;
                }
                else
                {
                    anim.SetBool("agachado", false);
                    velocidadMovimiento = velocidadInicial;
                }

                anim.SetBool("tocoSuelo", true);
            }
            else
            {
                EstoyCayendo();
            }
        }
    }

    void EstoyCayendo()
    {
        rb.AddForce(fuerzaExtra * Physics.gravity);

        anim.SetBool("tocoSuelo", false);
        anim.SetBool("salte", false);
    }

    public void GameOver()
    {
        gameOver = true;
        textoMision.text = "Juego Finalizado, presiona X para volver a jugar";
        anim.SetFloat("VelX", 0);
        anim.SetFloat("VelY", 0);
        anim.SetBool("tocoSuelo", false);
        anim.SetBool("salte", false);
        anim.SetBool("agachado", false);

        if (panelObjetivos != null)
        {
            panelObjetivos.SetActive(true);
        }
    }

    void RestartGame()
    {
        gameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateSphereCount(int numDeObjetivos)
    {
        textoMision.text = "Recoje las esferas\nTe quedan: " + numDeObjetivos;
    }
}
