using UnityEngine;
using UnityEngine.EventSystems;

public class UIToggle : MonoBehaviour
{
    [Header("Painel a ser ativado/desativado")]
    [SerializeField] GameObject targetPanel;

    [Header("Botão controlador (opcional)")]
    [SerializeField] GameObject toggleButton;

    private bool isOpen = false;

    void Awake()
    {
        // Se não for definido no inspetor, assume o próprio botão
        if (toggleButton == null)
            toggleButton = this.gameObject;
    }

    void Update()
    {
        // Fecha se clicar fora
        if (isOpen && Input.GetMouseButtonDown(0))
        {
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

        RectTransform rectTransform = go.GetComponent<RectTransform>();
        if (rectTransform == null) return false;

        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, null);
    }
}
