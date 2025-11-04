using UnityEngine;
using UnityEngine.SceneManagement;

public class Buraco : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // Recarrega a cena imediatamente (volta ao início da fase)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
