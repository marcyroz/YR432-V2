using UnityEngine;

public class GameCtrScript : MonoBehaviour
{

    [SerializeField] TypewriterEffect typewriter;
    [SerializeField] private AudioClip clickAudioClip; // Reference to the audio clip for the click sound


    void Start()
    {
        typewriter.ActivateDialog(); // Inicia diálogo externamente
    }

    public void PlaySound()
    {
        SoundFXManager.instance.PlayClickSound(clickAudioClip, transform, 1f);
    }
}


