using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampolim : MonoBehaviour
{
    [Header("Configurações do Trampolim")]
    [Tooltip("Força do impulso para cima")]
    public float forcaImpulso = 20f;
    
    [Tooltip("Direção do impulso (padrão: para cima)")]
    public Vector2 direcaoImpulso = Vector2.up;
    
    [Tooltip("Normalizar a direção automaticamente")]
    public bool normalizarDirecao = true;
    
    [Header("Animação")]
    [Tooltip("Animar o trampolim ao ser usado")]
    public bool animarAoUsar = true;
    
    [Tooltip("Escala de compressão quando ativado")]
    [Range(0.5f, 1f)]
    public float escalaCompressao = 0.8f;
    
    [Tooltip("Velocidade da animação")]
    public float velocidadeAnimacao = 0.1f;
    
    [Header("Audio")]
    [Tooltip("Tocar som ao ser usado")]
    public bool tocarSom = false;
    
    public AudioClip somTrampolim;
    
    [Header("Debug")]
    [Tooltip("Mostrar mensagens de debug no Console")]
    public bool mostrarDebug = false;
    
    private Vector3 escalaOriginal;
    private AudioSource audioSource;
    private bool animando = false;

    void Start()
    {
        // Salva a escala original
        escalaOriginal = transform.localScale;
        
        // Configura o AudioSource se necessário
        if (tocarSom)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null && somTrampolim != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = somTrampolim;
                audioSource.playOnAwake = false;
            }
        }
        
        if (mostrarDebug)
        {
            Debug.Log($"[Trampolim] Iniciado! Força: {forcaImpulso}, Direção: {direcaoImpulso}");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o objeto que colidiu tem a tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            AtivarTrampolim(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            AtivarTrampolim(other.gameObject);
        }
    }

    void AtivarTrampolim(GameObject player)
    {
        // Obtém o Rigidbody2D do player
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        
        if (playerRb != null)
        {
            // Normaliza a direção se configurado
            Vector2 direcao = normalizarDirecao ? direcaoImpulso.normalized : direcaoImpulso;
            
            // Reseta a velocidade vertical antes de aplicar o impulso (opcional)
            // Isso garante que o impulso seja consistente
            playerRb.velocity = new Vector2(playerRb.velocity.x, 0f);
            
            // Aplica o impulso
            playerRb.AddForce(direcao * forcaImpulso, ForceMode2D.Impulse);
            
            if (mostrarDebug)
            {
                Debug.Log($"[Trampolim] Impulso aplicado! Direção: {direcao}, Força: {forcaImpulso}");
            }
            
            // Toca o som
            if (tocarSom && audioSource != null && somTrampolim != null)
            {
                audioSource.PlayOneShot(somTrampolim);
            }
            
            // Anima o trampolim
            if (animarAoUsar && !animando)
            {
                StartCoroutine(AnimarTrampolim());
            }
        }
        else if (mostrarDebug)
        {
            Debug.LogWarning("[Trampolim] Player não tem Rigidbody2D!");
        }
    }

    IEnumerator AnimarTrampolim()
    {
        animando = true;
        
        // Comprimir
        float tempo = 0f;
        Vector3 escalaInicial = transform.localScale;
        Vector3 escalaComprimida = new Vector3(
            escalaOriginal.x * escalaCompressao, 
            escalaOriginal.y * escalaCompressao, 
            escalaOriginal.z
        );
        
        while (tempo < velocidadeAnimacao)
        {
            tempo += Time.deltaTime;
            float t = tempo / velocidadeAnimacao;
            transform.localScale = Vector3.Lerp(escalaInicial, escalaComprimida, t);
            yield return null;
        }
        
        // Expandir de volta
        tempo = 0f;
        while (tempo < velocidadeAnimacao)
        {
            tempo += Time.deltaTime;
            float t = tempo / velocidadeAnimacao;
            transform.localScale = Vector3.Lerp(escalaComprimida, escalaOriginal, t);
            yield return null;
        }
        
        transform.localScale = escalaOriginal;
        animando = false;
    }

    // Desenha a direção do impulso no editor
    private void OnDrawGizmosSelected()
    {
        // Desenha uma seta mostrando a direção do impulso
        Gizmos.color = Color.green;
        Vector3 posicao = transform.position;
        Vector3 direcao3D = new Vector3(direcaoImpulso.x, direcaoImpulso.y, 0);
        Vector3 direcaoNormalizada = normalizarDirecao ? direcao3D.normalized : direcao3D;
        Vector3 pontoFinal = posicao + direcaoNormalizada * 2f;
        
        // Linha da direção
        Gizmos.DrawLine(posicao, pontoFinal);
        
        // Ponta da seta
        Vector3 right = Quaternion.Euler(0, 0, 30) * -direcaoNormalizada * 0.5f;
        Vector3 left = Quaternion.Euler(0, 0, -30) * -direcaoNormalizada * 0.5f;
        Gizmos.DrawLine(pontoFinal, pontoFinal + right);
        Gizmos.DrawLine(pontoFinal, pontoFinal + left);
        
        // Esfera no centro
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(posicao, 0.3f);
    }
}
