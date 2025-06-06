using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip goodEndingMusic;

    public void PlayGameMusic()
    {
        if (musicSource.clip != gameMusic)
        {
            musicSource.clip = gameMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void StopGameMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void PlayGameOverMusic()
    {
        if (musicSource.clip != gameOverMusic)
        {
            musicSource.clip = gameOverMusic;
            musicSource.loop = false;
            musicSource.Play();
        }
    }
    public void PlayGoodEndingMusic()
    {
        if (musicSource.clip != goodEndingMusic)
        {
            musicSource.clip = goodEndingMusic;
            musicSource.loop = false;
            musicSource.Play();
        }
    }
}
