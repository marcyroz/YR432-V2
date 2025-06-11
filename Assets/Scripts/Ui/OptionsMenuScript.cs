using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenuScript : MonoBehaviour
{

    public AudioMixer musicMixer;
    public AudioMixer FXMixer;

    public void SetMusicVolume(float volume)
    {
        musicMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }
    public void SetSoundFXVolume(float volume)
    {
        FXMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }
}
