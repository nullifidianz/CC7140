using UnityEngine;

/// <summary>
/// Sistema global de contador de mortes que persiste entre as cenas
/// Este script deve ser adicionado a um GameObject vazio na primeira cena do jogo
/// </summary>
public class ContadorMortes : MonoBehaviour
{
    // Singleton para garantir que só existe uma instância
    private static ContadorMortes _instance;
    public static ContadorMortes Instance
    {
        get
        {
            // Se não existe instância, cria uma
            if (_instance == null)
            {
                GameObject go = new GameObject("ContadorMortes");
                _instance = go.AddComponent<ContadorMortes>();
            }
            return _instance;
        }
    }

    // Contador de mortes (privado, só pode ser modificado por métodos públicos)
    private int totalMortes = 0;

    void Awake()
    {
        // Se já existe uma instância e não é esta, destrói este objeto
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Define esta como a instância
        _instance = this;

        // Mantém este objeto entre as cenas
        DontDestroyOnLoad(gameObject);

        Debug.Log("[ContadorMortes] Sistema iniciado! Pronto para contar mortes.");
    }

    /// <summary>
    /// Incrementa o contador de mortes em 1
    /// </summary>
    public static void IncrementarMorte()
    {
        Instance.totalMortes++;
        Debug.Log($"[ContadorMortes] 💀 Morte #{Instance.totalMortes} registrada!");
    }

    /// <summary>
    /// Retorna o número total de mortes
    /// </summary>
    public static int ObterTotalMortes()
    {
        return Instance.totalMortes;
    }

    /// <summary>
    /// Reseta o contador de mortes para 0
    /// </summary>
    public static void ResetarContador()
    {
        Instance.totalMortes = 0;
        Debug.Log("[ContadorMortes] Contador resetado para 0");
    }
}

