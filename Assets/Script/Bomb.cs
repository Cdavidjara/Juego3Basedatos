using TMPro;
using UnityEngine;
using Photon.Pun;

public class Bomb : MonoBehaviour
{
    public GameObject explosionEffect;
    public float explosionDelay = 0f;
    private TextMeshProUGUI textoMision;

    void Start()
    {
        GameObject textoM = GameObject.FindWithTag("textomision");
        textoMision = textoM.GetComponent<TextMeshProUGUI>();
        textoMision.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Explode();
            textoMision.text = "Juego Finalizado, presiona X para volver a jugar";

            Logica logica = other.GetComponent<Logica>();
            if (logica != null)
            {
                logica.OnCollisionWithBomb();
            }
            else
            {
                Debug.Log("No se encontró el componente Logica en el jugador.");
            }
        }
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
