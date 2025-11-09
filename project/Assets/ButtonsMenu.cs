using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsMenu : MonoBehaviour
{
    [Header("Configuração da Cena")]
    [Tooltip("Índice da cena que será carregada (Build Index)")]
    public int sceneIndex;

    [Tooltip("Ou use o nome da cena")]
    public string sceneName;

    [Header("Configuração Tela Inicial")]
    [Tooltip("Nome da tela inicial (ex: 'Tela Inicial'). Se vazio, usa o índice abaixo")]
    public string telaInicialNome = "";
    
    [Tooltip("Índice da tela inicial (usado apenas se o nome estiver vazio)")]
    public int telaInicialIndex = 0;

    // Método para carregar cena por índice
    public void LoadSceneByIndex()
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError($"Índice de cena inválido: {sceneIndex}. Verifique o Build Settings!");
        }
    }

    // Método para carregar cena por nome
    public void LoadSceneByName()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Nome da cena está vazio!");
        }
    }

    // Método conveniente para carregar a próxima cena
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Esta é a última cena!");
        }
    }

    // Método para recarregar a cena atual
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Método para voltar à tela inicial
    public void VoltarTelaInicial()
    {
        // Se o nome da cena foi especificado, usa ele
        if (!string.IsNullOrEmpty(telaInicialNome))
        {
            SceneManager.LoadScene(telaInicialNome);
        }
        // Senão, usa o índice
        else if (telaInicialIndex >= 0 && telaInicialIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(telaInicialIndex);
        }
        else
        {
            Debug.LogError("[ButtonsMenu] Índice da tela inicial inválido!");
        }
    }

    // Método para sair do jogo
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
