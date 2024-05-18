using UnityEngine;
using System;
using UltimateAttributesPack;

public class HorizontalLaserScript : MonoBehaviour
{
    GameManager _gameManager;
    PlayerController _playerController;
    HorizontalLaserParams _trapParams;
    SpriteRenderer _spriteRenderer;

    [SerializeField] LayerMask _playerLayerMask;

    bool _playerOn;
    float _startX;
    float _endX;
    float _currentMovementTime;
    float _currentMovementTimer;

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
        if (_currentMovementTimer < _currentMovementTime)
        {
            transform.position = new Vector2(Mathf.Lerp(_startX, _endX, _trapParams.MovementCurve.Evaluate(_currentMovementTimer / _currentMovementTime)), 0);

            DamageToPlayer();

            _currentMovementTimer += Time.deltaTime;
        }
        else
        {
            transform.position = new Vector2(_endX, 0);
            DestroyTrap();
        }
    }

    public void InitializeTrapParams()
    {
        float currentWavePercent = _gameManager.GameLoopManager.CurrentWavePercent;

        _trapParams = _gameManager.TrapsManager.CurrentHorizontalLaserParams;
        _currentMovementTime = Mathf.Lerp(_trapParams.MovementTimeStart, _trapParams.MovementTimeEnd, currentWavePercent);
    }

    void SetPositions()
    {
        // Calculate start and end positions
        float rand = UnityEngine.Random.value;
        _startX = rand >= 0.5f ? _gameManager.ArenaManager.WallLeft.transform.position.x - _trapParams.SpawnOffset : _gameManager.ArenaManager.WallRight.transform.position.x + _trapParams.SpawnOffset;
        _endX = rand >= 0.5f ? _gameManager.ArenaManager.WallRight.transform.position.x + _trapParams.SpawnOffset : _gameManager.ArenaManager.WallLeft.transform.position.x - _trapParams.SpawnOffset;

        // Set laser position
        transform.position = new Vector2(_startX, 0);

        // Set laser scale
        float arenaHeight = Vector3.Distance(_gameManager.ArenaManager.WallDown.transform.position, _gameManager.ArenaManager.WallUp.transform.position);
        transform.localScale = new Vector2(_trapParams.LaserWidth, arenaHeight);
    }

    void DamageToPlayer()
    {
        if (_playerOn && _playerController.PlayerTrigger.CanTakeDamage)
        {
            _playerController.PlayerTrigger.SetDamage();

            // Animation
        }
    }

    void ActivateObject()
    {
        _spriteRenderer.enabled = true;
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
public class HorizontalLaserParams
{
    [MinValue(0)] public float SpawnOffset;
    [MinValue(0)] public float LaserWidth;
    public AnimationCurve MovementCurve;
    [Space]
    [MinValue(0)] public float MovementTimeStart;
    [MinValue(0)] public float MovementTimeEnd;
}