using UnityEngine;
using System;
using UltimateAttributesPack;

public class BearTrapScript : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField, ReadOnly] BearTrapParams _trapParams;
    [Space]
    [SerializeField] GameObject _shadowPrefab;
    bool _atGround;
    bool _deployed;
    float _fallTimer;
    float _deployTimer;
    float _destroyTimer;
    float _currentFallDuration;
    float _currentDeployDuration;
    float _currentDestroyDuration;
    GameObject _shadow;

    private void Awake()
    {
        // Get Game Manager
        if (GameObject.Find("Game Manager").TryGetComponent<GameManager>(out GameManager gm))
            _gameManager = gm;
        else
            Debug.Log("Game Manager not found");
    }

    private void Start()
    {
        // Set current trap params at start
        InitializeTrapParams();

        _shadow = GameObject.Instantiate(_shadowPrefab, transform.position, Quaternion.identity); // Instantiate a shadow prefab on the spawn point
        transform.position = new Vector2(transform.position.x, transform.position.y + _trapParams.SpawnYOffset); // Set the trap on the position + yOffset
    }

    private void Update()
    {
        // is falling
        if (!_atGround)
        {
            if(_fallTimer < _currentFallDuration)
            {
                // Lerp position drom up to down
                transform.position = Vector2.Lerp(transform.position, _shadow.transform.position, _trapParams.FallCurve.Evaluate(_fallTimer / _currentFallDuration));
                _fallTimer += Time.deltaTime;

                // Lerp scale of shadow
                _shadow.transform.localScale = Vector2.Lerp(Vector2.zero, transform.localScale, _trapParams.FallCurve.Evaluate(_fallTimer / _currentFallDuration));
            }
            else
            {
                transform.position = _shadow.transform.position;
                DestroyShadow();
                _atGround = true;
            }
        }
        // Is deploying
        else if(_atGround && !_deployed)
        {
            if (_deployTimer < _currentDeployDuration)
            {
                _deployTimer += Time.deltaTime;
            }
            else
            {
                _deployed = true;
            }
        }
        // Trap armed
        else
        {
            if (_destroyTimer < _currentDestroyDuration)
            {
                _destroyTimer += Time.deltaTime;
            }
            else
            {
                DestroyTrap();
            }
        }
    }

    public void InitializeTrapParams()
    {
        float currentWavePercent = _gameManager.GameLoopManager.CurrentWavePercent;

        _trapParams = _gameManager.TrapsManager.CurrentBearTrapsParams;
        _currentFallDuration = Mathf.Lerp(_trapParams.FallDurationMin, _trapParams.FallDurationMax, currentWavePercent);
        _currentDeployDuration = Mathf.Lerp(_trapParams.DeployDurationMin, _trapParams.DeployDurationMax, currentWavePercent);
        _currentDestroyDuration = Mathf.Lerp(_trapParams.DestroyDurationMin, _trapParams.DestroyDurationMax, currentWavePercent);
    }

    void DestroyShadow()
    {
        Destroy(_shadow);
    }

    void DestroyTrap()
    {
        Destroy(gameObject);
    }
}

[Serializable]
public class BearTrapParams
{
    [AssetPreview] public GameObject Prefab;
    public SpawnType SpawnType;
    public float SpawnYOffset;
    public AnimationCurve FallCurve;
    [Space]
    [MinValue(0)] public float FallDurationMin;
    [MinValue(0)] public float FallDurationMax;
    [Space]
    [MinValue(0)] public float DeployDurationMin;
    [MinValue(0)] public float DeployDurationMax;
    [Space]
    [MinValue(0)] public float DestroyDurationMin;
    [MinValue(0)] public float DestroyDurationMax;
}
