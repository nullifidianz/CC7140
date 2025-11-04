using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Configuração de Cena")]
    [Tooltip("Índice da próxima fase no Build Settings (0, 1, 2, etc.)")]
    public int indiceDaProximaFase;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem entrou no portal foi o jogador
        if (other.CompareTag("Player"))
        {
            // Teleporta imediatamente usando o índice da cena
            Debug.Log($"Teleportando para cena {indiceDaProximaFase}");
            SceneManager.LoadScene(indiceDaProximaFase);
        }
    }
}
