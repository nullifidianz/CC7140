using UnityEngine;

public class Paredes : MonoBehaviour
{
    [Header("Configurações de Detecção")]
    [Tooltip("Distância em que a parede detecta o player")]
    public float distanciaDeteccao = 5f;
    
    [Tooltip("Mostrar área de detecção no editor")]
    public bool mostrarAreaDeteccao = true;

    [Header("Configurações de Movimento")]
    [Tooltip("Tipo de movimento da parede")]
    public TipoMovimentoParede tipoMovimento = TipoMovimentoParede.MovimentoVertical;
    
    [Tooltip("Distância que a parede se move")]
    public float distanciaMovimento = 3f;
    
    [Tooltip("Velocidade do movimento")]
    public float velocidadeMovimento = 2f;
    
    [Tooltip("Tempo de espera no movimento antes de voltar (em segundos)")]
    [Min(0)]
    public float tempoNoMovimento = 2f;
    
    [Tooltip("Tempo de espera embaixo antes de subir novamente (em segundos)")]
    [Min(0)]
    public float tempoEspera = 1f;

    [Header("Comportamento")]
    [Tooltip("Ativar apenas quando player estiver próximo")]
    public bool apenasComPlayer = true;
    
    [Tooltip("Movimento contínuo (ciclo infinito) ou apenas uma vez")]
    public bool movimentoContinuo = true;
    
    [Header("Requer Artefato")]
    [Tooltip("Se marcado, a parede só começa a funcionar após o player pegar o artefato")]
    public bool requerArtefato = false;
    
    [Header("Debug")]
    [Tooltip("Mostrar mensagens de debug no Console")]
    public bool mostrarDebug = false;

    private Transform playerTransform;
    private Vector3 posicaoInicial;
    private Vector3 posicaoAlvo;
    private bool playerProximo;
    private bool playerProximoAnterior;
    private EstadoParede estadoAtual = EstadoParede.Embaixo;
    private float contadorTempo;
    private bool cicloAtivado;
    private bool primeiraAtivacao = true;
    private bool cicloCompletado;
    private bool jaExecutouUmaVez;

    private enum EstadoParede
    {
        Embaixo,        // Na posição inicial
        Subindo,        // Movendo para cima
        NoTopo,         // Parada no topo
        Descendo        // Voltando para baixo
    }

    public enum TipoMovimentoParede
    {
        MovimentoHorizontal,    // Move para a direita ou esquerda
        MovimentoVertical,      // Move para cima ou para baixo
        MovimentoDiagonal,      // Move diagonalmente
        MovimentoCustomizado    // Usa o vetor de direção customizado
    }

    [Header("Movimento Customizado")]
    [Tooltip("Direção customizada do movimento (apenas para Movimento Customizado)")]
    public Vector3 direcaoCustomizada = Vector3.right;

    void Start()
    {
        posicaoInicial = transform.position;
        CalcularPosicaoAlvo();
    }

    void Update()
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
            
            // Detecta quando o player SAI do range após ter completado um ciclo
            if (!playerProximo && playerProximoAnterior && cicloCompletado && movimentoContinuo)
            {
                // Prepara para um novo ciclo quando o player voltar (APENAS se movimento for contínuo)
                primeiraAtivacao = true;
                cicloCompletado = false;
                
                if (mostrarDebug)
                {
                    Debug.Log("[Paredes] Player saiu do range. Pronto para nova ativação (modo contínuo).");
                }
            }
            
            // Detecta quando o player ENTRA no range
            if (playerProximo && !playerProximoAnterior && primeiraAtivacao && estadoAtual == EstadoParede.Embaixo)
            {
                // Se movimento não é contínuo e já executou uma vez, não ativa novamente
                if (!movimentoContinuo && jaExecutouUmaVez)
                {
                    if (mostrarDebug)
                    {
                        Debug.Log("[Paredes] Movimento não contínuo já foi executado. Não ativa novamente.");
                    }
                }
                else
                {
                    IniciarCiclo();
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
                IniciarCiclo();
            }
        }

