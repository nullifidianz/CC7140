using System.Collections;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    [Header("Configurações do Bônus")]
    [Tooltip("GameObject do artefato que será ativado (para BonusArtefato)")]
    public GameObject artefatoParaAtivar;
    
    [Header("Efeito de Balanço")]
    [Tooltip("Altura do balanço quando o player bate")]
    public float alturaBalanco = 0.3f;
    
    [Tooltip("Duração do balanço")]
    public float duracaoBalanco = 0.2f;
    
    [Header("Animação do Artefato")]
    [Tooltip("Altura que o artefato sobe quando aparece da caixinha")]
    public float alturaSubidaArtefato = 0.2f;
    
    [Tooltip("Duração da animação de subida do artefato")]
    public float duracaoSubidaArtefato = 0.3f;
    
    [Header("Áudio")]
    [Tooltip("Som que toca quando o bônus é ativado")]
    public AudioClip somBonus;
    
    private Vector3 posicaoInicial;
    private bool jaFoiAtivado = false;
    private AudioSource audioSource;
    
    void Start()
    {
        posicaoInicial = transform.position;
        
        // Pega ou adiciona o AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[Bonus] Colisão detectada com: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        
        // Verifica se é o player
        if (collision.gameObject.CompareTag("Player") && !jaFoiAtivado)
        {
            Debug.Log("[Bonus] É o player! Verificando direção da colisão...");
            
            // Verifica se o player bateu por baixo
            // Pega o ponto de contato
            foreach (ContactPoint2D contato in collision.contacts)
            {
                Debug.Log($"[Bonus] Normal do contato: {contato.normal}, Normal Y: {contato.normal.y}");
                
                // Se a normal do contato aponta para CIMA, significa que o player bateu por baixo
                if (contato.normal.y > 0.5f)
                {
                    Debug.Log("[Bonus] Player bateu por BAIXO! Ativando bônus...");
                    jaFoiAtivado = true;
                    StartCoroutine(EfeitoBalanco());
                    ProcessarBonus(collision.gameObject);
                    break;
                }
                else
                {
                    Debug.Log($"[Bonus] Player NÃO bateu por baixo (bateu de outra direção). Normal Y = {contato.normal.y}");
                }
            }
        }
        else if (!collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("[Bonus] Não é o player!");
        }
        else if (jaFoiAtivado)
        {
            Debug.Log("[Bonus] Bônus já foi ativado anteriormente!");
        }
    }
    
    IEnumerator EfeitoBalanco()
    {
        float tempoDecorrido = 0f;
        
        // Sobe
        while (tempoDecorrido < duracaoBalanco / 2)
        {
            tempoDecorrido += Time.deltaTime;
            float progresso = tempoDecorrido / (duracaoBalanco / 2);
            transform.position = posicaoInicial + Vector3.up * (alturaBalanco * progresso);
            yield return null;
        }
        
        // Desce
        tempoDecorrido = 0f;
        while (tempoDecorrido < duracaoBalanco / 2)
        {
            tempoDecorrido += Time.deltaTime;
            float progresso = tempoDecorrido / (duracaoBalanco / 2);
            transform.position = posicaoInicial + Vector3.up * (alturaBalanco * (1 - progresso));
            yield return null;
        }
        
        // Garante que volta para a posição inicial
        transform.position = posicaoInicial;
    }
    
    void ProcessarBonus(GameObject player)
    {
        Debug.Log($"[Bonus] ProcessarBonus chamado! Tag da caixinha: {gameObject.tag}");
        
        // Toca o som do bônus se houver
        if (somBonus != null && audioSource != null)
        {
            audioSource.PlayOneShot(somBonus);
            Debug.Log("[Bonus] Som do bônus reproduzido!");
        }
        
        if (gameObject.CompareTag("BonusArtefato"))
        {
            Debug.Log("[Bonus] É um BonusArtefato!");
            
            // Ativa o artefato que está na cena
            if (artefatoParaAtivar != null)
            {
                Debug.Log($"[Bonus] Ativando artefato: {artefatoParaAtivar.name}");
                artefatoParaAtivar.SetActive(true);
                StartCoroutine(AnimarArtefatoSubindo(artefatoParaAtivar));
                Debug.Log($"[Bonus] Artefato ativado com sucesso!");
            }
            else
            {
                Debug.LogWarning("[Bonus] ❌ Artefato Para Ativar NÃO está configurado no Inspector!");
            }
        }
        else if (gameObject.CompareTag("BonusTroll"))
        {
            Debug.Log("[Bonus] É um BonusTroll! Retornando player à posição inicial...");
            // Retorna o player à posição inicial
            RetornarPlayerAoPontoInicial(player);
            
            // Ativa o artefato se houver um configurado (opcional)
            if (artefatoParaAtivar != null)
            {
                Debug.Log($"[Bonus] Ativando artefato: {artefatoParaAtivar.name}");
                artefatoParaAtivar.SetActive(true);
                StartCoroutine(AnimarArtefatoSubindo(artefatoParaAtivar));
            }
        }
        else
        {
            Debug.LogWarning($"[Bonus] ⚠️ Tag não reconhecida! Tag atual: '{gameObject.tag}'. Use 'BonusArtefato' ou 'BonusTroll'");
        }
    }
    
    void RetornarPlayerAoPontoInicial(GameObject player)
    {
        Debug.Log("[Bonus] RetornarPlayerAoPontoInicial chamado!");
        
        // Primeiro tenta procurar por um GameObject com tag "Respawn" na cena
        try
        {
            GameObject pontoRespawn = GameObject.FindGameObjectWithTag("Respawn");
            
            if (pontoRespawn != null)
            {
                Debug.Log($"[Bonus] Ponto de respawn encontrado em: {pontoRespawn.transform.position}");
                
                // Teleporta o player para o ponto de respawn
                player.transform.position = pontoRespawn.transform.position;
                
                // Reseta a velocidade do Rigidbody2D se houver
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    Debug.Log("[Bonus] Velocidade do player resetada");
                }
                
                Debug.Log($"[Bonus] ✅ Player retornou ao ponto de respawn: {pontoRespawn.transform.position}");
                return;
            }
        }
        catch (UnityException)
        {
            Debug.LogWarning("[Bonus] Tag 'Respawn' não existe no projeto!");
        }
        
        // Se não encontrou o ponto de respawn, procura pela posição inicial salva em um script do player
        // Tenta pegar o componente PlayerController do Tarodev
        var playerController = player.GetComponent<TarodevController.PlayerController>();
        if (playerController != null)
        {
            // Reseta a velocidade
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
            
            // Como o PlayerController do Tarodev não tem posição inicial salva,
            // vamos procurar por "SpawnPoint" ou usar uma posição padrão
            GameObject spawnPoint = GameObject.Find("SpawnPoint");
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                Debug.Log($"[Bonus] ✅ Player retornou ao SpawnPoint: {spawnPoint.transform.position}");
                return;
            }
        }
        
        Debug.LogWarning("[Bonus] ⚠️ Nenhum ponto de respawn encontrado! Opções:\n" +
            "1. Crie um GameObject vazio com a tag 'Respawn' na posição inicial do player\n" +
            "2. Ou crie um GameObject chamado 'SpawnPoint' na posição inicial do player");
    }
    
    IEnumerator AnimarArtefatoSubindo(GameObject artefato)
    {
        Vector3 posicaoInicial = artefato.transform.position;
        Vector3 posicaoFinal = posicaoInicial + Vector3.up * alturaSubidaArtefato;
        float duracao = duracaoSubidaArtefato;
        float tempoDecorrido = 0f;
        
        while (tempoDecorrido < duracao)
        {
            tempoDecorrido += Time.deltaTime;
            float progresso = tempoDecorrido / duracao;
            artefato.transform.position = Vector3.Lerp(posicaoInicial, posicaoFinal, progresso);
            yield return null;
        }
        
        // Garante que chegou na posição final
        artefato.transform.position = posicaoFinal;
        
        // Atualiza a posição inicial do ArtefatoMovimento após a animação terminar
        ArtefatoMovimento movimento = artefato.GetComponent<ArtefatoMovimento>();
        if (movimento != null)
        {
            movimento.AtualizarPosicaoInicial();
        }
    }
}

