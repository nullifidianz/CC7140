using UnityEngine;

/// <summary>
/// Gerencia o sistema de progressão e salvamento de fases
/// Usa PlayerPrefs para persistir quais fases estão liberadas
/// </summary>
public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;
    
    // Quantidade total de fases no jogo
    [Header("Configuração")]
    [Tooltip("Número total de fases no jogo")]
    public int totalDeFases = 7;
    
    [Tooltip("Sempre libera todas as fases (útil para testes)")]
    public bool modoTeste = false;

    void Awake()
    {
        // Singleton que persiste entre cenas
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Garante que a Fase 1 sempre está liberada
        if (GetInt("Fase1Liberada", 0) == 0)
        {
            LiberarFase(1);
        }
    }

    #region Métodos Estáticos Públicos
    
    /// <summary>
    /// Libera uma fase específica
    /// </summary>
    /// <param name="numeroFase">Número da fase (1, 2, 3...)</param>
    public static void LiberarFase(int numeroFase)
    {
        string chave = $"Fase{numeroFase}Liberada";
        PlayerPrefs.SetInt(chave, 1);
        PlayerPrefs.Save(); // Força salvar imediatamente
        Debug.Log($"[LevelManager] Fase {numeroFase} foi liberada!");
    }

    /// <summary>
    /// Verifica se uma fase está liberada
    /// </summary>
    /// <param name="numeroFase">Número da fase (1, 2, 3...)</param>
    /// <returns>True se a fase está liberada</returns>
    public static bool FaseEstaLiberada(int numeroFase)
    {
        // Modo teste: todas as fases liberadas
        if (instance != null && instance.modoTeste)
        {
            return true;
        }
        
        string chave = $"Fase{numeroFase}Liberada";
        return PlayerPrefs.GetInt(chave, 0) == 1;
    }

    /// <summary>
    /// Libera a próxima fase em sequência
    /// </summary>
    /// <param name="faseAtual">Número da fase que acabou de ser completada</param>
    public static void CompletarFaseELiberarProxima(int faseAtual)
    {
        int proximaFase = faseAtual + 1;
        
        if (instance != null && proximaFase <= instance.totalDeFases)
        {
            LiberarFase(proximaFase);
            Debug.Log($"[LevelManager] Fase {faseAtual} completada! Fase {proximaFase} liberada!");
        }
        else
        {
            Debug.Log($"[LevelManager] Fase {faseAtual} completada! Todas as fases já foram liberadas!");
        }
    }

    /// <summary>
    /// Reseta todo o progresso (libera apenas a Fase 1)
    /// </summary>
    public static void ResetarProgresso()
    {
        PlayerPrefs.DeleteAll();
        LiberarFase(1); // Fase 1 sempre começa liberada
        Debug.Log("[LevelManager] Progresso resetado!");
    }

    /// <summary>
    /// Retorna quantas fases o jogador já desbloqueou
    /// </summary>
    public static int GetFasesDesbloqueadas()
    {
        int count = 0;
        if (instance != null)
        {
            for (int i = 1; i <= instance.totalDeFases; i++)
            {
                if (FaseEstaLiberada(i))
                {
                    count++;
                }
            }
        }
        return count;
    }

    #endregion

    #region Métodos Auxiliares

    private static int GetInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    #endregion

    #region Debug / Teste

    // Método para testar no Inspector ou console
    [ContextMenu("Liberar Todas as Fases")]
    private void LiberarTodasAsFases()
    {
        for (int i = 1; i <= totalDeFases; i++)
        {
            LiberarFase(i);
        }
        Debug.Log("[LevelManager] Todas as fases foram liberadas!");
    }

    [ContextMenu("Resetar Progresso")]
    private void ResetarProgressoDebug()
    {
        ResetarProgresso();
    }

    [ContextMenu("Mostrar Fases Liberadas")]
    private void MostrarFasesLiberadas()
    {
        for (int i = 1; i <= totalDeFases; i++)
        {
            bool liberada = FaseEstaLiberada(i);
            Debug.Log($"Fase {i}: {(liberada ? "✅ LIBERADA" : "🔒 BLOQUEADA")}");
        }
    }

    #endregion
}

