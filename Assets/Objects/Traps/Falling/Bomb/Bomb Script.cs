using System;
using UnityEngine;
using UltimateAttributesPack;

public class BombScript : MonoBehaviour
{
    GameManager _gameManager;
    BombParams _trapParams;
    CircleCollider2D _collider;

    [SerializeField] GameObject _shadowObject;
    [SerializeField] GameObject _bombObject;
    [SerializeField] GameObject _deathZonePreview;
    [SerializeField] GameObject _deathZone;

    Vector2 _startPosition;
    bool _atGround;
    bool _exploded;
    float _fallTimer;
    float _explodeTimer;
    float _explosionTimer;
    float _currentFallDuration;
    float _currentExplodeDuration;
    float _currentExplosionDuration;

    private void Awake()
    {
        // Get Game Manager
        _gameManager = FindObjectOfType<GameManager>();

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
            if (_fallTimer < _currentFallDuration)
            {
                // Lerp position drom up to down
                _bombObject.transform.position = Vector2.Lerp(_startPosition, _shadowObject.transform.position, _trapParams.FallCurve.Evaluate(_fallTimer / _currentFallDuration));

                // Lerp scale of trap
                _bombObject.transform.localScale = Vector2.Lerp(Vector2.zero, transform.localScale, _trapParams.FallCurve.Evaluate(_fallTimer / _currentFallDuration));

                // Lerp scale of shadow
                _shadowObject.transform.localScale = Vector2.Lerp(Vector2.zero, transform.localScale, _trapParams.FallCurve.Evaluate(_fallTimer / _currentFallDuration));

                _fallTimer += Time.deltaTime;
            }
            else
            {
                _bombObject.transform.position = _shadowObject.transform.position;
                _shadowObject.SetActive(false);
                _deathZonePreview.SetActive(true);
                _collider.isTrigger = false;
                _atGround = true;
            }
        }
        // Bomb explosion timer
        else if (_atGround && !_exploded)
        {
            if (_explodeTimer < _currentExplodeDuration)
                _explodeTimer += Time.deltaTime;
            else
            {
                //Explosion animation

                _deathZone.SetActive(true);
                _deathZonePreview.GetComponent<SpriteRenderer>().color = Color.red;
                _exploded = true;
            }
        }
        // Bomb explosion
        else
        {
            if (_explosionTimer < _currentExplosionDuration)
                _explosionTimer += Time.deltaTime;
            else
            {
                DestroyTrap();
            }
        }
    }

    public void InitializeTrapParams()
    {
        float currentWavePercent = _gameManager.GameLoopManager.CurrentWavePercent;

        _trapParams = _gameManager.TrapsManager.CurrentBombParams;
        _deathZone.transform.localScale = new Vector2(_trapParams.ExplosionRange, _trapParams.ExplosionRange);
        _deathZonePreview.transform.localScale = new Vector2(_trapParams.ExplosionRange, _trapParams.ExplosionRange);
        _currentFallDuration = Mathf.Lerp(_trapParams.FallDurationStart, _trapParams.FallDurationEnd, currentWavePercent);
        _currentExplodeDuration = Mathf.Lerp(_trapParams.ExplodeDurationStart, _trapParams.ExplodeDurationEnd, currentWavePercent);
        _currentExplosionDuration = Mathf.Lerp(_trapParams.ExplosionDurationStart, _trapParams.ExplosionDurationEnd, currentWavePercent);
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
        _bombObject.transform.position = _startPosition;

        // Set trap rotation to random
        _bombObject.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0f, 360f));
    }

    void ActivateObject()
    {
        _shadowObject.SetActive(true);
        _bombObject.SetActive(true);
    }

    void DestroyTrap()
    {
        Destroy(gameObject);
    }
}

// ----- Serializable classes ----- //

[Serializable]
public class BombParams
{
    [MinValue(0)] public float SpawnYOffset;
    [MinValue(0)] public float SpawnRadius;
    [MinValue(0)] public float ExplosionRange;
    public AnimationCurve FallCurve;
    [Space]
    [MinValue(0)] public float FallDurationStart;
    [MinValue(0)] public float FallDurationEnd;
    [Space]
    [MinValue(0)] public float ExplodeDurationStart;
    [MinValue(0)] public float ExplodeDurationEnd;
    [Space]
    [MinValue(0)] public float ExplosionDurationStart;
    [MinValue(0)] public float ExplosionDurationEnd;
}
