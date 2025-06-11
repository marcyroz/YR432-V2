using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private AudioClip clickAudioClip; // Reference to the audio clip for the click sound
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
        PlaySound();
    }

    public void PlaySound()
    {
        SoundFXManager.instance.PlayClickSound(clickAudioClip, transform, 1f);
    }
}
