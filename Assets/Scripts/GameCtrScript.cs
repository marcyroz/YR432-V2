using UnityEngine;

public class GameCtrScript : MonoBehaviour
{
    [SerializeField] private TypewriterEffect typewriter;
    [SerializeField] private AudioClip clickAudioClip;
    [SerializeField] private GameOverScreen gameOverScreen;
    [SerializeField] private MusicManager musicManager; // 🆕 Adicione isso!

    void Start()
    {
        typewriter.ActivateDialog(); // Inicia diálogo externamente
        // GameOver();
    }

    public void PlaySound()
    {
        SoundFXManager.instance.PlayClickSound(clickAudioClip, transform, 1f);
    }

    public void GameOver()
    {
        gameOverScreen.Setup();
        musicManager.PlayGameOverMusic(); // 🆕 Troca música aqui
    }
}
