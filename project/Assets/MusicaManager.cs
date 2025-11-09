using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gerencia a música dos menus, mantendo ela tocando apenas nas cenas com a layer "Menu"
/// Adicione este script em um GameObject vazio chamado "Music Manager"
/// Adicione um componente AudioSource no mesmo GameObject
/// </summary>
public class MusicaManager : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Nome da layer que identifica cenas de menu (padrão: 'Menu')")]
    public string layerMenu = "Menu";
    
    private static MusicaManager instance;
    private bool primeiraVez = true;
    
    void Awake()
    {
        // Verifica se já existe uma instância
        if (instance != null && instance != this)
        {
            // Se já existe, destrói este objeto para não duplicar a música
            Destroy(gameObject);
            return;
        }
        
        // Define esta como a instância única
        instance = this;
        
        // Mantém este objeto vivo entre as cenas
        DontDestroyOnLoad(gameObject);
        
        // Inscreve no evento de mudança de cena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDestroy()
    {
        // Remove a inscrição do evento
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        // Limpa a instância se for esta
        if (instance == this)
        {
            instance = null;
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Pula a verificação na primeira vez (cena inicial onde o Music Manager foi criado)
        if (primeiraVez)
        {
            primeiraVez = false;
            return;
        }
        
        // Verifica se existe algum objeto com a layer "Menu" na cena
        bool isCenaDeMenu = ExisteLayerNaCena(layerMenu);
        
        // Se não é uma cena de menu, destrói o Music Manager (para a música)
        if (!isCenaDeMenu)
        {
            Destroy(gameObject);
        }
    }
    
    bool ExisteLayerNaCena(string nomeLayer)
    {
        // Converte o nome da layer para o número da layer
        int layerNumber = LayerMask.NameToLayer(nomeLayer);
        
        // Se a layer não existe, retorna falso
        if (layerNumber == -1)
        {
            return false;
        }
        
        // Procura por qualquer GameObject com essa layer na cena
        GameObject[] todosObjetos = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in todosObjetos)
        {
            // Ignora o próprio Music Manager na busca
            if (obj == gameObject)
                continue;
                
            if (obj.layer == layerNumber)
            {
                return true;
            }
        }
        
        return false;
    }
}

