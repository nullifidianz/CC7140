using UnityEngine;

public class Artefato : MonoBehaviour
{
    [Header("Portal")]
    [Tooltip("Arraste aqui o GameObject do Portal que deve ser ativado após pegar o artefato")]
    public GameObject portalParaAtivar;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu foi o Player
        if (other.CompareTag("Player"))
        {
            // Marca que o artefato foi pego (para ativar mecânicas que dependem disso)
            GameState.MarcarArtefatoPegado();
            
            // Ativa o portal após pegar o artefato (se houver um portal configurado)
            if (portalParaAtivar != null)
            {
                portalParaAtivar.SetActive(true);
            }
            
            // Destroi o artefato após ser pego
            Destroy(gameObject);
        }
    }
}

