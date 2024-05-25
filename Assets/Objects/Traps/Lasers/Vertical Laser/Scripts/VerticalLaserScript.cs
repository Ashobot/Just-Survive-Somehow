using UnityEngine;
using System;

public class VerticalLaserScript : MonoBehaviour
{
    GameManager _gameManager;
    PlayerController _playerController;
    VerticalLaserParamsObject _trapParams;
    SpriteRenderer _spriteRenderer;

    SpriteRenderer _warningSpriteRenderer;
    Animator _warningAnimator;

    [SerializeField] LayerMask _playerLayerMask;
    [SerializeField] GameObject _warningPrefab;
    [SerializeField] float _warningOffset;
    [Space]
    [SerializeField] float _laserSoundMaxVolume;
    [SerializeField] AudioClip _laserSound;
    AudioSource _laserSoundAudioSource;

    GameObject _warningInstance;
    bool _warningFinished;
    bool _playerOn;
    float _startY;
    float _endY;
    float _currentMovementTime;
    float _currentMovementTimer;
    float _warningTimer;

    private void Awake()
    {
        // Get Game Manager
        _gameManager = FindObjectOfType<GameManager>();
        _playerController = _gameManager.PlayerController;

        _spriteRenderer = GetComponent<SpriteRenderer>();

        InitializeTrapParams();
        SetPositions();
    }

    private void Start()
    {
        ActivateObject();
    }

    private void Update()
    {
        if (!_warningFinished)
        {
            if (_warningTimer < _trapParams.WarningDuration)
            {
                _warningTimer += Time.deltaTime;
            }
            else
            {
                Destroy(_warningInstance);
                _warningFinished = true;

                // Play laser sound
                _laserSoundAudioSource = SoundManager.instance.PlaySound(_laserSound, transform, _laserSoundMaxVolume, looped: true);
            }
        }
        else
        {
            if (_currentMovementTimer < _currentMovementTime)
            {
                transform.position = new Vector2(0, Mathf.Lerp(_startY, _endY, _trapParams.MovementCurve.Evaluate(_currentMovementTimer / _currentMovementTime)));

                DamageToPlayer();

                _currentMovementTimer += Time.deltaTime;
            }
            else
            {
                transform.position = new Vector2(0, _endY);
                DestroyTrap();
            }
        }
    }

    public void InitializeTrapParams()
    {
        float currentWavePercent = _gameManager.GameLoopManager.CurrentWavePercent;

        _trapParams = _gameManager.TrapsManager.CurrentVerticalLaserParams.TrapParams;
        _currentMovementTime = Mathf.Lerp(_trapParams.MovementTimeStart, _trapParams.MovementTimeEnd, currentWavePercent);
    }

    void SetPositions()
    {
        // Calculate start and end positions
        float rand = UnityEngine.Random.value;
        _startY = rand >= 0.5f ? _gameManager.ArenaManager.WallUp.transform.position.y + _trapParams.SpawnOffset : _gameManager.ArenaManager.WallDown.transform.position.y - _trapParams.SpawnOffset;
        _endY = rand >= 0.5f ? _gameManager.ArenaManager.WallDown.transform.position.y - _trapParams.SpawnOffset : _gameManager.ArenaManager.WallUp.transform.position.y + _trapParams.SpawnOffset;

        // Instanciate warning
        Vector2 warningPosition = new Vector2(0f, (rand >= 0.5f ? _gameManager.ArenaManager.WallUp.transform.position.y - _warningOffset : _gameManager.ArenaManager.WallDown.transform.position.y + _warningOffset));
        _warningInstance = Instantiate(_warningPrefab, warningPosition, Quaternion.identity);
        _warningSpriteRenderer = _warningInstance.GetComponent<SpriteRenderer>();
        _warningAnimator = _warningInstance.GetComponent<Animator>();
        _warningAnimator.SetBool("IsUpDown", true); // Set warning animation

        // Set warning flipX
        _warningSpriteRenderer.flipY = rand >= 0.5f ? false : true;

        // Set laser position
        transform.position = new Vector2(0, _startY);

        // Set laser scale
        float arenaWidth = Vector3.Distance(_gameManager.ArenaManager.WallLeft.transform.position, _gameManager.ArenaManager.WallRight.transform.position);
        transform.localScale = new Vector2(arenaWidth, _trapParams.LaserWidth);
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
        _spriteRenderer.enabled = true;
        _warningInstance.SetActive(true);
    }

    void DestroyTrap()
    {
        Destroy(_laserSoundAudioSource.gameObject);
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
public class VerticalLaserParams
{
    public string WaveNumber;
    public VerticalLaserParamsObject TrapParams;
}