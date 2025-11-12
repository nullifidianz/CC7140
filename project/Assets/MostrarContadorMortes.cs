using UnityEngine;
using TMPro;

/// <summary>
/// Exibe o contador de mortes em um TextMeshPro UI
/// Coloque este script em um GameObject que tenha um componente TextMeshProUGUI
/// ou configure o campo "textoMortes" no Inspector
/// </summary>
public class MostrarContadorMortes : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Componente de texto onde as mortes serão exibidas")]
    public TextMeshProUGUI textoMortes;

    [Tooltip("Formato do texto. Use {0} para o número de mortes")]
    public string formatoTexto = "Mortes: {0}";

    [Tooltip("Cor do texto")]
    public Color corTexto = Color.white;

    [Header("Configurações Opcionais")]
    [Tooltip("Atualizar automaticamente a cada frame")]
    public bool atualizarAutomaticamente = true;

    void Start()
    {
        // Se não foi configurado, tenta pegar o TextMeshPro do próprio GameObject
        if (textoMortes == null)
        {
            textoMortes = GetComponent<TextMeshProUGUI>();
        }

        // Verifica se encontrou o componente
        if (textoMortes == null)
        {
            Debug.LogError("[MostrarContadorMortes] ⚠️ TextMeshProUGUI não encontrado! Adicione um componente TextMeshProUGUI ou configure o campo no Inspector.");
            return;
        }

        // Aplica a cor configurada
        textoMortes.color = corTexto;

        // Atualiza o texto pela primeira vez
        AtualizarTexto();

        Debug.Log("[MostrarContadorMortes] Sistema de exibição iniciado!");
    }

    void Update()
    {
        // Atualiza o texto automaticamente se configurado
        if (atualizarAutomaticamente && textoMortes != null)
        {
            AtualizarTexto();
        }
    }

    /// <summary>
    /// Atualiza o texto com o número atual de mortes
    /// </summary>
    public void AtualizarTexto()
    {
        if (textoMortes != null)
        {
            int mortes = ContadorMortes.ObterTotalMortes();
            textoMortes.text = string.Format(formatoTexto, mortes);
        }
    }
}

