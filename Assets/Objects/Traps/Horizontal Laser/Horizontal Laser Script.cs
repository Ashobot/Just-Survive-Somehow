using UnityEngine;
using System;
using UltimateAttributesPack;

public class HorizontalLaserScript : MonoBehaviour
{
    GameManager _gameManager;
    HorizontalLaserParams _trapParams;

    [SerializeField] GameObject _laserObject;

    float _startX;
    float _endX;
    float _currentMovementTime;
    float _currentMovementTimer;

    private void Awake()
    {
        // Get Game Manager
        if (GameObject.Find("Game Manager").TryGetComponent<GameManager>(out GameManager gm))
            _gameManager = gm;
        else
            Debug.LogWarning("Game Manager not found");
    }

    private void Start()
    {
        InitializeTrapParams();
        SetPositions();
        ActivateObject();
    }

    private void Update()
    {
        if (_currentMovementTimer < _currentMovementTime)
        {
            transform.position = new Vector2(Mathf.Lerp(_startX, _endX, _trapParams.MovementCurve.Evaluate(_currentMovementTimer / _currentMovementTime)), 0);

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
        _currentMovementTime = Mathf.Lerp(_trapParams.MinMovementTimeStart, _trapParams.MaxMovementTimeEnd, currentWavePercent);
    }

    void SetPositions()
    {
        // Calculate start and end positions
        float rand = UnityEngine.Random.value;
        _startX = rand >= 0.5f ? _gameManager.ArenaManager.WallLeft.transform.position.x - _trapParams.SpawnOffset : _gameManager.ArenaManager.WallRight.transform.position.x + _trapParams.SpawnOffset;
        _endX = rand >= 0.5f ? _gameManager.ArenaManager.WallRight.transform.position.x + _trapParams.SpawnOffset : _gameManager.ArenaManager.WallLeft.transform.position.x - _trapParams.SpawnOffset;

        // Set laser position
        transform.position = new Vector2(_startX, 0);
        _laserObject.transform.position = new Vector2(_startX, 0);

        // Set laser scale
        float arenaHeight = Vector3.Distance(_gameManager.ArenaManager.WallDown.transform.position, _gameManager.ArenaManager.WallUp.transform.position);
        _laserObject.transform.localScale = new Vector2(_trapParams.LaserWidth, arenaHeight);
    }

    void ActivateObject()
    {
        _laserObject.SetActive(true);
    }

    void DestroyTrap()
    {
        Destroy(gameObject);
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
    [MinValue(0)] public float MinMovementTimeStart;
    [MinValue(0)] public float MaxMovementTimeEnd;
}