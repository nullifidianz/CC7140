using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// Script para cutscene final que volta automaticamente para a tela inicial
/// Coloque este script no mesmo GameObject que tem o PlayableDirector (Timeline)
/// </summary>
public class CutsceneFinal : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Tempo extra de espera após a cutscene terminar (em segundos)")]
    public float tempoEsperaExtra = 2f;
    
    [Tooltip("Índice da cena para voltar (veja File > Build Settings)")]
    public int indiceCenaDestino = 0;
    
    private PlayableDirector timeline;
    private bool cutsceneTerminou = false;

    void Start()
    {
        // Pega o Timeline no mesmo GameObject
        timeline = GetComponent<PlayableDirector>();
        
        if (timeline == null)
        {
            Debug.LogError("[CutsceneFinal] PlayableDirector não encontrado! Adicione este script no GameObject que tem o Timeline.");
            return;
        }
        
        // Inicia a cutscene automaticamente
        timeline.Play();
        
        Debug.Log("[CutsceneFinal] Cutscene iniciada!");
    }

    void Update()
    {
        // Verifica se a cutscene terminou
        if (timeline != null && timeline.state != PlayState.Playing && !cutsceneTerminou)
        {
            cutsceneTerminou = true;
            StartCoroutine(VoltarParaTelaInicial());
        }
    }
    
    /// <summary>
    /// Aguarda um tempo e volta para a tela inicial
    /// </summary>
    IEnumerator VoltarParaTelaInicial()
    {
        Debug.Log($"[CutsceneFinal] Cutscene terminou! Voltando para cena {indiceCenaDestino} em {tempoEsperaExtra}s...");
        
        // Aguarda o tempo configurado
        yield return new WaitForSeconds(tempoEsperaExtra);
        
        // Carrega a cena de destino
        SceneManager.LoadScene(indiceCenaDestino);
    }
}

