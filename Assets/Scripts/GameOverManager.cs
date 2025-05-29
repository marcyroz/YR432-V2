using System.Collections;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Header("Referências dos elementos do ciclo")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject animationPanel;
    [SerializeField] private GameObject gameOverTextPanel;
    [SerializeField] private GameObject statsPanel;

    [Header("Timing")]
    [SerializeField] private float delayAfterAnimation = 1f;
    [SerializeField] private float delayAfterText = 5f;

    [Header("Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationTriggerName = "Play";

    public void StartGameOverCycle()
    {
        StartCoroutine(HandleGameOverSequence());
    }

    private IEnumerator HandleGameOverSequence()
    {
        // 1. Ativa a tela principal
        gameOverScreen.SetActive(true);

        // 2. Ativa a animação
        animationPanel.SetActive(true);
        animator.SetTrigger(animationTriggerName);

        // 3. Espera a animação terminar
        yield return new WaitUntil(() => AnimationFinished());

        yield return new WaitForSeconds(delayAfterAnimation);

        // 4. Ativa o texto "Game Over"
        gameOverTextPanel.SetActive(true);

        yield return new WaitForSeconds(delayAfterText);

        // 5. Mostra o painel de estatísticas
        statsPanel.SetActive(true);
    }

    private bool AnimationFinished()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 &&
               !animator.IsInTransition(0);
    }

    public void EndGameOverCycle()
    {
        gameOverScreen.SetActive(false);
        animationPanel.SetActive(true);
        gameOverTextPanel.SetActive(false);
        statsPanel.SetActive(false);
    }
}
