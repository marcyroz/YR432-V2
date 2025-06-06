using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCtrScript : MonoBehaviour
{
    [SerializeField] private CountdownScript countdown;
    [SerializeField] private TypewriterEffect typewriter;
    [SerializeField] private AudioClip clickAudioClip;
    [SerializeField] private GameOverManager gameOverManager;
    [SerializeField] private GoodEndingManager goodEndingManager;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private CellSpawnerScript cellSpawner;
    [SerializeField] private StatsManagerScript statsManager;
    [SerializeField] private DialogData introDialog;

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !typewriter.IsDialogActive())
        {
            SceneManager.LoadScene("MenuScene");
        }

    }

    public void PlaySound()
    {
        SoundFXManager.instance.PlayClickSound(clickAudioClip, transform, 1f);
    }

    public void GameOver()
    {
        gameOverManager.StartGameOverCycle();
        musicManager.PlayGameOverMusic();
        countdown.StopCountdownCycle();
        cellSpawner.ResetSpawning();
    }

    public void GoodEnding()
    {
        goodEndingManager.StartGoodEndingCycle();
        musicManager.PlayGoodEndingMusic();
        countdown.StopCountdownCycle();
        cellSpawner.ResetSpawning();
    }

    public void PauseSpawns()
    {
        cellSpawner.StopSpawning();
    }

    public void ResumeSpawns()
    {
        cellSpawner.StartSpawning();
    }

    private IEnumerator StartSequence()
    {
        typewriter.PlayDialog(introDialog);

        // Espera o diálogo de introdução acabar
        while (typewriter.IsDialogActive())
            yield return null;

        statsManager.ResetDoses(); // reset após o diálogo, se preferir
        cellSpawner.StartSpawning();
        countdown.StartCountdownCycle();
        countdown.ForceInitialRefill(); // agora sim
    }

    public void StartGame()
    {
        CellScript.countBoard = Object.FindFirstObjectByType<CountBoardScript>();
        StartCoroutine(StartSequence());
        // statsManager.ResetDoses();
        // cellSpawner.StartSpawning();
    }

    public void RestartGame()
    {
        GameStatsTracker.Instance?.ResetStats();
        countdown.StopCountdownCycle();
        gameOverManager.EndGameOverCycle();
        goodEndingManager.EndGoodEndingCycle();
        cellSpawner.ResetSpawning();
        CellScript.countBoard.ResetCounts();
        cellSpawner.ResetStats();
        musicManager.PlayGameMusic();

        StartGame();

    }
}
