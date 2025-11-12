using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Espinhos : MonoBehaviour
{
    [Header("Configurações de Morte")]
    [Tooltip("Se marcado, teleporta o player para o Respawn. Se não, reseta a cena.")]
    public bool usarRespawn = false;

    [Header("Configurações de Espinho Troll")]
    [Tooltip("Distância em que o espinho Troll detecta o player")]
    public float distanciaDeteccao = 5f;
    
    [Tooltip("Velocidade de movimento do espinho Troll")]
    public float velocidadeMovimento = 3f;
    
    [Tooltip("Tipo de movimento do espinho Troll")]
    public TipoMovimentoTroll tipoMovimento = TipoMovimentoTroll.CicloVertical;
    
    [Tooltip("Distância do movimento (para movimentos de ciclo)")]
    public float distanciaMovimento = 2f;
    
    [Tooltip("Tempo de espera no movimento antes de voltar (em segundos)")]
    [Min(0)]
    public float tempoNoMovimento = 2f;
    
    [Tooltip("Tempo de espera embaixo antes de subir novamente (em segundos)")]
    [Min(0)]
    public float tempoEspera = 1f;
    
    [Tooltip("Mostrar área de detecção no editor")]
    public bool mostrarAreaDeteccao = true;
    
    [Header("Comportamento Troll")]
    [Tooltip("Ativar apenas quando player estiver próximo")]
    public bool apenasComPlayer = true;
    
    [Tooltip("Movimento contínuo (ciclo infinito) ou apenas uma vez")]
    public bool movimentoContinuo = true;
    
    [Tooltip("Usar tempo de espera antes de começar o movimento pela primeira vez")]
    public bool usarTempoEsperaInicial = false;
    
    [Tooltip("Tempo de espera antes de iniciar o movimento (em segundos)")]
    [Min(0)]
    public float tempoEsperaInicial = 1f;
    
    [Header("Requer Artefato")]
    [Tooltip("Se marcado, o espinho só começa a funcionar após o player pegar o artefato")]
    public bool requerArtefato = false;
    
    [Header("Debug")]
    [Tooltip("Mostrar mensagens de debug no Console")]
    public bool mostrarDebug = false;

    private Transform playerTransform;
    private Vector3 posicaoInicial;
    private Vector3 posicaoAlvo;
    private bool playerProximo = false;
    private bool playerProximoAnterior = false; // Rastreia o estado anterior do player
    private bool isTroll = false;
    private EstadoEspinho estadoAtual = EstadoEspinho.Embaixo;
    private float contadorTempo = 0f;
    private bool cicloAtivado = false;
    private bool primeiraAtivacao = true; // Controla se é a primeira vez que ativa
    private bool jaExecutouUmaVez = false; // Flag permanente para movimento não contínuo

    private enum EstadoEspinho
    {
        Embaixo,        // Na posição inicial
        Subindo,        // Movendo para cima
        NoTopo,         // Parado no topo
        Descendo        // Voltando para baixo
    }

    public enum TipoMovimentoTroll
    {
        CicloVertical,      // Sobe e desce em ciclo
        CicloHorizontal,    // Vai e volta horizontalmente em ciclo
        CicloDiagonal       // Movimento diagonal em ciclo
    }

    void Start()
    {
        // Verifica se este espinho tem a tag "Troll"
        isTroll = gameObject.CompareTag("Troll");
        
        // Salva a posição inicial
        posicaoInicial = transform.position;
        
        // Calcula a posição alvo se for troll
        if (isTroll)
        {
            CalcularPosicaoAlvo();
            
            if (mostrarDebug)
            {
                Debug.Log($"[Espinhos] Iniciado! Posição Inicial: {posicaoInicial}, Posição Alvo: {posicaoAlvo}");
            }
        }
    }

    void Update()
    {
        // Se for um espinho Troll, executa o ciclo de movimento
        if (isTroll)
        {
            // Se requer artefato e ainda não foi pego, não faz nada
            if (requerArtefato && !GameState.ArtefatoFoiPegado())
            {
                return;
            }
            
            // Verifica proximidade do player se necessário
            if (apenasComPlayer)
            {
                VerificarProximidadePlayer();
                
                // Detecta quando o player ENTRA no range pela primeira vez
                if (playerProximo && !playerProximoAnterior && primeiraAtivacao && estadoAtual == EstadoEspinho.Embaixo)
                {
                    // Se movimento não é contínuo e já executou uma vez, não ativa novamente
                    if (!movimentoContinuo && jaExecutouUmaVez)
                    {
                        if (mostrarDebug)
                        {
                            Debug.Log("[Espinhos] Movimento não contínuo já foi executado. Não ativa novamente.");
                        }
                    }
                    else
                    {
                        cicloAtivado = true;
                        
                        if (mostrarDebug)
                        {
                            Debug.Log("[Espinhos] ==== INICIANDO CICLO DE MOVIMENTO ====");
                        }
                    }
                }
                
                // Atualiza o estado anterior
                playerProximoAnterior = playerProximo;
            }
            else
            {
                // Modo automático - ativa uma única vez no início
                if (!cicloAtivado && primeiraAtivacao)
                {
                    cicloAtivado = true;
                }
            }

            // Executa o ciclo se estiver ativado
            if (cicloAtivado)
            {
                ExecutarCicloMovimento();
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
                    Debug.Log($"[Espinhos] Player encontrado: {player.name}");
                }
            }
            else if (mostrarDebug)
            {
                Debug.LogWarning("[Espinhos] Player não encontrado! Verifique se o GameObject tem a tag 'Player'");
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
                    Debug.Log($"[Espinhos] Player ENTROU na área de detecção! Distância: {distancia:F2}");
                }
                else
                {
                    Debug.Log($"[Espinhos] Player SAIU da área de detecção! Distância: {distancia:F2}");
                }
            }
        }
    }

    void CalcularPosicaoAlvo()
    {
        switch (tipoMovimento)
        {
            case TipoMovimentoTroll.CicloVertical:
                posicaoAlvo = posicaoInicial + Vector3.up * distanciaMovimento;
                break;

            case TipoMovimentoTroll.CicloHorizontal:
                posicaoAlvo = posicaoInicial + Vector3.right * distanciaMovimento;
                break;

            case TipoMovimentoTroll.CicloDiagonal:
                posicaoAlvo = posicaoInicial + (Vector3.right + Vector3.up).normalized * distanciaMovimento;
                break;
        }
    }

    void ExecutarCicloMovimento()
    {
        switch (estadoAtual)
        {
            case EstadoEspinho.Embaixo:
                // Se for a primeira ativação
                if (primeiraAtivacao)
                {
                    // Se usar tempo de espera inicial, aguarda antes de subir
                    if (usarTempoEsperaInicial)
                    {
                        contadorTempo += Time.deltaTime;
                        
                        if (mostrarDebug && contadorTempo < 0.1f)
                        {
                            Debug.Log($"[Espinhos] >>> PRIMEIRA ATIVAÇÃO! Aguardando {tempoEsperaInicial}s antes de iniciar movimento <<<");
                        }
                        
                        if (contadorTempo >= tempoEsperaInicial)
                        {
                            primeiraAtivacao = false;
                            estadoAtual = EstadoEspinho.Subindo;
                            contadorTempo = 0f;
                            
                            if (mostrarDebug)
                            {
                                Debug.Log($"[Espinhos] Tempo de espera inicial completo ({tempoEsperaInicial}s)! Iniciando movimento!");
                            }
                        }
                    }
                    else
                    {
                        // Sem delay, sobe imediatamente
                        primeiraAtivacao = false;
                        estadoAtual = EstadoEspinho.Subindo;
                        contadorTempo = 0f;
                        
                        if (mostrarDebug)
                        {
                            Debug.Log("[Espinhos] >>> PRIMEIRA ATIVAÇÃO! Subindo IMEDIATAMENTE (sem esperar) <<<");
                        }
                    }
                }
                else if (movimentoContinuo && cicloAtivado)
                {
                    // Ciclos contínuos - aguarda tempo de espera e sobe novamente
                    // (independente de onde o player está)
                    contadorTempo += Time.deltaTime;
                    
                    if (mostrarDebug && contadorTempo < 0.1f)
                    {
                        Debug.Log($"[Espinhos] Ciclo contínuo - Aguardando {tempoEspera}s embaixo...");
                    }
                    
                    if (contadorTempo >= tempoEspera)
                    {
                        contadorTempo = 0f;
                        estadoAtual = EstadoEspinho.Subindo;
                        
                        if (mostrarDebug)
                        {
                            Debug.Log($"[Espinhos] Tempo embaixo completo ({tempoEspera}s)! Subindo novamente!");
                        }
                    }
                }
                // Se não for primeira ativação e não for contínuo, não faz nada (aguarda nova ativação)
                break;

            case EstadoEspinho.Subindo:
                // Move para a posição alvo
                transform.position = Vector3.MoveTowards(transform.position, posicaoAlvo, velocidadeMovimento * Time.deltaTime);
                
                // Chegou no topo?
                if (Vector3.Distance(transform.position, posicaoAlvo) < 0.01f)
                {
                    transform.position = posicaoAlvo;
                    estadoAtual = EstadoEspinho.NoTopo;
                    contadorTempo = 0f;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log($"[Espinhos] Chegou no topo! Aguardando {tempoNoMovimento} segundos...");
                    }
                }
                break;

            case EstadoEspinho.NoTopo:
                // Esperando no topo antes de descer
                contadorTempo += Time.deltaTime;
                
                if (mostrarDebug && contadorTempo < 0.1f)
                {
                    Debug.Log($"[Espinhos] *** PARADA NO TOPO - Aguardando {tempoNoMovimento}s antes de descer ***");
                }
                
                if (contadorTempo >= tempoNoMovimento)
                {
                    contadorTempo = 0f;
                    estadoAtual = EstadoEspinho.Descendo;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log($"[Espinhos] Tempo no movimento completo ({tempoNoMovimento}s)! Iniciando descida!");
                    }
                }
                break;

            case EstadoEspinho.Descendo:
                // Move de volta para a posição inicial
                transform.position = Vector3.MoveTowards(transform.position, posicaoInicial, velocidadeMovimento * Time.deltaTime);
                
                // Chegou embaixo?
                if (Vector3.Distance(transform.position, posicaoInicial) < 0.01f)
                {
                    transform.position = posicaoInicial;
                    estadoAtual = EstadoEspinho.Embaixo;
                    contadorTempo = 0f;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log($"[Espinhos] Voltou para baixo! Movimento Contínuo = {movimentoContinuo}, Apenas Com Player = {apenasComPlayer}");
                    }
                    
                    // Decide o que fazer após voltar embaixo
                    if (movimentoContinuo)
                    {
                        // Movimento contínuo: mantém o ciclo ativo e continua repetindo
                        // independentemente da distância do player
                        primeiraAtivacao = false;
                        
                        if (mostrarDebug)
                        {
                            Debug.Log($"[Espinhos] Modo contínuo - Aguardando {tempoEspera}s antes de repetir (independente do player).");
                        }
                    }
                    else
                    {
                        // Movimento NÃO contínuo: para o ciclo completamente após uma execução
                        cicloAtivado = false;
                        primeiraAtivacao = true;
                        jaExecutouUmaVez = true; // Marca que já executou (não vai mais executar)
                        
                        if (mostrarDebug)
                        {
                            Debug.Log("[Espinhos] Movimento não contínuo - Ciclo FINALIZADO (não executará novamente nesta cena).");
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
            // Se requer artefato e ainda não foi pego, não mata
            if (requerArtefato && !GameState.ArtefatoFoiPegado())
            {
                return;
            }
            
            MatarPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // Se requer artefato e ainda não foi pego, não mata
            if (requerArtefato && !GameState.ArtefatoFoiPegado())
            {
                return;
            }
            
            MatarPlayer();
        }
    }

    void MatarPlayer()
    {
        // Busca o player para teleportar
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            if (usarRespawn)
            {
                Debug.Log("[Espinhos] Player tocou nos espinhos! Teleportando para o respawn...");
                // Teleporta o player para o respawn
                StartCoroutine(TeleportarPlayerParaRespawn(player));
            }
            else
            {
                Debug.Log("[Espinhos] Player tocou nos espinhos! Resetando a cena...");
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
            Debug.LogWarning("[Espinhos] Tag 'Respawn' não existe no projeto! Resetando a cena...");
            // Incrementa o contador de mortes
            ContadorMortes.IncrementarMorte();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield break;
        }
        
        if (pontoRespawn != null)
        {
            Debug.Log($"[Espinhos] Ponto de respawn encontrado em: {pontoRespawn.transform.position}");
            
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
            
            Debug.Log($"[Espinhos] ✅ Player teleportado para o respawn: {pontoRespawn.transform.position}");
        }
        else
        {
            Debug.LogWarning("[Espinhos] ⚠️ Nenhum GameObject com tag 'Respawn' encontrado! Resetando a cena...");
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
            estadoAtual = EstadoEspinho.Embaixo;
            contadorTempo = 0f;
            cicloAtivado = false;
            primeiraAtivacao = true;
            playerProximoAnterior = false;
            jaExecutouUmaVez = false;
            
            if (mostrarDebug)
            {
                Debug.Log("[Espinhos] Reset forçado!");
            }
        }
    }

    // Desenha a área de detecção no editor (apenas para espinhos Troll)
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
            }
        }
    }
}
