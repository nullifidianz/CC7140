using UnityEngine;

public class ArtefatoMovimento : MonoBehaviour
{
    [Header("Efeito de Flutuação")]
    [Tooltip("Altura da flutuação (quanto sobe e desce)")]
    public float alturaDaFlutuacao = 0.25f;
    
    [Tooltip("Velocidade da flutuação")]
    public float velocidadeDaFlutuacao = 2f;
    
    private Vector3 posicaoInicial;
    
    void Start()
    {
        // Salva a posição inicial do artefato
        posicaoInicial = transform.position;
    }
    
    void Update()
    {
        // Calcula a flutuação usando uma onda senoidal suave
        float novoY = posicaoInicial.y + Mathf.Sin(Time.time * velocidadeDaFlutuacao) * alturaDaFlutuacao;
        
        // Aplica a nova posição mantendo X e Z iguais
        transform.position = new Vector3(posicaoInicial.x, novoY, transform.position.z);
    }
}
