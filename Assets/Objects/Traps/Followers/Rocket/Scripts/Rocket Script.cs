using System;
using UltimateAttributesPack;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
    GameManager _gameManager;
    PlayerController _playerController;
    RocketParamsObject _trapParams;

    [SerializeField] Transform _targetRotator;
    [SerializeField] LayerMask _playerLayerMask;
    [SerializeField] LayerMask _wallLayerMask;
    [Space]
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] GameObject _explosionPrefab;
    [Space]
    [LineTitle("Sounds")]
    [SerializeField] float _followingSoundMaxVolume;
    [SerializeField] AudioClip _followingSound;
    AudioSource _followingSoundSource;

    bool _destroyed = false;
    bool _onSpawnWall = true;
    float _currentMovementSpeed;
    float _currentRotationSpeed;

    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider;

    private void Awake()
    {
        // Get Game Manager
        _gameManager = FindObjectOfType<GameManager>();  
        _playerController = _gameManager.PlayerController;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();

        InitializeTrapParams();
        SetPositions();
    }

    private void Start()
    {
        ActivateObject();

        // Play following sound
        _followingSoundSource = SoundManager.instance.PlaySound(_followingSound, transform, _followingSoundMaxVolume, looped : true);
    }

    private void Update()
    {
        if (_destroyed)
        {
            if(_particleSystem == null)
                Destroy(gameObject);
        }
        else
        {           
            Move();
        }      
    }

    public void InitializeTrapParams()
    {
        float currentWavePercent = _gameManager.GameLoopManager.CurrentWavePercent;

        _trapParams = _gameManager.TrapsManager.CurrentRocketParams.TrapParams;
        _currentMovementSpeed = Mathf.Lerp(_trapParams.MovementSpeedStart, _trapParams.MovementSpeedEnd, currentWavePercent);
        _currentRotationSpeed = Mathf.Lerp(_trapParams.RotationSpeedStart, _trapParams.RotationSpeedEnd, currentWavePercent);
    }

    void SetPositions()
    {
        GameObject[] walls = {_gameManager.ArenaManager.WallLeft, _gameManager.ArenaManager.WallRight, _gameManager.ArenaManager.WallUp, _gameManager.ArenaManager.WallDown};
        int randWallIndex = UnityEngine.Random.Range(0, 4);
        Vector2 minRandPos, maxRandPos; 

        if(randWallIndex <= 1) // If it's left or right wall
        {
            minRandPos = new Vector2(walls[randWallIndex].transform.position.x + (randWallIndex == 0 ? -_trapParams.SpawnOffset : _trapParams.SpawnOffset), -walls[randWallIndex].transform.localScale.y / 2);
            maxRandPos = new Vector2(walls[randWallIndex].transform.position.x + (randWallIndex == 0 ? -_trapParams.SpawnOffset : _trapParams.SpawnOffset), walls[randWallIndex].transform.localScale.y / 2);
        }
        else // If it's up or down wall
        {
            minRandPos = new Vector2(-walls[randWallIndex].transform.localScale.x / 2, walls[randWallIndex].transform.position.y + (randWallIndex == 2 ? _trapParams.SpawnOffset : -_trapParams.SpawnOffset));
            maxRandPos = new Vector2(walls[randWallIndex].transform.localScale.x / 2, walls[randWallIndex].transform.position.y + (randWallIndex == 2 ? _trapParams.SpawnOffset : -_trapParams.SpawnOffset));
        }

        // Set rocket spawn position
        transform.position = Vector2.Lerp(minRandPos, maxRandPos, UnityEngine.Random.value);

        // Set rocket spawn rotation
        Vector2 lookPlayerDirection = _playerController.transform.position - transform.position;
        transform.up = lookPlayerDirection;
    }

    private void Move()
    {
        // Set target rotator rotation
        Vector3 direction = _playerController.transform.position - transform.position;
        _targetRotator.up = direction;

        // Calculate smoothed rotation and rotate the rocket
        Quaternion smoothedRotation = Quaternion.RotateTowards(transform.rotation, _targetRotator.rotation, _currentRotationSpeed * Time.deltaTime);
        smoothedRotation.x = 0;
        smoothedRotation.y = 0;
        transform.rotation = smoothedRotation;

        // Move rocket
        transform.position += transform.up * _currentMovementSpeed * Time.deltaTime;

        // Set sound position
        _followingSoundSource.transform.position = transform.position;
    }

    void ActivateObject()
    {
        _spriteRenderer.enabled = true;
        _targetRotator.gameObject.SetActive(true);
        _particleSystem.gameObject.SetActive(true);
    }

    void DestroyTrap()
    {
        Destroy(_followingSoundSource.gameObject);
        _boxCollider.enabled = false;
        _spriteRenderer.enabled = false;
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        _particleSystem.Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detect if collides with wall
        if ((_wallLayerMask.value & (1 << collision.transform.gameObject.layer)) > 0)
        {
            if (_onSpawnWall)
                _onSpawnWall = false;
            else
            {
                _destroyed = true;
                DestroyTrap();
            }
        }

        // Detect if collides with player
        if ((_playerLayerMask.value & (1 << collision.transform.gameObject.layer)) > 0 && _playerController.PlayerTrigger.CanTakeDamage)
        {
            _playerController.PlayerTrigger.SetDamage();
            _destroyed = true;
            DestroyTrap();
        }
    }
}

// ----- Serializable classes ----- //

[Serializable]
public class RocketParams
{
    public string WaveNumber;
    public RocketParamsObject TrapParams;
}