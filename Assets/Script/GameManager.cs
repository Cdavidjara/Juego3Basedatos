using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI gameOverText; // Referencia al texto de Game Over

    private void Start()
    {
        // Asegúrate de que el texto de Game Over esté desactivado al inicio
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && gameOverText.gameObject.activeSelf)
        {
            RestartGame();
        }
    }

    private void RestartGame()
    {
        // Reiniciar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
