using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    //transformando a classe em singleton
    public static SoundFXManager instance;
    [SerializeField] private AudioSource clickAudioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayClickSound(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //spawn in gameObject
        AudioSource audioSource = Instantiate(clickAudioSource, spawnTransform.position, Quaternion.identity);

        //Assign the audio clip to the audio source
        audioSource.clip = audioClip;

        //Assign the volume of the audio source
        audioSource.volume = volume;

        // Play the sound effect here
        audioSource.Play();

        // get the length of the audio clip
        float clipLength = audioSource.clip.length;

        // Destroy the audio source after the clip has finished playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
