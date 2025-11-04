using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    private Vector3 posicaoInicial;
    private bool jaFoiAtivado = false;
    
    void Start()
    {
        posicaoInicial = transform.position;
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
            Debug.Log("[Bonus] É um BonusTroll! Matando o player...");
            // Mata o player e recarrega a cena (volta ao início da fase)
            MatarPlayer();
        }
        else
        {
            Debug.LogWarning($"[Bonus] ⚠️ Tag não reconhecida! Tag atual: '{gameObject.tag}'. Use 'BonusArtefato' ou 'BonusTroll'");
        }
    }
    
    void MatarPlayer()
    {
        // Recarrega a cena imediatamente (volta ao início da fase)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

