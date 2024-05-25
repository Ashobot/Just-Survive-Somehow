using UnityEngine;
using EZCameraShake;

public class ExplosionScript : MonoBehaviour
{
    [SerializeField] AudioClip[] _explosionSounds;
    [SerializeField] float _maxSoundVolume;
    [Space]
    [SerializeField] float _camShakeMagnitude;
    [SerializeField] float _camShakeRoughness;
    [SerializeField] float _camShakeFadeInTime;
    [SerializeField] float _camShakeFadeOutTime;

    private void Start()
    {
        // Play sound
        SoundManager.instance.PlayRandomSound(_explosionSounds, transform, _maxSoundVolume, false, 0f);

        // Camera shake
        CameraShaker.Instance.ShakeOnce(_camShakeMagnitude, _camShakeRoughness, _camShakeFadeInTime, _camShakeFadeOutTime);
    }
}