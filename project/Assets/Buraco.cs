using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buraco : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Se marcado, teleporta o player para o Respawn. Se não, reseta a cena.")]
    public bool usarRespawn = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            if (usarRespawn)
            {
                Debug.Log("[Buraco] Player caiu no buraco! Teleportando para o respawn...");
                // Teleporta o player para o respawn
                StartCoroutine(TeleportarPlayerParaRespawn(other.gameObject));
            }
            else
            {
                Debug.Log("[Buraco] Player caiu no buraco! Resetando a cena...");
                // Recarrega a cena imediatamente (volta ao início da fase)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
    
    IEnumerator TeleportarPlayerParaRespawn(GameObject player)
    {
        // Procura por um GameObject com tag "Respawn" na cena
        GameObject pontoRespawn = null;
        
        try
        {
            pontoRespawn = GameObject.FindGameObjectWithTag("Respawn");
        }
        catch (UnityException)
        {
            Debug.LogWarning("[Buraco] Tag 'Respawn' não existe no projeto! Resetando a cena...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield break;
        }
        
        if (pontoRespawn != null)
        {
            Debug.Log($"[Buraco] Ponto de respawn encontrado em: {pontoRespawn.transform.position}");
            
            // Reseta a velocidade do Rigidbody2D primeiro
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            
            // Aguarda um frame para garantir que a física resetou
            yield return new WaitForFixedUpdate();
            
            // Teleporta o player para o ponto de respawn
            player.transform.position = pontoRespawn.transform.position;
            
            Debug.Log($"[Buraco] ✅ Player teleportado para o respawn: {pontoRespawn.transform.position}");
        }
        else
        {
            Debug.LogWarning("[Buraco] ⚠️ Nenhum GameObject com tag 'Respawn' encontrado! Resetando a cena...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
