using UnityEngine;
using System;
using System.Collections.Generic;
using UltimateAttributesPack;

public class TrapsManager : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] Transform _trapsParentObject;
    public Transform TrapsParentObject => _trapsParentObject;

    [Title("Traps categories")]
    [SerializeField] WaveTrapTypes[] _waveTrapCategories;

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

    private void Start()
    {
        // Initialize all the traps params to first wave params
        SetTrapsParamsLevel(0);
    }

    public void SetTrapsParamsLevel(int levelIndex)
    {
        // ----- Falling ----- //

        // Bear Trap
        if (_bearTrapsParams[levelIndex] != null) _currentBearTrapsParams = _bearTrapsParams[levelIndex];
        else Debug.LogWarning($"Bear Traps Params : difficulty level {levelIndex} is not set in Traps Manager");

        // Bomb
        if (_bombsParams[levelIndex] != null) _currentBombParams = _bombsParams[levelIndex];
        else Debug.LogWarning($"Bomb Params : difficulty level {levelIndex} is not set in Traps Manager");

        // ----- Lasers ----- //

        // Vertical Laser
        if (_verticalLaserParams[levelIndex] != null) _currentVerticalLaserParams = _verticalLaserParams[levelIndex];
        else Debug.LogWarning($"Vertical Laser Params : difficulty level {levelIndex} is not set in Traps Manager");

        // Horizontal Laser
        if (_horizontalLaserParams[levelIndex] != null) _currentHorizontalLaserParams = _horizontalLaserParams[levelIndex];
        else Debug.LogWarning($"Horizontal Laser Params : difficulty level {levelIndex} is not set in Traps Manager");

        // ----- Followers ----- //

        // Rocket
        if (_rocketParams[levelIndex] != null) _currentRocketParams = _rocketParams[levelIndex];
        else Debug.LogWarning($"Rocket Params : difficulty level {levelIndex} is not set in Traps Manager");
    }

    // ---------- Traps Spawn ---------- //

    List<Trap> GetSpawnableTrapsList(DifficultyParams difficultyParams)
    {
        List<Trap> spawnableTraps = new List<Trap>();
        int[] trapsPerCategories = new int[_waveTrapCategories[_gameManager.GameLoopManager.CurrentWaveIndex].TrapCategories.Length];
             
        // Foreach traps in traps parent
        foreach (TrapTypeScript trap in _trapsParentObject.GetComponentsInChildren<TrapTypeScript>())
        {
            // Foreach trap categories in current wave
            for(int i = 0; i < trapsPerCategories.Length; i++)
            {
                // If current trap is of the current trap category type
                if (trap.TrapType == _waveTrapCategories[_gameManager.GameLoopManager.CurrentWaveIndex].TrapCategories[i].TrapType)
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
                if(trap.Type == _waveTrapCategories[_gameManager.GameLoopManager.CurrentWaveIndex].TrapCategories[i].TrapType)
                {
                    // If there is less trap of this type than max count of this type in the map
                    if(trapsPerCategories[i] < _waveTrapCategories[_gameManager.GameLoopManager.CurrentWaveIndex].TrapCategories[i].MaxCountInMap)
                    {
                        spawnableTraps.Add(trap); // Add the trap in the spawnable trap list
                        break;
                    }
                }
            }
        }
        return spawnableTraps;
    }

    public void SpawnRandomTrap(DifficultyParams difficultyParams)
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