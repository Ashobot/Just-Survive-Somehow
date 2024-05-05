using UnityEngine;
using System;
using UltimateAttributesPack;

public class BearTrapScript : MonoBehaviour
{
    GameManager _gameManager;
    BearTrapParams _trapParams;
    CircleCollider2D _collider;

    [SerializeField] GameObject _shadowObject;
    [SerializeField] GameObject _trapObject;
    [SerializeField] GameObject _deathZone;

    Vector2 _startPosition;
    bool _atGround;
    bool _deployed;
    float _fallTimer;
    float _deployTimer;
    float _destroyTimer;
    float _currentSpawnYOffset;
    float _currentSpawnRadius;
    float _currentFallDuration;
    float _currentDeployDuration;
    float _currentDestroyDuration;

    private void Awake()
    {
        // Get Game Manager
        if (GameObject.Find("Game Manager").TryGetComponent<GameManager>(out GameManager gm))
            _gameManager = gm;
        else
            Debug.LogWarning("Game Manager not found");

        _collider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        InitializeTrapParams(); // Set current trap params with current wave percent (difficulty)
        SetPositions(); // Set the object position and rotation at random
        ActivateObject(); // Activate objects
    }

    private void Update()
    {
        // Is falling
        if (!_atGround)
        {
            if(_fallTimer < _currentFallDuration)
            {
                // Lerp position drom up to down
                _trapObject.transform.position = Vector2.Lerp(_startPosition, _shadowObject.transform.position, _trapParams.FallCurve.Evaluate(_fallTimer / _currentFallDuration));

                // Lerp scale of trap
                _trapObject.transform.localScale = Vector2.Lerp(Vector2.zero, transform.localScale, _trapParams.FallCurve.Evaluate(_fallTimer / _currentFallDuration));

                // Lerp scale of shadow
                _shadowObject.transform.localScale = Vector2.Lerp(Vector2.zero, transform.localScale, _trapParams.FallCurve.Evaluate(_fallTimer / _currentFallDuration));

                _fallTimer += Time.deltaTime;
            }
            else
            {
                _trapObject.transform.position = _shadowObject.transform.position;
                DeactivateShadow();
                _collider.isTrigger = false;
                _atGround = true;
            }
        }
        // Is deploying
        else if(_atGround && !_deployed)
        {
            if (_deployTimer < _currentDeployDuration)
                _deployTimer += Time.deltaTime;
            else
            {
                _deathZone.SetActive(true);
                _deployed = true;
            }
        }
        // Trap armed
        else
        {
            if (_destroyTimer < _currentDestroyDuration)
                _destroyTimer += Time.deltaTime;
            else
                DestroyTrap();
        }
    }

    public void InitializeTrapParams()
    {
        float currentWavePercent = _gameManager.GameLoopManager.CurrentWavePercent;

        _trapParams = _gameManager.TrapsManager.CurrentBearTrapsParams;
        _currentFallDuration = Mathf.Lerp(_trapParams.FallDurationStart, _trapParams.FallDurationEnd, currentWavePercent);
        _currentDeployDuration = Mathf.Lerp(_trapParams.DeployDurationStart, _trapParams.DeployDurationEnd, currentWavePercent);
        _currentDestroyDuration = Mathf.Lerp(_trapParams.DestroyDurationStart, _trapParams.DestroyDurationEnd, currentWavePercent);
    }

    void SetPositions()
    {
        // Calculate random position around player
        Vector2 playerPosition = _gameManager.PlayerController.transform.position;
        Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        float randomRange = UnityEngine.Random.Range(0f, _trapParams.SpawnRadius);
        Vector2 randomSpawnPosition = playerPosition + (randomDirection * randomRange);
        randomSpawnPosition.x = Mathf.Clamp(randomSpawnPosition.x, _gameManager.ArenaManager.WallLeft.transform.position.x + 1, _gameManager.ArenaManager.WallRight.transform.position.x - 1);
        randomSpawnPosition.y = Mathf.Clamp(randomSpawnPosition.y, _gameManager.ArenaManager.WallDown.transform.position.y + 1, _gameManager.ArenaManager.WallUp.transform.position.y - 1);

        // Set shadow position to random
        transform.position = randomSpawnPosition;
        _shadowObject.transform.position = randomSpawnPosition;

        // Set trap position to up offset
        _startPosition = new Vector3(randomSpawnPosition.x, randomSpawnPosition.y + _trapParams.SpawnYOffset);
        _trapObject.transform.position = _startPosition;

        // Set trap rotation to random
        _trapObject.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0f, 360f));
    }

    void ActivateObject()
    {
        _shadowObject.SetActive(true);
        _trapObject.SetActive(true);
    }

    void DeactivateShadow()
    {
        _shadowObject.SetActive(false);
    }

    void DestroyTrap()
    {
        Destroy(gameObject);
    }
}

// ----- Serializable classes ----- //

[Serializable]
public class BearTrapParams
{
    [MinValue(0)] public float SpawnYOffset;
    [MinValue(0)] public float SpawnRadius;
    public AnimationCurve FallCurve;
    [Space]
    [MinValue(0)] public float FallDurationStart;
    [MinValue(0)] public float FallDurationEnd;
    [Space]
    [MinValue(0)] public float DeployDurationStart;
    [MinValue(0)] public float DeployDurationEnd;
    [Space]
    [MinValue(0)] public float DestroyDurationStart;
    [MinValue(0)] public float DestroyDurationEnd;
}
