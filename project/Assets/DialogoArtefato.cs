using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogoArtefato : MonoBehaviour
{
    public GameObject dialoguePanel;   // Arraste o Panel do diálogo aqui
    public Text textComponent;
    public Image dialogueImage;
    public CanvasGroup panelGroup;
    public string[] lines;
    public float textSpeed;
    public float displayTime = 3f;
    public float fadeInTime = 0.5f;   // Tempo de fade in
    public float fadeOutTime = 1f;    // Tempo de fade out
    private int index;
    public static bool isDialogueActive = false;
    private bool hasBeenShown = false; // Controla se o diálogo já foi exibido

    void Start()
    {
        // Garante que o panel começa invisível
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        
        if (panelGroup != null)
        {
            panelGroup.alpha = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu tem a tag "Dialogo" e se o diálogo ainda não foi mostrado
        if (other.CompareTag("Dialogo") && !isDialogueActive && !hasBeenShown)
        {
            ActivateDialogue();
        }
    }

    void ActivateDialogue()
    {
        if (isDialogueActive) return;
        
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }
        
        if (panelGroup != null)
        {
            panelGroup.alpha = 0; // Começa transparente para o fade in
        }
        
        isDialogueActive = true;
        hasBeenShown = true; // Marca que o diálogo já foi exibido
        
        if (textComponent != null)
        {
            textComponent.text = string.Empty;
            StartCoroutine(ShowDialogueSequence());
        }
    }

    IEnumerator ShowDialogueSequence()
    {
        if (lines.Length == 0 || lines[index] == null)
        {
            Debug.LogError("Nenhuma linha de diálogo definida!");
            yield break;
        }

        // Fade In - Aparece suavemente
        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInTime);
            
            if (panelGroup != null)
            {
                panelGroup.alpha = alpha;
            }
            
            yield return null;
        }
        
        // Garante que ficou totalmente visível
        if (panelGroup != null)
        {
            panelGroup.alpha = 1f;
        }

        // Animação de entrada do texto (digitação)
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        // Espera o tempo configurado
        yield return new WaitForSeconds(displayTime);

        // Fade Out - Desaparece suavemente
        elapsedTime = 0f;
        while (elapsedTime < fadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutTime);
            
            if (panelGroup != null)
            {
                panelGroup.alpha = alpha;
            }
            
            yield return null;
        }

        // Desativa o panel ao finalizar
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        isDialogueActive = false;
    }
}
