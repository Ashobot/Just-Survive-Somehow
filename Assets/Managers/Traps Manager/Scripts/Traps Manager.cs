using UnityEngine;
using System;
using System.Collections.Generic;
using UltimateAttributesPack;

public class TrapsManager : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] Transform _trapsParentObject;
    public Transform TrapsParentObject => _trapsParentObject;

    [Title("Falling", "white", "orange")]

    [LineTitle("Bear trap")]
    [SerializeField] BearTrapParams[] _bearTrapsParams;
    BearTrapParams _currentBearTrapsParams;
    public BearTrapParams CurrentBearTrapsParams => _currentBearTrapsParams;

    [LineTitle("Bomb")]
    [SerializeField] BombParams[] _bombsParams;
    BombParams _currentBombParams;
    public BombParams CurrentBombParams => _currentBombParams;

    [Title("Lasers", "white", "red")]

    [LineTitle("Vertical laser")]
    [SerializeField] VerticalLaserParams[] _verticalLaserParams;
    VerticalLaserParams _currentVerticalLaserParams;
    public VerticalLaserParams CurrentVerticalLaserParams => _currentVerticalLaserParams;

    [LineTitle("Horizontal laser")]
    [SerializeField] HorizontalLaserParams[] _horizontalLaserParams;
    HorizontalLaserParams _currentHorizontalLaserParams;
    public HorizontalLaserParams CurrentHorizontalLaserParams => _currentHorizontalLaserParams;

    [Title("Followers","white", "light blue")]
    [SerializeField] RocketParams[] _rocketParams;
    RocketParams _currentRocketParams;
    public RocketParams CurrentRocketParams => _currentRocketParams;

    private void Awake()
    {
        // Get game manager
        _gameManager = FindObjectOfType<GameManager>();
    }

    public void SetTrapsParamsLevel(int waveIndex)
    {
        // ----- Falling ----- //

        // Bear Trap
        foreach(BearTrapParams param in _bearTrapsParams)
            if (param.WaveNumber == (waveIndex + 1).ToString())
                _currentBearTrapsParams = param;

        // Bomb
        foreach (BombParams param in _bombsParams)
            if (param.WaveNumber == (waveIndex + 1).ToString())
                _currentBombParams = param;

        // ----- Lasers ----- //

        // Vertical Laser
        foreach (VerticalLaserParams param in _verticalLaserParams)
            if (param.WaveNumber == (waveIndex + 1).ToString())
                _currentVerticalLaserParams = param;

        // Horizontal Laser
        foreach (HorizontalLaserParams param in _horizontalLaserParams)
            if (param.WaveNumber == (waveIndex + 1).ToString())
                _currentHorizontalLaserParams = param;

        // ----- Followers ----- //

        // Rocket
        foreach (RocketParams param in _rocketParams)
            if (param.WaveNumber == (waveIndex + 1).ToString())
                _currentRocketParams = param;
    }

    // ---------- Traps Spawn ---------- //

    List<Trap> GetSpawnableTrapsList(DifficultyParamsObject difficultyParams)
    {
        List<Trap> spawnableTraps = new List<Trap>();
        int[] trapsPerCategories = new int[_gameManager.DifficultyManager.CurrentDifficultyParams.WaveTrapTypes.TrapCategories.Length];
        
        // Foreach traps in traps parent
        foreach (TrapTypeScript trap in _trapsParentObject.GetComponentsInChildren<TrapTypeScript>())
        {
            // Foreach trap categories in current wave
            for(int i = 0; i < trapsPerCategories.Length; i++)
            {
                // If current trap is of the current trap category type
                if (trap.TrapType == _gameManager.DifficultyManager.CurrentDifficultyParams.WaveTrapTypes.TrapCategories[i].TrapType)
                {
                    trapsPerCategories[i]++; // Increment count of the current index of traps per category array
                    break;
                }
            }
        }

        // Foreach trap in current wave traps
        foreach(Trap trap in difficultyParams.Traps)
        {
            // Foreach trap category
            for(int i = 0; i < trapsPerCategories.Length; i++)
            {
                // If the type of the trap is the same as trap categoty
                if(trap.Type == _gameManager.DifficultyManager.CurrentDifficultyParams.WaveTrapTypes.TrapCategories[i].TrapType)
                {
                    // If there is less trap of this type than max count of this type in the map
                    if(trapsPerCategories[i] < _gameManager.DifficultyManager.CurrentDifficultyParams.WaveTrapTypes.TrapCategories[i].MaxCountInMap)
                    {
                        spawnableTraps.Add(trap); // Add the trap in the spawnable trap list
                        break;
                    }
                }
            }
        }
        return spawnableTraps;
    }

    public void SpawnRandomTrap(DifficultyParamsObject difficultyParams)
    {
        // Get spawnable traps
        List<Trap> spawnableTraps = GetSpawnableTrapsList(difficultyParams);
        if (spawnableTraps.Count == 0)
            return;

        // Calculate max chance
        float maxChance = 0;
        foreach(Trap trap in spawnableTraps)
            maxChance += trap.percentChance;

        // Chech if total chance is less than 100, else log warning
        if (maxChance > 100)
            Debug.LogWarning($"Traps spawn total percent chance is more than 100 in wave {_gameManager.GameLoopManager.CurrentWaveIndex}");

        float rand = UnityEngine.Random.Range(0, maxChance);
        float chancesSum = 0;

        // Get random trap and spawn it
        for (int i = 0; i < spawnableTraps.Count; i++)
        {
            if (rand >= chancesSum && rand <= spawnableTraps[i].percentChance + chancesSum)
            {
                Instantiate(spawnableTraps[i].TrapPrefab, Vector2.zero, Quaternion.identity, _trapsParentObject.transform); // Spawn new trap
                break;
            }
            else
                chancesSum += spawnableTraps[i].percentChance;
        }  
    }
}

// ----- Enums ----- //

public enum TrapType
{
    Falling,
    Laser,
    Followers

}

// ----- Serializable classes ----- //

[Serializable]
public class Trap
{
    public string Name;
    public TrapType Type;
    public GameObject TrapPrefab;
    [MinValue(0), MaxValue(100), Indent(2)] public float percentChance;
}

[Serializable]
public class TrapTypeParams
{
    public TrapType TrapType;
    public int MaxCountInMap;
}

[Serializable]
public class WaveTrapTypes
{
    public TrapTypeParams[] TrapCategories;
}