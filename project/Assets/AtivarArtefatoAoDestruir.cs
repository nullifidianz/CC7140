using UnityEngine;

public class AtivarArtefatoAoDestruir : MonoBehaviour
{
    [Header("Artefato")]
    [Tooltip("Arraste aqui o GameObject do Artefato que deve ser ativado quando este objeto for destruído")]
    public GameObject artefatoParaAtivar;
    
    [Header("Configurações de Autodestruição")]
    [Tooltip("Ativar verificação de coordenada X?")]
    public bool verificarX = true;
    
    [Tooltip("Coordenada X em que o objeto será destruído")]
    public float coordenadaX;
    
    [Tooltip("Ativar verificação de coordenada Y?")]
    public bool verificarY = true;
    
    [Tooltip("Coordenada Y em que o objeto será destruído")]
    public float coordenadaY;
    
    [Tooltip("Distância mínima para considerar que chegou na coordenada (tolerância)")]
    public float tolerancia = 0.5f;

    void Update()
    {
        bool deveDestruir = false;
        
        // Verifica X se estiver ativado
        if (verificarX)
        {
            float distanciaX = Mathf.Abs(transform.position.x - coordenadaX);
            
            // Se só X está ativado, verifica apenas X
            if (!verificarY)
            {
                deveDestruir = distanciaX <= tolerancia;
            }
            // Se X e Y estão ativados, precisa verificar ambos
            else
            {
                float distanciaY = Mathf.Abs(transform.position.y - coordenadaY);
                deveDestruir = (distanciaX <= tolerancia && distanciaY <= tolerancia);
            }
        }
        // Se só Y está ativado
        else if (verificarY)
        {
            float distanciaY = Mathf.Abs(transform.position.y - coordenadaY);
            deveDestruir = distanciaY <= tolerancia;
        }
        
        // Destrói o objeto se a condição foi atendida
        if (deveDestruir)
        {
            Destroy(gameObject);
        }
    }
    
    void OnDestroy()
    {
        // Quando o objeto for destruído, ativa o artefato
        if (artefatoParaAtivar != null)
        {
            artefatoParaAtivar.SetActive(true);
        }
    }
}
