using UnityEngine;

public class ArtefatoMovimento : MonoBehaviour
{
    [Header("Efeito de Flutuação")]
    [Tooltip("Altura da flutuação (quanto sobe e desce)")]
    public float alturaDaFlutuacao = 0.25f;
    
    [Tooltip("Velocidade da flutuação")]
    public float velocidadeDaFlutuacao = 2f;
    
    private Vector3 posicaoInicial;
    private bool posicaoInicialDefinida = false;
    
    void Update()
    {
        // Só flutua se a posição inicial já foi definida
        if (posicaoInicialDefinida)
        {
            // Calcula a flutuação usando uma onda senoidal suave
            float novoY = posicaoInicial.y + Mathf.Sin(Time.time * velocidadeDaFlutuacao) * alturaDaFlutuacao;
            
            // Aplica a nova posição mantendo X e Z iguais
            transform.position = new Vector3(posicaoInicial.x, novoY, transform.position.z);
        }
    }
    
    // Método público para atualizar a posição inicial (chamado após animação de subida)
    public void AtualizarPosicaoInicial()
    {
        posicaoInicial = transform.position;
        posicaoInicialDefinida = true;
    }
}
