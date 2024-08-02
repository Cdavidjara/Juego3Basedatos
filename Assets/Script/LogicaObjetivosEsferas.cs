using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.Networking;

public class Logica : MonoBehaviourPunCallbacks
{
    public int numDeObjetivos; // Número de esferas restantes
    public int cantidadInicialDeEsferas; // Cantidad inicial de esferas que se deben recoger
    private TextMeshProUGUI textoMision;
    private GameObject botonDeMision;
    private TextMeshProUGUI textoTemporizador; // Referencia al texto del temporizador

    private int esferasRecolectadas; // Variable para almacenar el número de esferas recolectadas
    private bool gameOver = false; // Estado del juego
    public float tiempoTotal = 60f; // Tiempo total en segundos
    private float tiempoRestante; // Tiempo restante
    private bool temporizadorActivo = true; // Estado del temporizador

    void Start()
    {
        // Configurar el texto de la misión, botón y temporizador
        GameObject textoM = GameObject.FindWithTag("textomision");
        GameObject boton = GameObject.FindWithTag("boton");
        GameObject textoT = GameObject.FindWithTag("textoTemporizador");

        textoMision = textoM.GetComponent<TextMeshProUGUI>();
        botonDeMision = boton;
        textoTemporizador = textoT.GetComponent<TextMeshProUGUI>();

        // Establecer la cantidad inicial de esferas a 3
        cantidadInicialDeEsferas = 3;
        numDeObjetivos = cantidadInicialDeEsferas;
        esferasRecolectadas = 0; // Inicializar contador de esferas recolectadas

        // Inicializar el temporizador
        tiempoRestante = tiempoTotal;
        ActualizarTemporizadorUI();

        textoMision.text = "Recoje las esferas\nTe quedan: " + numDeObjetivos;
    }

    void Update()
    {
        if (temporizadorActivo)
        {
            // Disminuir el tiempo restante
            tiempoRestante -= Time.deltaTime;

            // Asegurarse de que el tiempo no sea negativo
            if (tiempoRestante <= 0)
            {
                tiempoRestante = 0;
                TemporizadorFinalizado();
            }

            // Actualizar el texto del temporizador
            ActualizarTemporizadorUI();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("objetivo"))
        {
            if (photonView.IsMine && !gameOver) // Asegúrate de que el juego no esté terminado
            {
                // Obtener el PhotonView del objeto colisionado
                PhotonView targetPhotonView = col.gameObject.GetComponent<PhotonView>();

                if (targetPhotonView != null)
                {
                    // Llamar al método RPC para destruir la esfera
                    photonView.RPC("DestroySphere", RpcTarget.AllBuffered, targetPhotonView.ViewID);

                    numDeObjetivos--;
                    esferasRecolectadas++; // Incrementar el contador de esferas recolectadas
                    textoMision.text = "Recoje las esferas\nTe quedan: " + numDeObjetivos;

                    // Verificar si se han recogido todas las esferas
                    if (numDeObjetivos <= 0)
                    {
                        textoMision.text = "Completaste la misión";
                        botonDeMision.SetActive(true);
                        gameOver = true; // Marcar el juego como terminado
                        temporizadorActivo = false; // Detener el temporizador

                        // Enviar la cantidad de esferas recolectadas al servidor
                        StartCoroutine(PullScore(esferasRecolectadas, 0)); // Inicialmente, no hay bombas chocadas
                    }
                }
            }
        }
    }

    [PunRPC]
    void DestroySphere(int viewID)
    {
        PhotonView photonView = PhotonView.Find(viewID);
        if (photonView != null)
        {
            PhotonNetwork.Destroy(photonView.gameObject);
        }
    }

    void ActualizarTemporizadorUI()
    {
        if (tiempoRestante > 0)
        {
            // Formatear el tiempo restante como minutos y segundos
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            textoTemporizador.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
        else
        {
            textoTemporizador.text = "¡Tiempo agotado!";
        }
    }

    void TemporizadorFinalizado()
    {
        // Aquí puedes definir lo que sucede cuando el tiempo se agota
        Debug.Log("¡El tiempo se ha agotado!");
        textoMision.text = "¡Tiempo agotado!";
        botonDeMision.SetActive(true);
        temporizadorActivo = false; // Detener el temporizador
        gameOver = true; // Marcar el juego como terminado
    }

    public void OnCollisionWithBomb()
    {
        if (!gameOver) // Asegúrate de que el juego no esté terminado
        {
            // Enviar datos al servidor sobre las esferas recolectadas y la colisión con la bomba
            StartCoroutine(PullScore(esferasRecolectadas, 1)); // Actualizar con bombas chocadas
            gameOver = true; // Marcar el juego como terminado
            temporizadorActivo = false; // Detener el temporizador
        }
    }

    IEnumerator PullScore(int cantidadEsferas, int cantidadBombas)
    {
        string url = "http://localhost/juego3d/juego.php";
        WWWForm form = new WWWForm();
        form.AddField("cantidad", cantidadEsferas);
        form.AddField("chocadaConBomba", cantidadBombas);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al enviar datos al servidor: " + www.error);
            }
            else
            {
                Debug.Log($"Datos enviados: Cantidad Esferas = {cantidadEsferas}, Bombas Chocadas = {cantidadBombas}");
            }
        }
    }
}
