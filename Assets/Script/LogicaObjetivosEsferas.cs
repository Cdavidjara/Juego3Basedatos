using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Logica : MonoBehaviour
{
    public int numDeObjetivos; // Número de esferas restantes
    public int cantidadInicialDeEsferas; // Cantidad inicial de esferas
    public TextMeshProUGUI textoMision;
    public GameObject botonDeMision;

    public LogicaPersonaje1 logicaPersonaje1;

    void Start()
    {
        cantidadInicialDeEsferas = GameObject.FindGameObjectsWithTag("objetivo").Length;
        numDeObjetivos = cantidadInicialDeEsferas;
        textoMision.text = "Recoje las esferas" +
            "\n Te quedan:" + numDeObjetivos;
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "objetivo")
        {
            Destroy(col.transform.parent.gameObject);
            numDeObjetivos--;
            textoMision.text = "Recoje las esferas" +
            "\n Te quedan:" + numDeObjetivos;
            if (numDeObjetivos <= 0)
            {
                logicaPersonaje1.nivelPersonaje++;
                textoMision.text = "Completaste la mision";
                botonDeMision.SetActive(true);

                // Enviar la cantidad de esferas recolectadas al servidor
                int esferasRecolectadas = cantidadInicialDeEsferas - numDeObjetivos;
                StartCoroutine(PullScore(esferasRecolectadas));
            }
        }
    }

    IEnumerator PullScore(int cantidad)
    {
        string url = "http://localhost/juego3d/juego.php";
        WWWForm form = new WWWForm();
        form.AddField("cantidad", cantidad);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al enviar datos al servidor: " + www.error);
            }
            else
            {
                Debug.Log("Datos enviados correctamente al servidor.");
            }
        }
    }
}
