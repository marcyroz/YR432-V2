using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenuScript : MonoBehaviour
{

    public AudioMixer audioMixer;
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }
}
