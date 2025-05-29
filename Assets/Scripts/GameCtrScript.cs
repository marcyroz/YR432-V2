using UnityEngine;

public class GameCtrScript : MonoBehaviour
{
    [SerializeField] private TypewriterEffect typewriter;
    [SerializeField] private AudioClip clickAudioClip;
    [SerializeField] private GameOverManager gameOverManager;
    [SerializeField] private GoodEndingManager goodEndingManager;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private CellSpawnerScript cellSpawner; // 🎯 Referência ao spawner

    void Start()
    {
        CellScript.countBoard = Object.FindFirstObjectByType<CountBoardScript>();
        typewriter.OnDialogFinished.AddListener(OnDialogComplete);
        typewriter.ActivateDialog(); // Inicia o diálogo
    }

    private void OnDialogComplete()
    {
        cellSpawner.StartSpawning();
        Debug.Log("Spawns iniciados após o diálogo.");
    }


    public void PlaySound()
    {
        SoundFXManager.instance.PlayClickSound(clickAudioClip, transform, 1f);
    }

    public void GameOver()
    {
        // gameOverScreen.Setup();
        gameOverManager.StartGameOverCycle();
        musicManager.PlayGameOverMusic();

        cellSpawner.StopSpawning();
    }

    public void GoodEnding()
    {
        goodEndingManager.StartGoodEndingCycle();
        musicManager.PlayGoodEndingMusic();
        cellSpawner.StopSpawning();
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
        cellSpawner.ResetSpawning();
        // Você também pode reiniciar HUD, tempo, pontuação aqui
    }

}
