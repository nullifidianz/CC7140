using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Script para botões de seleção de fase
/// Gerencia a aparência e interatividade baseado se a fase está liberada
/// </summary>
[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    [Header("Configuração da Fase")]
    [Tooltip("Número da fase que este botão representa (1, 2, 3...)")]
    public int numeroFase = 1;
    
    [Tooltip("Nome da cena da fase (ex: 'Fase 1', 'Fase 2')")]
    public string nomeCenaFase = "Fase 1";

    private Button botao;

    void Start()
    {
        botao = GetComponent<Button>();
        AtualizarEstadoBotao();
    }

    /// <summary>
    /// Atualiza o botão baseado se a fase está liberada
    /// </summary>
    public void AtualizarEstadoBotao()
    {
        bool faseEstaLiberada = LevelManager.FaseEstaLiberada(numeroFase);

        // Habilita ou desabilita o botão (Unity cuida do visual automaticamente)
        if (botao != null)
        {
            botao.interactable = faseEstaLiberada;
        }

        Debug.Log($"[LevelButton] Fase {numeroFase}: {(faseEstaLiberada ? "✅ LIBERADA" : "🔒 BLOQUEADA")}");
    }

    /// <summary>
    /// Carrega a fase quando o botão é clicado
    /// Este método deve ser chamado pelo evento OnClick do botão
    /// </summary>
    public void CarregarFase()
    {
        if (LevelManager.FaseEstaLiberada(numeroFase))
        {
            Debug.Log($"[LevelButton] Carregando {nomeCenaFase}...");
            SceneManager.LoadScene(nomeCenaFase);
        }
        else
        {
            Debug.LogWarning($"[LevelButton] Tentativa de carregar fase {numeroFase} bloqueada!");
        }
    }

    // Método para forçar atualização (útil para testes no Inspector)
    [ContextMenu("Atualizar Estado")]
    private void ForcarAtualizacao()
    {
        if (botao == null)
        {
            botao = GetComponent<Button>();
        }
        AtualizarEstadoBotao();
    }
}

