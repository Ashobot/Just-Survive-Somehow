using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UltimateAttributesPack;

public class DifficultyManager : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField, MinValue(0)] int _currentDifficultyLevel;

    [HelpBox("All the difficulty parameters per wave")]
    [SerializeField] DifficultyParams[] _difficultyParams;
    DifficultyParams _currentDifficultyParams;
    public DifficultyParams CurrentWaveDifficultyParams { get { return _currentDifficultyParams; } }

    [SerializeField, ReadOnly] float _currentWaveTimer;
    [SerializeField, ReadOnly] float _currentTrapSpawnRate;
    [SerializeField, ReadOnly] float _currentSpawnTimer;

    private void Awake()
    {
        // Get game manager
        _gameManager = transform.root.GetComponent<GameManager>();
    }

    private void Start()
    {
        _currentDifficultyParams = _difficultyParams[0];
    }

    private void Update()
    {
        ManageWave();
        ManageTrapSpawn();
    }

    public void SetDifficultyLevelTo(int level)
    {
        _currentDifficultyLevel = level;
        _currentDifficultyParams = _difficultyParams[level];
        _currentTrapSpawnRate = _currentDifficultyParams.TrapSpawnRateMin;
        _currentWaveTimer = 0;
    }

    void ManageWave()
    {
        if (_currentWaveTimer < _currentDifficultyParams.WaveDuration)
        {
            _currentTrapSpawnRate = Mathf.Lerp(_currentDifficultyParams.TrapSpawnRateMin, _currentDifficultyParams.TrapSpawnRateMax, _currentDifficultyParams.TrapSpawnRateCurve.Evaluate(_currentWaveTimer / _currentDifficultyParams.WaveDuration));
            _currentWaveTimer += Time.deltaTime;
        }
        else
        {
            SetDifficultyLevelTo(_currentDifficultyLevel + 1);
        }
    }

    void ManageTrapSpawn()
    {
        float timeBetweenNewTrap = 1 / _currentTrapSpawnRate;

        if(_currentSpawnTimer < timeBetweenNewTrap)
        {
            _currentSpawnTimer += Time.deltaTime;
        }
        else
        {
            SpawnRandomTrap();
            _currentSpawnTimer = 0;
        }
    }

    // Spawn random trap of the current wave difficulty
    void SpawnRandomTrap()
    {
        TrapType trapType = TrapType.BearTrap;
        SpawnType spawnType = SpawnType.TargetZone;
        Precision precision = _currentDifficultyParams.SpawnPrecisions[0];

        float rand = UnityEngine.Random.Range(0, 100);
        float chancesSum = 0;

        // Get random trap
        for(int i = 0; i < _currentDifficultyParams.Traps.Length; i++)
        {
            if(rand >= chancesSum && rand <= _currentDifficultyParams.Traps[i].percentChance + chancesSum)
            {
                trapType = _currentDifficultyParams.Traps[i].trapType;
                spawnType = _currentDifficultyParams.Traps[i].spawnType;
            }
            else
            {
                chancesSum += _currentDifficultyParams.Traps[i].percentChance;
            }
        }
        // Get random precision
        for (int i = 0; i < _currentDifficultyParams.SpawnPrecisions.Length; i++)
        {
            if (rand >= chancesSum && rand <= _currentDifficultyParams.SpawnPrecisions[i].percentChance + chancesSum)
            {
                precision = _currentDifficultyParams.SpawnPrecisions[i];
            }
            else
            {
                chancesSum += _currentDifficultyParams.SpawnPrecisions[i].percentChance;
            }
        }

        // Spawn trap
        _gameManager.TrapsManager.SpawnTrap(trapType, spawnType, precision);
    }
}

[Serializable]
public class DifficultyParams
{
    [MinValue(0), Suffix("Seconds")] public float WaveDuration;
    public Trap[] Traps;
    public Precision[] SpawnPrecisions;
    [Space]
    public AnimationCurve TrapSpawnRateCurve;
    [MinValue(0), Suffix("Per second")] public float TrapSpawnRateMin;
    [MinValue(0), Suffix("Per second")] public float TrapSpawnRateMax;
}
