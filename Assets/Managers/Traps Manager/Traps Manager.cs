using UnityEngine;
using System;
using System.Collections.Generic;
using UltimateAttributesPack;

public class TrapsManager : MonoBehaviour
{
    GameManager _gameManager;

    [Title("Falling traps", "white", "orange")]

    [LineTitle("Bear trap")]
    [SerializeField] BearTrapParams[] _bearTrapsParams;
    BearTrapParams _currentBearTrapsParams;
    public BearTrapParams CurrentBearTrapsParams { get { return _currentBearTrapsParams; } }

    [Title("Lasers", "white", "red")]

    [LineTitle("Vertical laser")]
    [SerializeField] VerticalLaserParams[] _verticalLaserParams;
    VerticalLaserParams _currentVerticalLaserParams;
    public VerticalLaserParams CurrentVerticalLaserParams { get { return _currentVerticalLaserParams;} }


    private void Awake()
    {
        // Get game manager
        _gameManager = transform.root.GetComponent<GameManager>();
    }

    private void Start()
    {
        // Initialize all the traps params to first wave params
        SetTrapsParamsLevel(0);
    }

    /// <summary>
    /// Set all traps params to difficulty level (in trap params list)
    /// </summary>
    public void SetTrapsParamsLevel(int levelIndex)
    {
        // Bear Trap
        if (_bearTrapsParams[levelIndex] != null) _currentBearTrapsParams = _bearTrapsParams[levelIndex];
        else Debug.LogWarning($"Bear Traps Params : difficulty level {levelIndex} is not set in Traps Manager");

        // Vertical Laser
        if (_verticalLaserParams[levelIndex] != null) _currentVerticalLaserParams = _verticalLaserParams[levelIndex];
        else Debug.LogWarning($"Vertical Laser Params : difficulty level {levelIndex} is not set in Traps Manager");
    }

    // ---------- Traps Spawn ---------- //

    /// <summary>
    /// Spawns a random trap with current difficulty params
    /// </summary>
    /// <param name="difficultyParams">The difficulty params</param>
    public void SpawnRandomTrap(DifficultyParams difficultyParams)
    {
        float rand = UnityEngine.Random.Range(0, 100);
        float chancesSum = 0;

        // Get random trap
        for (int i = 0; i < difficultyParams.Traps.Length; i++)
        {
            if (rand >= chancesSum && rand <= difficultyParams.Traps[i].percentChance + chancesSum)
            {
                if (difficultyParams.Traps[i].IsFallingTrap && !_gameManager.PlayerController.PlayerMovement.CanSpawnGroundTrapAround(difficultyParams.Traps[i].MaxTrapsAroundPlayer, difficultyParams.Traps[i].CheckTrapsAroundPlayerRadius))
                {
                    SpawnRandomNonGroundTrap(difficultyParams);
                    return;
                }
                else
                    Instantiate(difficultyParams.Traps[i].TrapPrefab, Vector2.zero, Quaternion.identity); // Spawn new trap
            }
            else
                chancesSum += difficultyParams.Traps[i].percentChance;
        }  
    }

    /// <summary>
    /// Spawns a random non ground trap with current difficulty params
    /// </summary>
    /// <param name="difficultyParams">The difficulty params</param>
    void SpawnRandomNonGroundTrap(DifficultyParams difficultyParams)
    {
        // Get all non ground traps in current difficulty params
        float maxChance = 0f;
        List<Trap> nonGroundTraps = new List<Trap>();
        foreach(Trap trap in difficultyParams.Traps)
        {
            if (!trap.IsFallingTrap)
            {
                nonGroundTraps.Add(trap);
                maxChance += trap.percentChance;
            }
        }

        // Get and instanciate random non ground trap
        float rand = UnityEngine.Random.Range(0, maxChance);
        float chancesSum = 0;
        for (int i = 0; i < nonGroundTraps.Count; i++)
        {
            if (rand >= chancesSum && rand <= nonGroundTraps[i].percentChance + chancesSum)
                Instantiate(nonGroundTraps[i].TrapPrefab, Vector2.zero, Quaternion.identity); // Spawn new trap
            else
                chancesSum += nonGroundTraps[i].percentChance;
        }
    }
}

// ----- Serializable classes ----- //

[Serializable]
public class Trap
{
    public GameObject TrapPrefab;
    [MinValue(0), MaxValue(100), Indent(2)] public float percentChance;
    [Space]
    public bool IsFallingTrap;
    [MinValue(0)] public int MaxTrapsAroundPlayer;
    [MinValue(0)] public float CheckTrapsAroundPlayerRadius;
}