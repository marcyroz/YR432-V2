using UnityEngine;
using UnityEngine.EventSystems;

public class UIToggle : MonoBehaviour
{
    [Header("O que será ativado/desativado")]
    [SerializeField] GameObject targetPanel;

    [Header("Referencia opcional ao botão que controla o toggle")]
    [SerializeField] GameObject toggleButton;

    private bool isOpen = false;

    void Update()
    {
        // Verifica clique fora quando está aberto
        if (isOpen && Input.GetMouseButtonDown(0))
        {
            // Se o clique NÃO foi no painel e NÃO foi no botão, fecha
            if (!IsPointerOverGameObject(targetPanel) && !IsPointerOverGameObject(toggleButton))
            {
                TogglePanel(false);
            }
        }
    }

    public void TogglePanel()
    {
        TogglePanel(!isOpen);
    }

    public void TogglePanel(bool state)
    {
        isOpen = state;
        targetPanel.SetActive(isOpen);
    }

    private bool IsPointerOverGameObject(GameObject go)
    {
        if (go == null) return false;

        // Cria um retângulo da área do objeto na tela
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        if (rectTransform == null) return false;

        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, null);
    }
}
