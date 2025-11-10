using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Configuração de Cena")]
    [Tooltip("Índice da próxima fase no Build Settings (0, 1, 2, etc.)")]
    public int indiceDaProximaFase;
    
    [Header("Configuração de Progressão")]
    [Tooltip("Número da fase atual (1, 2, 3...). Usado para liberar a próxima fase")]
    public int numeroFaseAtual = 1;
    
    [Tooltip("Se marcado, libera a próxima fase quando passar pelo portal")]
    public bool liberarProximaFase = true;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem entrou no portal foi o jogador
        if (other.CompareTag("Player"))
        {
            // Libera a próxima fase antes de teleportar
            if (liberarProximaFase)
            {
                LevelManager.CompletarFaseELiberarProxima(numeroFaseAtual);
            }
            
            // Teleporta imediatamente usando o índice da cena
            Debug.Log($"Teleportando para cena {indiceDaProximaFase}");
            SceneManager.LoadScene(indiceDaProximaFase);
        }
    }
}
