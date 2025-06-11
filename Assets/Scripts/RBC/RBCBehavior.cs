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

    void OnEnable()
    {
        // Toda vez que a pool reativa este objeto,
        // voltamos ao sprite “saudável” original.
        rend.sprite = originalSprite;
    }

    public void Infect()
    {
        if (infectedSprite != null)
            rend.sprite = infectedSprite;
    }

    public void ResetSprite()
    {
        rend.sprite = originalSprite;
    }
}
