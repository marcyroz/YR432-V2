using UnityEngine;

public class GameCtrScript : MonoBehaviour
{
    [SerializeField] private TypewriterEffect typewriter;
    [SerializeField] private AudioClip clickAudioClip;
    [SerializeField] private GameOverScreen gameOverScreen;
    [SerializeField] private MusicManager musicManager;

    [SerializeField] private CellSpawnerScript cellSpawner; // 🎯 Referência ao spawner

    void Start()
    {
        typewriter.OnDialogFinished.AddListener(OnDialogComplete);

        typewriter.ActivateDialog(); // Inicia o diálogo
                                     // cellSpawner.StartSpawning(); — REMOVIDO daqui!
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
        gameOverScreen.Setup();
        musicManager.PlayGameOverMusic();

        cellSpawner.StopSpawning(); // Para spawns no game over
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
