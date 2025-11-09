using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configurações do Efeito Hover")]
    [Tooltip("Escala do texto quando o mouse passa por cima (1.1 = 10% maior)")]
    [Range(1.0f, 2.0f)]
    public float hoverScale = 1.1f;
    
    [Tooltip("Cor do texto original")]
    public Color normalColor = Color.black;
    
    [Tooltip("Cor do texto quando o mouse passa por cima")]
    public Color hoverColor = Color.white;
    
    [Tooltip("Velocidade da transição")]
    public float transitionSpeed = 10f;

    [Header("Configurações de Ancoragem")]
    [Tooltip("Ajustar o pivot para crescer da esquerda para direita")]
    public bool anchorLeft = true;

    private Transform textTransform;
    private RectTransform textRectTransform;
    private TextMeshProUGUI textMeshPro;
    private Text textUI;
    private Vector3 normalScale;
    private Vector3 targetScale;
    private Color targetColor;
    private bool useTextMeshPro = false;

    void Start()
    {
        // Tenta encontrar o componente de texto no botão
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            useTextMeshPro = true;
            normalColor = textMeshPro.color;
            textTransform = textMeshPro.transform;
            textRectTransform = textMeshPro.GetComponent<RectTransform>();
        }
        else
        {
            textUI = GetComponentInChildren<Text>();
            if (textUI != null)
            {
                normalColor = textUI.color;
                textTransform = textUI.transform;
                textRectTransform = textUI.GetComponent<RectTransform>();
            }
        }

        if (textTransform != null)
        {
            // Ajusta o pivot para crescer da esquerda para direita
            if (anchorLeft && textRectTransform != null)
            {
                textRectTransform.pivot = new Vector2(0f, 0.5f);
            }
            
            normalScale = textTransform.localScale;
            targetScale = normalScale;
        }
        
        targetColor = normalColor;
    }

    void Update()
    {
        // Anima suavemente a transição (usa unscaledDeltaTime para funcionar mesmo com o jogo pausado)
        if (textTransform != null)
        {
            textTransform.localScale = Vector3.Lerp(textTransform.localScale, targetScale, Time.unscaledDeltaTime * transitionSpeed);
        }
        
        if (useTextMeshPro && textMeshPro != null)
        {
            textMeshPro.color = Color.Lerp(textMeshPro.color, targetColor, Time.unscaledDeltaTime * transitionSpeed);
        }
        else if (textUI != null)
        {
            textUI.color = Color.Lerp(textUI.color, targetColor, Time.unscaledDeltaTime * transitionSpeed);
        }
    }

    // Chamado quando o mouse entra na área do botão
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = normalScale * hoverScale;
        targetColor = hoverColor;
    }

    // Chamado quando o mouse sai da área do botão
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = normalScale;
        targetColor = normalColor;
    }
}


