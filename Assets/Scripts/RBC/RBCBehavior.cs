using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RBCBehavior : MonoBehaviour
{
    [Tooltip("Sprite que será usado quando este RBC for infectado")]
    public Sprite infectedSprite;

    private SpriteRenderer rend;
    private Sprite originalSprite;

    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        originalSprite = rend.sprite;
    }

    /// <summary>
    /// Chame para marcar visualmente este RBC como infectado.
    /// </summary>
    public void Infect()
    {
        if (infectedSprite != null)
            rend.sprite = infectedSprite;
    }

    /// <summary>
    /// (Opcional) Caso queira resetar o sprite mais tarde:
    /// </summary>
    public void ResetSprite()
    {
        rend.sprite = originalSprite;
    }
}
