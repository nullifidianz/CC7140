using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gerenciador de estado global do jogo
/// Reseta automaticamente quando a cena é carregada
/// </summary>
public class GameState : MonoBehaviour
{
    private static GameState instance;
    private bool artefatoPegado = false;
    
    void Awake()
    {
        // Singleton simples que se destrói ao trocar de cena
        if (instance == null)
        {
            instance = this;
            
            // NÃO usa DontDestroyOnLoad - queremos que resete a cada cena
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Reseta o estado ao iniciar a cena
        artefatoPegado = false;
        
        // Se inscreve no evento de cena carregada para resetar
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reseta o estado quando uma nova cena é carregada
        artefatoPegado = false;
        Debug.Log("[GameState] Cena carregada - Estado resetado!");
    }
    
    /// <summary>
    /// Marca que o artefato foi pego
    /// </summary>
    public static void MarcarArtefatoPegado()
    {
        if (instance != null)
        {
            instance.artefatoPegado = true;
            Debug.Log("[GameState] Artefato foi pegado!");
        }
    }
    
    /// <summary>
    /// Verifica se o artefato foi pego nesta cena
    /// </summary>
    public static bool ArtefatoFoiPegado()
    {
        return instance != null && instance.artefatoPegado;
    }
}

