using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Serra : MonoBehaviour
{
    [Header("Configurações de Morte")]
    [Tooltip("Se marcado, a serra pode matar o player. Se não, a serra não causa dano.")]
    public bool matavel = true;
    
    [Tooltip("Se marcado, teleporta o player para o Respawn. Se não, reseta a cena.")]
    public bool usarRespawn = false;

    [Header("Configurações de Rotação")]
    [Tooltip("Velocidade de rotação da serra (em graus por segundo)")]
    public float velocidadeRotacao = 360f;

    [Header("Configurações Serra Troll")]
    [Tooltip("Distância em que a serra Troll detecta o player")]
    public float distanciaDeteccao = 5f;
    
    [Tooltip("Mostrar área de detecção no editor")]
    public bool mostrarAreaDeteccao = true;
    
    [Header("Primeiro Movimento")]
    [Tooltip("Eixo do primeiro movimento")]
    public EixoMovimento primeiroMovimentoEixo = EixoMovimento.X;
    
    [Tooltip("Distância do primeiro movimento")]
    public float primeiroMovimentoDistancia = 3f;
    
    [Tooltip("Tempo de duração do primeiro movimento (em segundos)")]
    [Min(0.1f)]
    public float primeiroMovimentoTempo = 2f;
    
    [Header("Segundo Movimento")]
    [Tooltip("Eixo do segundo movimento")]
    public EixoMovimento segundoMovimentoEixo = EixoMovimento.Y;
    
    [Tooltip("Distância do segundo movimento")]
    public float segundoMovimentoDistancia = 3f;
    
    [Tooltip("Velocidade do segundo movimento")]
    [Min(0.1f)]
    public float segundoMovimentoVelocidade = 3f;
    
    [Header("Tempo de Espera")]
    [Tooltip("Tempo de espera entre o primeiro e segundo movimento (em segundos)")]
    [Min(0)]
    public float tempoEsperaEntreMovimentos = 1f;
    
    [Tooltip("Tempo de espera após completar todos os movimentos antes de voltar (em segundos)")]
    [Min(0)]
    public float tempoEsperaAntesDeVoltar = 1f;
    
    [Header("Comportamento")]
    [Tooltip("Movimento contínuo (ciclo infinito) ou apenas uma vez")]
    public bool movimentoContinuo = true;
    
    [Header("Debug")]
    [Tooltip("Mostrar mensagens de debug no Console")]
    public bool mostrarDebug = false;

    private Transform playerTransform;
    private Vector3 posicaoInicial;
    private Vector3 posicaoAposPrimeiroMovimento;
    private Vector3 posicaoFinal;
    private bool playerProximo = false;
    private bool isTroll = false;
    private EstadoSerra estadoAtual = EstadoSerra.Parado;
    private float contadorTempo = 0f;
    private bool movimentoAtivado = false;
    private bool jaExecutouUmaVez = false; // Flag permanente para movimento não contínuo

    private enum EstadoSerra
    {
        Parado,                 // Parada na posição inicial
        PrimeiroMovimento,      // Executando o primeiro movimento
        EsperandoSegundo,       // Esperando entre o primeiro e segundo movimento
        SegundoMovimento,       // Executando o segundo movimento
        EsperandoVoltar,        // Esperando antes de voltar
        VoltandoSegundo,        // Voltando do segundo movimento
        VoltandoPrimeiro        // Voltando do primeiro movimento
    }

    public enum EixoMovimento
    {
        X,  // Movimento horizontal
        Y   // Movimento vertical
    }

    void Start()
    {
        // Verifica se esta serra tem a tag "Troll"
        isTroll = gameObject.CompareTag("Troll");
        
        // Salva a posição inicial
        posicaoInicial = transform.position;
        
        // Calcula as posições se for troll
        if (isTroll)
        {
            CalcularPosicoes();
            
            if (mostrarDebug)
            {
                Debug.Log($"[Serra] Iniciada! Posição Inicial: {posicaoInicial}");
                Debug.Log($"[Serra] Após Primeiro Movimento: {posicaoAposPrimeiroMovimento}");
                Debug.Log($"[Serra] Posição Final: {posicaoFinal}");
            }
        }
    }

    void Update()
    {
        // Rotação constante da serra (sempre gira) - usando Vector3.forward para o eixo Z
        transform.Rotate(Vector3.forward, velocidadeRotacao * Time.deltaTime);
        
        // Se for uma serra Troll, executa a lógica de movimento
        if (isTroll)
        {
            // Verifica proximidade do player
            VerificarProximidadePlayer();
            
            // Se o player está próximo e ainda não ativou o movimento
            if (playerProximo && !movimentoAtivado && estadoAtual == EstadoSerra.Parado)
            {
                // Se movimento não é contínuo e já executou uma vez, não ativa novamente
                if (!movimentoContinuo && jaExecutouUmaVez)
                {
                    if (mostrarDebug)
                    {
                        Debug.Log("[Serra] Movimento não contínuo já foi executado. Não ativa novamente.");
                    }
                }
                else
                {
                    movimentoAtivado = true;
                    estadoAtual = EstadoSerra.PrimeiroMovimento;
                    contadorTempo = 0f;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log("[Serra] ==== MOVIMENTO ATIVADO! Iniciando primeiro movimento ====");
                    }
                }
            }
            
            // Executa o movimento se estiver ativado
            if (movimentoAtivado)
            {
                ExecutarMovimento();
            }
        }
    }

    void VerificarProximidadePlayer()
    {
        // Tenta encontrar o player se ainda não tiver referência
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                if (mostrarDebug)
                {
                    Debug.Log($"[Serra] Player encontrado: {player.name}");
                }
            }
        }

        // Verifica se o player está dentro da distância de detecção
        if (playerTransform != null)
        {
            float distancia = Vector3.Distance(transform.position, playerTransform.position);
            bool proximoAntes = playerProximo;
            playerProximo = distancia <= distanciaDeteccao;
            
            if (mostrarDebug && playerProximo != proximoAntes)
            {
                if (playerProximo)
                {
                    Debug.Log($"[Serra] Player ENTROU na área de detecção! Distância: {distancia:F2}");
                }
                else
                {
                    Debug.Log($"[Serra] Player SAIU da área de detecção! Distância: {distancia:F2}");
                }
            }
        }
    }

    void CalcularPosicoes()
    {
        // Calcula a posição após o primeiro movimento
        Vector3 direcaoPrimeiro = ObterDirecao(primeiroMovimentoEixo);
        posicaoAposPrimeiroMovimento = posicaoInicial + direcaoPrimeiro * primeiroMovimentoDistancia;
        
        // Calcula a posição final (após o segundo movimento)
        Vector3 direcaoSegundo = ObterDirecao(segundoMovimentoEixo);
        posicaoFinal = posicaoAposPrimeiroMovimento + direcaoSegundo * segundoMovimentoDistancia;
    }

    Vector3 ObterDirecao(EixoMovimento eixo)
    {
        switch (eixo)
        {
            case EixoMovimento.X:
                return Vector3.right;
            case EixoMovimento.Y:
                return Vector3.up;
            default:
                return Vector3.zero;
        }
    }

    void ExecutarMovimento()
    {
        switch (estadoAtual)
        {
            case EstadoSerra.PrimeiroMovimento:
                // Move para a posição do primeiro movimento baseado no tempo
                contadorTempo += Time.deltaTime;
                float progresso = Mathf.Clamp01(contadorTempo / primeiroMovimentoTempo);
                transform.position = Vector3.Lerp(posicaoInicial, posicaoAposPrimeiroMovimento, progresso);
                
                // Completou o primeiro movimento?
                if (progresso >= 1f)
                {
                    transform.position = posicaoAposPrimeiroMovimento;
                    estadoAtual = EstadoSerra.EsperandoSegundo;
                    contadorTempo = 0f;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log($"[Serra] Primeiro movimento completo! Aguardando {tempoEsperaEntreMovimentos}s...");
                    }
                }
                break;

            case EstadoSerra.EsperandoSegundo:
                // Espera entre o primeiro e segundo movimento
                contadorTempo += Time.deltaTime;
                
                if (contadorTempo >= tempoEsperaEntreMovimentos)
                {
                    contadorTempo = 0f;
                    estadoAtual = EstadoSerra.SegundoMovimento;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log("[Serra] Iniciando segundo movimento!");
                    }
                }
                break;

            case EstadoSerra.SegundoMovimento:
                // Move para a posição final com velocidade constante
                transform.position = Vector3.MoveTowards(transform.position, posicaoFinal, segundoMovimentoVelocidade * Time.deltaTime);
                
                // Chegou na posição final?
                if (Vector3.Distance(transform.position, posicaoFinal) < 0.01f)
                {
                    transform.position = posicaoFinal;
                    estadoAtual = EstadoSerra.EsperandoVoltar;
                    contadorTempo = 0f;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log($"[Serra] Segundo movimento completo! Aguardando {tempoEsperaAntesDeVoltar}s antes de voltar...");
                    }
                }
                break;

            case EstadoSerra.EsperandoVoltar:
                // Espera antes de voltar
                contadorTempo += Time.deltaTime;
                
                if (contadorTempo >= tempoEsperaAntesDeVoltar)
                {
                    contadorTempo = 0f;
                    estadoAtual = EstadoSerra.VoltandoSegundo;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log("[Serra] Voltando! Desfazendo segundo movimento...");
                    }
                }
                break;

            case EstadoSerra.VoltandoSegundo:
                // Volta do segundo movimento
                transform.position = Vector3.MoveTowards(transform.position, posicaoAposPrimeiroMovimento, segundoMovimentoVelocidade * Time.deltaTime);
                
                if (Vector3.Distance(transform.position, posicaoAposPrimeiroMovimento) < 0.01f)
                {
                    transform.position = posicaoAposPrimeiroMovimento;
                    estadoAtual = EstadoSerra.VoltandoPrimeiro;
                    contadorTempo = 0f;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log("[Serra] Segundo movimento desfeito! Desfazendo primeiro movimento...");
                    }
                }
                break;

            case EstadoSerra.VoltandoPrimeiro:
                // Volta do primeiro movimento
                contadorTempo += Time.deltaTime;
                float progressoVolta = Mathf.Clamp01(contadorTempo / primeiroMovimentoTempo);
                transform.position = Vector3.Lerp(posicaoAposPrimeiroMovimento, posicaoInicial, progressoVolta);
                
                if (progressoVolta >= 1f)
                {
                    transform.position = posicaoInicial;
                    estadoAtual = EstadoSerra.Parado;
                    contadorTempo = 0f;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log($"[Serra] Voltou à posição inicial! Movimento Contínuo = {movimentoContinuo}");
                    }
                    
                    // Decide o que fazer após voltar
                    if (movimentoContinuo)
                    {
                        // Movimento contínuo: mantém pronta para repetir quando o player se aproximar novamente
                        movimentoAtivado = false;
                        
                        if (mostrarDebug)
                        {
                            Debug.Log("[Serra] Modo contínuo - Pronta para nova ativação quando o player se aproximar.");
                        }
                    }
                    else
                    {
                        // Movimento NÃO contínuo: para completamente após uma execução
                        movimentoAtivado = false;
                        jaExecutouUmaVez = true; // Marca que já executou (não vai mais executar)
                        
                        if (mostrarDebug)
                        {
                            Debug.Log("[Serra] Movimento não contínuo - Ciclo FINALIZADO (não executará novamente nesta cena).");
                        }
                    }
                }
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o objeto que colidiu tem a tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            MatarPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            MatarPlayer();
        }
    }

    void MatarPlayer()
    {
        // Se a serra não é matável, não faz nada
        if (!matavel)
        {
            if (mostrarDebug)
            {
                Debug.Log("[Serra] Player tocou na serra, mas ela não é matável!");
            }
            return;
        }
        
        // Busca o player para teleportar
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            if (usarRespawn)
            {
                Debug.Log("[Serra] Player tocou na serra! Teleportando para o respawn...");
                // Teleporta o player para o respawn
                StartCoroutine(TeleportarPlayerParaRespawn(player));
            }
            else
            {
                Debug.Log("[Serra] Player tocou na serra! Resetando a cena...");
                // Incrementa o contador de mortes
                ContadorMortes.IncrementarMorte();
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
            Debug.LogWarning("[Serra] Tag 'Respawn' não existe no projeto! Resetando a cena...");
            // Incrementa o contador de mortes
            ContadorMortes.IncrementarMorte();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield break;
        }
        
        if (pontoRespawn != null)
        {
            Debug.Log($"[Serra] Ponto de respawn encontrado em: {pontoRespawn.transform.position}");
            
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
            
            Debug.Log($"[Serra] ✅ Player teleportado para o respawn: {pontoRespawn.transform.position}");
        }
        else
        {
            Debug.LogWarning("[Serra] ⚠️ Nenhum GameObject com tag 'Respawn' encontrado! Resetando a cena...");
            // Incrementa o contador de mortes
            ContadorMortes.IncrementarMorte();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Método público para forçar o reset (pode ser chamado por outros scripts)
    public void ForcarReset()
    {
        if (isTroll)
        {
            transform.position = posicaoInicial;
            estadoAtual = EstadoSerra.Parado;
            contadorTempo = 0f;
            movimentoAtivado = false;
            jaExecutouUmaVez = false;
            
            if (mostrarDebug)
            {
                Debug.Log("[Serra] Reset forçado!");
            }
        }
    }

    // Desenha a área de detecção no editor (apenas para serras Troll)
    private void OnDrawGizmosSelected()
    {
        if (isTroll || (Application.isPlaying == false && gameObject.CompareTag("Troll")))
        {
            if (mostrarAreaDeteccao)
            {
                // Desenha a área de detecção como uma esfera wireframe
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, distanciaDeteccao);
                
                // Desenha uma esfera menor no centro
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 0.2f);
                
                // Desenha linhas mostrando o caminho do movimento
                if (Application.isPlaying)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(posicaoInicial, posicaoAposPrimeiroMovimento);
                    Gizmos.DrawLine(posicaoAposPrimeiroMovimento, posicaoFinal);
                    
                    // Marca as posições
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(posicaoInicial, 0.3f);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(posicaoAposPrimeiroMovimento, 0.3f);
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(posicaoFinal, 0.3f);
                }
            }
        }
    }
}
