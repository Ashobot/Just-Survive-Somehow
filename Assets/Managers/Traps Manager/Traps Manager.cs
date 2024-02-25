using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UltimateAttributesPack;

public class TrapsManager : MonoBehaviour
{
    GameManager _gameManager;
    
    [Title("All Traps", "white", "orange")]

    [SubTitle("BearTrap", "white", "orange")]

    [SerializeField] BearTrapParams[] _bearTrapsParams;
    BearTrapParams _currentBearTrapsParams;
    public BearTrapParams CurrentBearTrapsParams { get { return _currentBearTrapsParams; } }

    private void Awake()
    {
        // Get game manager
        _gameManager = transform.root.GetComponent<GameManager>();
    }

    private void Start()
    {
        // Initialize all the traps params to first wave params
        InitializeTrapsParams();
    }

    void InitializeTrapsParams()
    {
        _currentBearTrapsParams = _bearTrapsParams[0];
    }

    float CalculateFullDeployTimeOf(TrapType trapType)
    {
        float fullDeployTime = 0;
        switch (trapType)
        {
            case TrapType.BearTrap:
                fullDeployTime += Mathf.Lerp(_currentBearTrapsParams.FallDurationMin, _currentBearTrapsParams.FallDurationMax, _gameManager.GameLoopManager.CurrentWavePercent);
                fullDeployTime += Mathf.Lerp(_currentBearTrapsParams.DeployDurationMin, _currentBearTrapsParams.DeployDurationMax, _gameManager.GameLoopManager.CurrentWavePercent);
                break;
            case TrapType.Laser:

                break;
            default:
                break;
        }
        return fullDeployTime;
    }

    public void SpawnTrap(TrapType trapType, SpawnType spawnType, Precision precision)
    {
        // Calculate random spawn position
        Vector2 spawnPosition = Vector2.zero;
        switch (spawnType)
        {
            case SpawnType.TargetZone:
                spawnPosition = _gameManager.PlayerController.PlayerMovement.CalculateTargetZoneSpawn(CalculateFullDeployTimeOf(trapType), precision);
                break;
            case SpawnType.OnSide:

                break;
            default:
                break;
        }

        // spawn trap of type
        switch (trapType)
        {
            case TrapType.BearTrap:
                SpawnBearTrap(spawnPosition);
                break;
            case TrapType.Laser:

                break;

            default:
                break;
        }
    }

    Vector2 CalculateRandomOnSideSpawn(SpawnSides spawnSides, Precision precision)
    {
        return Vector2.zero;
    }

    void SpawnBearTrap(Vector2 spawnPosition)
    {
        GameObject newTrap = GameObject.Instantiate(_bearTrapsParams[_gameManager.GameLoopManager.CurrentWave].Prefab, spawnPosition, Quaternion.identity); // Instanciate a new bear trap
    }

    // ----- Debug ----- //

    private void OnDrawGizmos()
    {
        
    }
}

// ----- Enums ----- //

public enum SpawnType
{
    OnSide,
    TargetZone
}

[Flags]
public enum SpawnSides
{
    None = 0,
    Left = 1 << 0,
    Up = 1 << 1,
    Right = 1 << 2,
    Down = 1 << 3
}

public enum TrapType
{
    BearTrap,
    Laser
}

public enum PrecisionType
{
    Precise,
    NotPrecise,
    Random
}

// ----- Serializable classes ----- //

[Serializable]
public class Trap
{
    public TrapType trapType;
    public SpawnType spawnType;
    public SpawnSides spawnSides;
    [MinValue(0), MaxValue(100), Indent(2)] public float percentChance;
}

[Serializable]
public class Precision
{
    public PrecisionType precisionType;
    [MinValue(0), MaxValue(100), Indent(4)] public float percentChance;
    [Space]
    [MinValue(0), MaxValue(100), Indent(2)] public float precisionAngle;
}