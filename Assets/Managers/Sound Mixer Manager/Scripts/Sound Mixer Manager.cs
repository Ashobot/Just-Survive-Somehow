using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;

    private void Awake()
    {
        InitializeVolume();
    }

    void InitializeVolume()
    {
        if (PlayerPrefs.HasKey("Master Volume"))
            _audioMixer.SetFloat("Master Volume", Mathf.Log10(PlayerPrefs.GetFloat("Master Volume")) * 20f);

        if (PlayerPrefs.HasKey("Music Volume"))
            _audioMixer.SetFloat("Music Volume", Mathf.Log10(PlayerPrefs.GetFloat("Music Volume")) * 20f);

        if (PlayerPrefs.HasKey("Sound FX Volume"))
            _audioMixer.SetFloat("FX Volume", Mathf.Log10(PlayerPrefs.GetFloat("Sound FX Volume")) * 20f);
    } 

    public void SetMasterVolume(float level)
    {
        _audioMixer.SetFloat("Master Volume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("Master Volume", level);
    }

    public void SetMusicVolume(float level)
    {
        _audioMixer.SetFloat("Music Volume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("Music Volume", level);
    }

    public void SetFXVolume(float level)
    {
        _audioMixer.SetFloat("FX Volume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("Sound FX Volume", level);
    }
}