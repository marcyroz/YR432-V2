using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip gameOverMusic;

    public void PlayGameMusic()
    {
        if (musicSource.clip != gameMusic)
        {
            musicSource.clip = gameMusic;
            musicSource.Play();
        }
    }

    public void PlayGameOverMusic()
    {
        if (musicSource.clip != gameOverMusic)
        {
            musicSource.clip = gameOverMusic;
            musicSource.Play();
        }
    }
}
