using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script opcional para resetar o contador de mortes
/// Útil para botões de "Novo Jogo" ou "Voltar ao Menu"
/// </summary>
public class ResetarContadorMortes : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Resetar o contador automaticamente ao iniciar esta cena")]
    public bool resetarAoIniciar = false;

    [Tooltip("Nome da cena para carregar após resetar (opcional)")]
    public string nomeCenaParaCarregar = "";

    void Start()
    {
        if (resetarAoIniciar)
        {
            ResetarContador();
        }
    }

    /// <summary>
    /// Reseta o contador de mortes para 0
    /// Pode ser chamado por botões no Inspector
    /// </summary>
    public void ResetarContador()
    {
        ContadorMortes.ResetarContador();
        Debug.Log("[ResetarContadorMortes] ✅ Contador resetado!");
    }

    /// <summary>
    /// Reseta o contador e carrega uma cena
    /// Pode ser chamado por botões no Inspector
    /// </summary>
    public void ResetarECarregarCena()
    {
        ResetarContador();

        if (!string.IsNullOrEmpty(nomeCenaParaCarregar))
        {
            Debug.Log($"[ResetarContadorMortes] Carregando cena: {nomeCenaParaCarregar}");
            SceneManager.LoadScene(nomeCenaParaCarregar);
        }
        else
        {
            Debug.LogWarning("[ResetarContadorMortes] ⚠️ Nome da cena não foi configurado!");
        }
    }

    /// <summary>
    /// Reseta o contador e carrega uma cena pelo índice
    /// Pode ser chamado por botões no Inspector
    /// </summary>
    public void ResetarECarregarCenaPorIndice(int indiceCena)
    {
        ResetarContador();
        Debug.Log($"[ResetarContadorMortes] Carregando cena de índice: {indiceCena}");
        SceneManager.LoadScene(indiceCena);
    }
}

