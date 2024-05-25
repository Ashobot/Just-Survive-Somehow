using System;
using UnityEngine;
using UltimateAttributesPack;

public class BombScript : MonoBehaviour
{
    GameManager _gameManager;
    PlayerController _playerController;
    BombParamsObject _trapParams;
    CircleCollider2D _collider;
    SpriteRenderer _bombSpriteRenderer;
    SpriteRenderer _previewZoneSpriteRenderer;

    [SerializeField] LayerMask _playerLayerMask;
    [Space]
    [SerializeField] GameObject _shadowObject;
    [SerializeField] GameObject _bombObject;
    [SerializeField] GameObject _deathZonePreview;
    [SerializeField] GameObject _deathZone;
    [SerializeField] Color _lerpToColor;
    [SerializeField] Color _previewZoneLerpToColor;
    [SerializeField] Vector2 _lerpToScale;
    [Space]
    [SerializeField] GameObject _explosionPrefab;
    [Space]
    [LineTitle("Sounds")]
    [SerializeField] float _fallingSoundMaxVolume;
    [SerializeField] AudioClip _fallingSound;
    [Space]
    [SerializeField] float _fuseSoundMaxVolume;
    [SerializeField] AudioClip _fuseSound;
    AudioSource _fuseSoundAudioSource;

    Vector2 _startPosition;
    bool _playerOn;
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
        _playerController = _gameManager.PlayerController;

        _collider = GetComponent<CircleCollider2D>();
        _bombSpriteRenderer = _bombObject.GetComponent<SpriteRenderer>();
        _previewZoneSpriteRenderer = _deathZonePreview.GetComponent<SpriteRenderer>();

        InitializeTrapParams(); // Set current trap params with current wave percent (difficulty)
        SetPositions(); // Set the object position and rotation at random
    }

    private void Start()
    {      
        ActivateObject(); // Activate objects

        // Play falling sound
        SoundManager.instance.PlaySound(_fallingSound, transform, _fallingSoundMaxVolume, true, _currentFallDuration);
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
                // Play fuse sound
                _fuseSoundAudioSource = SoundManager.instance.PlaySound(_fuseSound, transform, _fuseSoundMaxVolume, looped : true);

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
            {
                // Lerp color of the bomb (from normal to red)
                _bombSpriteRenderer.color = Color.Lerp(Color.white, _lerpToColor, _explodeTimer / _currentExplodeDuration);
                _previewZoneSpriteRenderer.color = Color.Lerp(Color.white, _previewZoneLerpToColor, _explodeTimer / _currentExplodeDuration);

                // Lerp scale of the bomb
                _bombObject.transform.localScale = Vector3.Lerp(transform.localScale, _lerpToScale, _explodeTimer / _currentExplodeDuration);

                _explodeTimer += Time.deltaTime;
            }
            else
            {
                // Destroy fuse sound
                Destroy(_fuseSoundAudioSource.gameObject);

                _bombSpriteRenderer.color = _lerpToColor;
                _previewZoneSpriteRenderer.color = _previewZoneLerpToColor;

                // Instanciate explosion
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

                _deathZone.SetActive(true);
                _exploded = true;
            }
        }
        // Bomb explosion
        else
        {
            if (_explosionTimer < _currentExplosionDuration)
            {
                DamageToPlayer();

                _explosionTimer += Time.deltaTime;
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

        _trapParams = _gameManager.TrapsManager.CurrentBombParams.TrapParams;
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
    }

    void DamageToPlayer()
    {
        if (_playerOn && _playerController.PlayerTrigger.CanTakeDamage)
        {
            _playerController.PlayerTrigger.SetDamage();
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_playerLayerMask.value & (1 << collision.transform.gameObject.layer)) > 0)
            _playerOn = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((_playerLayerMask.value & (1 << collision.transform.gameObject.layer)) > 0)
            _playerOn = false;
    }
}

// ----- Serializable classes ----- //

[Serializable]
public class BombParams
{
    public string WaveNumber;
    public BombParamsObject TrapParams;
}
