using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownScript : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private StatsManagerScript statsManager;
    [SerializeField] private GameRulesScript gameRules;
    [SerializeField] private GameCtrScript gameController;
    [SerializeField] private TypewriterEffect typewriter;
    [SerializeField] private DialogData HighDoseGoodDialog;
    [SerializeField] private DialogData LowDoseGoodDialog;
    [SerializeField] private DialogData HighDoseBadDialog;
    [SerializeField] private DialogData LowDoseBadDialog;
    [SerializeField] private DialogData GoodEndingDialog;
    [SerializeField] private DialogData GameOverDialog;
    [SerializeField] private float countdownDuration = 60f;
    [SerializeField] private MusicManager musicManager;
    private float currentTime;
    private Coroutine countdownRoutine;

    private bool isEndGameTriggered = false;

    void Start()
    {
        // Garantir que dependências estão atribuídas
        if (gameRules == null)
            gameRules = FindFirstObjectByType<GameRulesScript>();

        if (gameController == null)
            gameController = FindFirstObjectByType<GameCtrScript>();
    }

    public void StartCountdownCycle()
    {
        if (countdownRoutine == null)
        {
            currentTime = countdownDuration;
            countdownRoutine = StartCoroutine(CountdownLoop());
        }
    }

    public void StopCountdownCycle()
    {
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;

            if (timerText != null)
                timerText.text = "00:00";
        }
    }

    private IEnumerator CountdownLoop()
    {
        while (true)
        {
            while (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                UpdateTimerUI();
                yield return null;
            }

            EndCycle(); // ⬅️ Chamada centralizada
            currentTime = countdownDuration;
        }
    }

    private void UpdateTimerUI()
    {
        int totalSeconds = Mathf.CeilToInt(currentTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void RefillDoses()
    {
        int newDoses = Random.Range(1, 21);
        statsManager?.AddDoses(newDoses);

        if (newDoses >= 10)
        {
            typewriter.PlayDialog(HighDoseGoodDialog); // Diálogo para alta dose boa
        }
        else if (newDoses < 10)
        {
            typewriter.PlayDialog(LowDoseGoodDialog); // Diálogo para baixa dose boa
        }
    }

    private void RefillDosesIfNoEnd()
    {
        if (isEndGameTriggered) return; // evita conflitos com finais

        int newDoses = Random.Range(1, 21);
        statsManager?.AddDoses(newDoses);

        int virusCount = gameRules?.countBoard?.VirusCount ?? 0;

        if (newDoses >= 10)
        {
            if (virusCount > 10) // ⬅️ define a regra do "HighDoseBadDialog"
                typewriter.PlayDialog(HighDoseBadDialog);
            else
                typewriter.PlayDialog(HighDoseGoodDialog);
        }
        else
        {
            if (virusCount > 10) // ⬅️ define a regra do "LowDoseBadDialog"
                typewriter.PlayDialog(LowDoseBadDialog);
            else
                typewriter.PlayDialog(LowDoseGoodDialog);
        }
    }

    private void EndCycle()
    {
        CheckGameState();
    }


    private IEnumerator WaitDialogThenEndGame(bool isGoodEnding)
    {
        // Espera até que o TypewriterEffect termine
        while (typewriter.IsDialogActive())
        {
            yield return null;
        }

        if (isGoodEnding)
            gameController.GoodEnding();
        else
            gameController.GameOver();
    }


    private void CheckGameState()
    {
        if (gameRules == null || gameController == null || typewriter == null)
            return;

        string result = gameRules.checkGame();

        switch (result)
        {
            case "gameOver":
                isEndGameTriggered = true;
                typewriter.PlayDialog(GameOverDialog); // ou outro diálogo de fim ruim
                // musicManager.StopGameMusic(); // Toca música de fim de jogo
                StartCoroutine(WaitDialogThenEndGame(false));
                break;

            case "goodEnding":
                isEndGameTriggered = true;
                typewriter.PlayDialog(GoodEndingDialog); // ou outro diálogo de fim bom
                // musicManager.StopGameMusic(); // Toca música de fim de jogo
                StartCoroutine(WaitDialogThenEndGame(true)); // true = final bom
                break;

            case "continue":
            default:
                // Apenas se não houver fim de jogo, executa refill
                RefillDosesIfNoEnd();
                break;
        }
    }

    public void ForceInitialRefill()
    {
        isEndGameTriggered = false; // Garante que não está em estado final
        statsManager?.ResetDoses(); // Reseta doses antes de recarregar
    }



}
