using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Configuração do Menu de Pause")]
    [Tooltip("Painel que contém o menu de pause (Canvas com fundo preto e botões)")]
    public GameObject pauseMenuPanel;

    [Header("Configurações de Cenas")]
    [Tooltip("Nome da cena do menu inicial (ex: 'Tela Inicial'). Se vazio, usa o índice abaixo")]
    public string menuPrincipalNome = "";
    
    [Tooltip("Índice da cena do menu inicial (usado apenas se o nome estiver vazio)")]
    public int menuPrincipalIndex = 0;

    [Tooltip("Índice da cena de como jogar (deixe -1 se não tiver)")]
    public int comoJogarIndex = -1;

    [Header("Tecla de Pause")]
    [Tooltip("Tecla para abrir/fechar o menu de pause")]
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("Debug")]
    [Tooltip("Mostrar mensagens de debug no console")]
    public bool showDebug = true;

    private bool isPaused = false;

    void Start()
    {
        if (showDebug)
        {
            Debug.Log("[PauseMenu] Script inicializado!");
            if (pauseMenuPanel != null)
            {
                Debug.Log("[PauseMenu] Painel atribuído: " + pauseMenuPanel.name);
            }
            else
            {
                Debug.LogError("[PauseMenu] ERRO: Painel não foi atribuído no Inspector!");
            }
        }
        
        // Garante que o jogo começa despausado
        ResumeGame();
    }

    void Update()
    {
        // Detecta quando o jogador aperta ESC
        if (Input.GetKeyDown(pauseKey))
        {
            if (showDebug)
            {
                Debug.Log("[PauseMenu] Tecla ESC detectada! isPaused = " + isPaused);
            }
            
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Pausa o jogo
    public void PauseGame()
    {
        if (showDebug)
        {
            Debug.Log("[PauseMenu] Pausando o jogo...");
        }
        
        isPaused = true;
        Time.timeScale = 0f; // Para o tempo do jogo
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true); // Mostra o menu
            if (showDebug)
            {
                Debug.Log("[PauseMenu] Painel ativado!");
            }
        }
        else
        {
            Debug.LogWarning("[PauseMenu] Painel de pause não foi atribuído no Inspector!");
        }
    }

    // Resume o jogo
    public void ResumeGame()
    {
        if (showDebug)
        {
            Debug.Log("[PauseMenu] Despausando o jogo...");
        }
        
        isPaused = false;
        Time.timeScale = 1f; // Volta o tempo normal
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false); // Esconde o menu
            if (showDebug)
            {
                Debug.Log("[PauseMenu] Painel desativado!");
            }
        }
    }

    // Reinicia a fase atual
    public void ResetarFase()
    {
        if (showDebug)
        {
            Debug.Log("[PauseMenu] Reiniciando a fase...");
        }
        
        Time.timeScale = 1f; // Garante que o tempo volta ao normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Volta para o menu principal
    public void VoltarAoInicio()
    {
        if (showDebug)
        {
            Debug.Log("[PauseMenu] Voltando ao menu principal (cena 0)...");
        }
        
        Time.timeScale = 1f; // Garante que o tempo volta ao normal
        SceneManager.LoadScene(0); // Carrega a cena 0 (menu inicial)
    }

    // Vai para a cena de Como Jogar
    public void ComoJogar()
    {
        if (showDebug)
        {
            Debug.Log("[PauseMenu] Botão COMO JOGAR pressionado!");
        }
        
        Time.timeScale = 1f; // Garante que o tempo volta ao normal
        
        if (comoJogarIndex >= 0 && comoJogarIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(comoJogarIndex);
        }
        else
        {
            Debug.LogWarning("[PauseMenu] Cena 'Como Jogar' não configurada ou inválida!");
        }
    }

    // Sai do jogo
    public void SairDoJogo()
    {
        if (showDebug)
        {
            Debug.Log("[PauseMenu] Saindo do jogo...");
        }
        
        Time.timeScale = 1f; // Garante que o tempo volta ao normal
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}

