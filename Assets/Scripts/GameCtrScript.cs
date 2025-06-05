using UnityEngine;

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

    void Start()
    {
        CellScript.countBoard = Object.FindFirstObjectByType<CountBoardScript>();
        typewriter.OnDialogFinished.AddListener(OnDialogComplete);
        typewriter.ActivateDialog();
        statsManager.ResetDoses();
    }

    private void OnDialogComplete()
    {
        cellSpawner.StartSpawning();
        countdown.StartCountdownCycle();
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

    public void RestartGame()
    {
        countdown.StopCountdownCycle();
        cellSpawner.StopSpawning();
        gameOverManager.EndGameOverCycle();
        goodEndingManager.EndGoodEndingCycle();

        cellSpawner.ResetSpawning();
        CellScript.countBoard.ResetCounts();
        typewriter.RestartDialog();
        typewriter.OnDialogFinished.RemoveAllListeners();
        typewriter.OnDialogFinished.AddListener(OnDialogComplete);
        musicManager.PlayGameMusic();

        if (statsManager != null)
            statsManager.ResetDoses();

        Debug.Log("Jogo reiniciado com sucesso.");
    }
}
