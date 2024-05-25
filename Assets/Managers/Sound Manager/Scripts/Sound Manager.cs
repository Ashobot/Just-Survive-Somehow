using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Range(0, 1)] public float currentVolume;
    [SerializeField] Transform _soundParent;
    [SerializeField] AudioSource _soundObject;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public AudioSource PlaySound(AudioClip clip, Transform spawnTransform, float maxVolume, bool changeSpeed = false, float wantedClipTime = 0f, bool looped = false)
    {
        // Instanciate sound object
        AudioSource audioSource = Instantiate(_soundObject, spawnTransform.position, Quaternion.identity);

        // Change speed of sound if necessary
        if (changeSpeed)
            audioSource.pitch = clip.length / wantedClipTime;

        // Set looped
        if(looped)
            audioSource.loop = true;

        audioSource.transform.parent = _soundParent; // Set object parent
        audioSource.gameObject.name = clip.name; // Set object name
        audioSource.clip = clip; // Set clip
        audioSource.volume = maxVolume * currentVolume; // Set volume with game volume
        audioSource.Play(); // Play sound
        float clipLenght = audioSource.clip.length;

        // Destroy object when sound finished playing
        if(!looped)
            Destroy(audioSource.gameObject, clipLenght);

        return audioSource;
    }

    public AudioSource PlayRandomSound(AudioClip[] clips, Transform spawnTransform, float maxVolume, bool changeSpeed = false, float wantedClipTime = 0f, bool looped = false)
    {        
        // Instanciate random sound object
        int randIndex = Random.Range(0, clips.Length);
        AudioSource audioSource = Instantiate(_soundObject, spawnTransform.position, Quaternion.identity);

        // Change speed of sound if necessary
        if (changeSpeed)
            audioSource.pitch = clips[randIndex].length / wantedClipTime;

        audioSource.transform.parent = _soundParent; // Set object parent
        audioSource.gameObject.name = clips[randIndex].name; // Set object name
        audioSource.clip = clips[randIndex]; // Set clip
        audioSource.volume = maxVolume * currentVolume; // Set volume with game volume
        audioSource.Play(); // Play sound
        float clipLenght = audioSource.clip.length;

        // Destroy object when sound finished playing
        if (!looped)
            Destroy(audioSource.gameObject, clipLenght);

        return audioSource;
    }

    public void SetSoundsPause(bool state)
    {
        foreach(AudioSource audio in _soundParent.GetComponentsInChildren<AudioSource>())
        {
            if(state)
                audio.Pause();
            else
                audio.UnPause();
        }
    }
}