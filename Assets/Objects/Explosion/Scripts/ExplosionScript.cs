using UnityEngine;
using EZCameraShake;

public class ExplosionScript : MonoBehaviour
{
    [SerializeField] float _camShakeMagnitude;
    [SerializeField] float _camShakeRoughness;
    [SerializeField] float _camShakeFadeInTime;
    [SerializeField] float _camShakeFadeOutTime;

    private void Start()
    {
        CameraShaker.Instance.ShakeOnce(_camShakeMagnitude, _camShakeRoughness, _camShakeFadeInTime, _camShakeFadeOutTime);
    }
}