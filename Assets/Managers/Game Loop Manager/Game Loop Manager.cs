using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] int _currentWave;
    public int CurrentWave { get { return _currentWave; } }
    [SerializeField, Range(0, 1)] float _currentWavePercent;
    public float CurrentWavePercent { get { return _currentWavePercent; } }

    private void Awake()
    {
        // Get game manager
        _gameManager = transform.root.GetComponent<GameManager>();
    }
}