        // Executa o ciclo se estiver ativado
        if (cicloAtivado)
        {
            ExecutarCicloMovimento();
        }
    }
    
    void IniciarCiclo()
    {
        cicloAtivado = true;
        
        if (mostrarDebug)
        {
            Debug.Log("[Paredes] ==== INICIANDO CICLO DE MOVIMENTO ====");
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
                    Debug.Log($"[Paredes] Player encontrado: {player.name}");
                }
            }
            else if (mostrarDebug)
            {
                Debug.LogWarning("[Paredes] Player não encontrado! Verifique se o GameObject tem a tag 'Player'");
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
                    Debug.Log($"[Paredes] Player ENTROU na área de detecção! Distância: {distancia:F2}");
                }
                else
                {
                    Debug.Log($"[Paredes] Player SAIU da área de detecção! Distância: {distancia:F2}");
                }
            }
        }
    }

    void CalcularPosicaoAlvo()
    {
        switch (tipoMovimento)
        {
            case TipoMovimentoParede.MovimentoHorizontal:
                posicaoAlvo = posicaoInicial + Vector3.right * distanciaMovimento;
                break;

            case TipoMovimentoParede.MovimentoVertical:
                posicaoAlvo = posicaoInicial + Vector3.up * distanciaMovimento;
                break;

            case TipoMovimentoParede.MovimentoDiagonal:
                posicaoAlvo = posicaoInicial + (Vector3.right + Vector3.up).normalized * distanciaMovimento;
                break;

            case TipoMovimentoParede.MovimentoCustomizado:
                posicaoAlvo = posicaoInicial + direcaoCustomizada.normalized * distanciaMovimento;
                break;
        }
    }

    void ExecutarCicloMovimento()
    {
        switch (estadoAtual)
        {
            case EstadoParede.Embaixo:
                // Se for a primeira ativação, sobe imediatamente SEM DELAY
                if (primeiraAtivacao)
                {
                    primeiraAtivacao = false;
                    estadoAtual = EstadoParede.Subindo;
                    contadorTempo = 0f;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log("[Paredes] >>> PRIMEIRA ATIVAÇÃO! Subindo IMEDIATAMENTE (sem esperar) <<<");
                    }
                }
                else if (movimentoContinuo)
                {
                    // Ciclos contínuos - aguarda tempo de espera e sobe novamente
                    contadorTempo += Time.deltaTime;
                    
                    if (mostrarDebug && contadorTempo < 0.1f)
                    {
                        Debug.Log($"[Paredes] Ciclo contínuo - Aguardando {tempoEspera}s embaixo...");
                    }
                    
                    if (contadorTempo >= tempoEspera)
                    {
                        contadorTempo = 0f;
                        estadoAtual = EstadoParede.Subindo;
                        
                        if (mostrarDebug)
                        {
                            Debug.Log($"[Paredes] Tempo embaixo completo ({tempoEspera}s)! Subindo novamente!");
                        }
                    }
                }
                // Se não for primeira ativação e não for contínuo, não faz nada (aguarda nova ativação)
                break;

            case EstadoParede.Subindo:
                // Move para cima
                transform.position = Vector3.MoveTowards(transform.position, posicaoAlvo, velocidadeMovimento * Time.deltaTime);
                
                // Chegou no topo?
                if (Vector3.Distance(transform.position, posicaoAlvo) < 0.01f)
                {
                    transform.position = posicaoAlvo;
                    estadoAtual = EstadoParede.NoTopo;
                    contadorTempo = 0f;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log($"[Paredes] Chegou no topo! Aguardando {tempoNoMovimento} segundos...");
                    }
                }
                break;

            case EstadoParede.NoTopo:
                // Esperando no topo antes de descer
                contadorTempo += Time.deltaTime;
                
                if (mostrarDebug && contadorTempo < 0.1f)
                {
                    Debug.Log($"[Paredes] *** PARADA NO TOPO - Aguardando {tempoNoMovimento}s antes de descer ***");
                }
                
                if (contadorTempo >= tempoNoMovimento)
                {
                    contadorTempo = 0f;
                    estadoAtual = EstadoParede.Descendo;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log($"[Paredes] Tempo no movimento completo ({tempoNoMovimento}s)! Iniciando descida!");
                    }
                }
                break;

            case EstadoParede.Descendo:
                // Move para baixo
                transform.position = Vector3.MoveTowards(transform.position, posicaoInicial, velocidadeMovimento * Time.deltaTime);
                
                // Chegou embaixo?
                if (Vector3.Distance(transform.position, posicaoInicial) < 0.01f)
                {
                    transform.position = posicaoInicial;
                    estadoAtual = EstadoParede.Embaixo;
                    contadorTempo = 0f;
                    
                    if (mostrarDebug)
                    {
                        Debug.Log($"[Paredes] Voltou para baixo! Movimento Contínuo = {movimentoContinuo}, Apenas Com Player = {apenasComPlayer}");
                    }
                    
                    // Decide o que fazer após voltar embaixo
                    if (movimentoContinuo)
                    {
                        // Movimento contínuo: mantém o ciclo ativo e continua repetindo
                        primeiraAtivacao = false;
                        
                        if (apenasComPlayer)
                        {
                            // Com detecção de player: para e aguarda player sair e voltar
                            cicloAtivado = false;
                            cicloCompletado = true;
                            
                            if (mostrarDebug)
                            {
                                Debug.Log("[Paredes] Ciclo COMPLETADO. Aguardando player sair e voltar ao range para repetir.");
                            }
                        }
                        else
                        {
                            // Modo automático: aguarda tempo de espera e repete
                            if (mostrarDebug)
                            {
                                Debug.Log($"[Paredes] Modo contínuo - Aguardando {tempoEspera}s antes de repetir.");
                            }
                        }
                    }
                    else
                    {
                        // Movimento NÃO contínuo: para o ciclo completamente após uma execução
                        cicloAtivado = false;
                        primeiraAtivacao = true;
                        jaExecutouUmaVez = true; // Marca que já executou (não vai mais executar)
                        
                        if (apenasComPlayer)
                        {
                            cicloCompletado = true;
                        }
                        
                        if (mostrarDebug)
                        {
                            Debug.Log("[Paredes] Movimento não contínuo - Ciclo FINALIZADO (não executará novamente nesta cena).");
                        }
                    }
                }
                break;
        }
    }

    // Método público para forçar o reset (pode ser chamado por outros scripts)
    public void ForcarReset()
    {
        transform.position = posicaoInicial;
        estadoAtual = EstadoParede.Embaixo;
        contadorTempo = 0f;
        cicloAtivado = false;
        primeiraAtivacao = true;
        cicloCompletado = false;
        playerProximoAnterior = false;
        jaExecutouUmaVez = false;
        
        if (mostrarDebug)
        {
            Debug.Log("[Paredes] Reset forçado!");
        }
    }

    // Desenha a área de detecção no editor
    private void OnDrawGizmosSelected()
    {
        if (mostrarAreaDeteccao)
        {
            // Desenha a área de detecção
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, distanciaDeteccao);
            
            // Desenha a linha do movimento
            Vector3 posInicial = Application.isPlaying ? posicaoInicial : transform.position;
            Vector3 posAlvo;

            switch (tipoMovimento)
            {
                case TipoMovimentoParede.MovimentoHorizontal:
                    posAlvo = posInicial + Vector3.right * distanciaMovimento;
                    break;
                case TipoMovimentoParede.MovimentoVertical:
                    posAlvo = posInicial + Vector3.up * distanciaMovimento;
                    break;
                case TipoMovimentoParede.MovimentoDiagonal:
                    posAlvo = posInicial + (Vector3.right + Vector3.up).normalized * distanciaMovimento;
                    break;
                case TipoMovimentoParede.MovimentoCustomizado:
                    posAlvo = posInicial + direcaoCustomizada.normalized * distanciaMovimento;
                    break;
                default:
                    posAlvo = posInicial;
                    break;
            }

            // Linha verde mostrando o caminho
            Gizmos.color = Color.green;
            Gizmos.DrawLine(posInicial, posAlvo);
            
            // Marcador na posição alvo
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(posAlvo, Vector3.one * 0.3f);
        }
    }
}
