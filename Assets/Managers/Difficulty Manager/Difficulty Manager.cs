using UnityEngine;
using System;
using UltimateAttributesPack;

public class DifficultyManager : MonoBehaviour
{
    GameManager _gameManager;

    [HelpBox("All the difficulty parameters for each wave")]
    [SerializeField] DifficultyParams[] _difficultyParams;
    DifficultyParams _currentDifficultyParams;
    public DifficultyParams CurrentDifficultyParams { get { return _currentDifficultyParams; } }
    [Space]
    [SerializeField, ReadOnly] float _currentTrapSpawnRate;
    [SerializeField, ReadOnly] float _currentSpawnTimer;

    private void Awake()
    {
        // Get game manager
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        _currentDifficultyParams = _difficultyParams[0];
    }

    private void Update()
    {
        if (!_gameManager.GameLoopManager.WaveFinished)
        {
            RefreshTrapSpawnRate();
            ManageTrapSpawn();
        }
    }

    /// <summary>
    ///  Increase the difficulty level to the next level
    /// </summary>
    public void SetNextWave()
    {
        int newWaveIndex = Mathf.Clamp(_gameManager.GameLoopManager.CurrentWaveIndex + 1, 0, _difficultyParams.Length - 1);
        _gameManager.GameLoopManager.SetWave(newWaveIndex);
        _currentDifficultyParams = _difficultyParams[newWaveIndex]; // Change difficulty params
        _currentTrapSpawnRate = _currentDifficultyParams.TrapSpawnRateMin; // Set trap spawn rate to min of wave
        _gameManager.GameLoopManager.ResetWaveTimer(); // Set wave timer to 0
        _gameManager.TrapsManager.SetTrapsParamsLevel(newWaveIndex); // Set all traps params to the new difficulty level
    }

    /// <summary>
    /// Manage spawn rate during current wave with current difficulty params
    /// </summary>
    void RefreshTrapSpawnRate()
    {
        _currentTrapSpawnRate = Mathf.Lerp(_currentDifficultyParams.TrapSpawnRateMin, _currentDifficultyParams.TrapSpawnRateMax, _currentDifficultyParams.TrapSpawnRateCurve.Evaluate(_gameManager.GameLoopManager.CurrentWavePercent));
    }

    /// <summary>
    /// Manage trap spawn timer with current trap spawn rate
    /// </summary>
    void ManageTrapSpawn()
    {
        float timeBetweenNewTrap = 1 / _currentTrapSpawnRate; // Calculate current time between each trap spawn (in seconds)

        // If next trap spawn timer is not finished
        if(_currentSpawnTimer < timeBetweenNewTrap)
        {
            _currentSpawnTimer += Time.deltaTime;
        }
        // If timer is finished, spawn new trap and reset timer
        else
        {
            _gameManager.TrapsManager.SpawnRandomTrap(_currentDifficultyParams); // Spawn a random trap with current difficulty params
            _currentSpawnTimer = 0; // Reset timer
        }
    }
}

// ----- Serializable classes ----- //

[Serializable]
public class DifficultyParams
{
    [MinValue(0), Suffix("Seconds")] public float WaveDuration;
    [MinValue(0)] public float ArenaSize;
    public Trap[] Traps; 
    [Space]
    public AnimationCurve TrapSpawnRateCurve;
    [MinValue(0), Suffix("Per second")] public float TrapSpawnRateMin;
    [MinValue(0), Suffix("Per second")] public float TrapSpawnRateMax;
}