using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class AtivarCutscene : MonoBehaviour
{
    [Header("Configurações da Cutscene")]
    public PlayableDirector timelineDirector;  // Arraste o objeto com o Timeline aqui
    
    [Header("Configurações do Jogador")]
    public bool desabilitarControles = true;   // Desabilitar controles durante cutscene
    
    [Header("Configurações de Câmera (Opcional)")]
    public CinemachineVirtualCamera virtualCameraCutscene;  // Virtual Camera para a cutscene
    
    [Header("Configurações do Artefato")]
    public bool destruirArtefatoNoFinal = true;  // Destruir a moeda no final da cutscene?
    
    private bool cutsceneAtivada = false;      // Impede que a cutscene seja ativada múltiplas vezes
    private PlayerMovement playerMovement;
    private PlayerMovementInicio playerMovementInicio;
    private CinemachineBrain cinemachineBrain;
    private GameObject artefatoColetado;  // Referência ao artefato que ativou a cutscene

    void Start()
    {
        // Se não foi atribuído manualmente, tenta encontrar o PlayableDirector na cena
        if (timelineDirector == null)
        {
            timelineDirector = FindFirstObjectByType<PlayableDirector>();
        }
        
        // Busca os scripts de movimento do jogador
        playerMovement = GetComponent<PlayerMovement>();
        playerMovementInicio = GetComponent<PlayerMovementInicio>();
        
        // Garante que o Cinemachine Brain está ativo
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu tem a tag "Artefato" e se a cutscene ainda não foi ativada
        if (other.CompareTag("Artefato") && !cutsceneAtivada)
        {
            artefatoColetado = other.gameObject;  // Guarda a referência do artefato
            AtivarCutsceneArtefato();
        }
    }

    void AtivarCutsceneArtefato()
    {
        if (timelineDirector != null)
        {
            cutsceneAtivada = true;
            
            // Garante que o Cinemachine Brain está ativo
            if (cinemachineBrain != null)
            {
                cinemachineBrain.enabled = true;
            }
            
            // Ativa a Virtual Camera da cutscene se foi configurada
            if (virtualCameraCutscene != null)
            {
                virtualCameraCutscene.Priority = 100;  // Prioridade alta
            }
            
            // Desabilita o controle do jogador durante a cutscene
            if (desabilitarControles)
            {
                if (playerMovement != null)
                {
                    playerMovement.enabled = false;
                }
                
                if (playerMovementInicio != null)
                {
                    playerMovementInicio.enabled = false;
                }
            }
            
            // Inicia a cutscene
            timelineDirector.Play();
            
            // Reabilitar controles após o fim da cutscene
            StartCoroutine(ReabilitarControlesAposCutscene());
        }
    }

    IEnumerator ReabilitarControlesAposCutscene()
    {
        // Espera a cutscene terminar
        float duracao = (float)timelineDirector.duration;
        yield return new WaitForSeconds(duracao);
        
        // Destrói o artefato no final da cutscene
        if (destruirArtefatoNoFinal && artefatoColetado != null)
        {
            Destroy(artefatoColetado);
        }
        
        // Restaura a prioridade da Virtual Camera da cutscene
        if (virtualCameraCutscene != null)
        {
            virtualCameraCutscene.Priority = 0;  // Volta para prioridade baixa
        }
        
        // Reabilita o controle do jogador
        if (desabilitarControles)
        {
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
            
            if (playerMovementInicio != null)
            {
                playerMovementInicio.enabled = true;
            }
        }
    }
}
