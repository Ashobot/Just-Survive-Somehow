using UnityEngine;
using System;
using UltimateAttributesPack;

public class VerticalLaserScript : MonoBehaviour
{
    GameManager _gameManager;
    VerticalLaserParams _trapParams;

    [SerializeField] GameObject _laserObject;

    float _startY;
    float _endY;
    float _currentMovementTime;
    float _currentMovementTimer;

    private void Awake()
    {
        // Get Game Manager
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        InitializeTrapParams();
        SetPositions();
        ActivateObject();
    }

    private void Update()
    {
        if(_currentMovementTimer < _currentMovementTime)
        {
            transform.position = new Vector2(0, Mathf.Lerp(_startY, _endY, _trapParams.MovementCurve.Evaluate(_currentMovementTimer / _currentMovementTime)));

            _currentMovementTimer += Time.deltaTime;
        }
        else
        {
            transform.position = new Vector2(0, _endY);
            DestroyTrap();
        }
    }

    public void InitializeTrapParams()
    {
        float currentWavePercent = _gameManager.GameLoopManager.CurrentWavePercent;

        _trapParams = _gameManager.TrapsManager.CurrentVerticalLaserParams;
        _currentMovementTime = Mathf.Lerp(_trapParams.MovementTimeStart, _trapParams.MovementTimeEnd, currentWavePercent);
    }

    void SetPositions()
    {
        // Calculate start and end positions
        float rand = UnityEngine.Random.value;
        _startY = rand >= 0.5f ? _gameManager.ArenaManager.WallUp.transform.position.y + _trapParams.SpawnOffset : _gameManager.ArenaManager.WallDown.transform.position.y - _trapParams.SpawnOffset;
        _endY = rand >= 0.5f ? _gameManager.ArenaManager.WallDown.transform.position.y - _trapParams.SpawnOffset : _gameManager.ArenaManager.WallUp.transform.position.y + _trapParams.SpawnOffset;

        // Set laser position
        transform.position = new Vector2(0, _startY);
        _laserObject.transform.position = new Vector2(0, _startY);

        // Set laser scale
        float arenaWidth = Vector3.Distance(_gameManager.ArenaManager.WallLeft.transform.position, _gameManager.ArenaManager.WallRight.transform.position);
        _laserObject.transform.localScale = new Vector2(arenaWidth, _trapParams.LaserWidth);
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
public class VerticalLaserParams
{
    [MinValue(0)] public float SpawnOffset;
    [MinValue(0)] public float LaserWidth;
    public AnimationCurve MovementCurve;
    [Space]
    [MinValue(0)] public float MovementTimeStart;
    [MinValue(0)] public float MovementTimeEnd;
}