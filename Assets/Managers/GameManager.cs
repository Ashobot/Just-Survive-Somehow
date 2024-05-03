using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameLoopManager _gameLoopManager;
    public GameLoopManager GameLoopManager { get { return _gameLoopManager; } }

    [SerializeField] DifficultyManager _difficultyManager;
    public DifficultyManager DifficultyManager { get { return _difficultyManager; } }

    [SerializeField] TrapsManager _trapsManager;
    public TrapsManager TrapsManager { get { return _trapsManager; } }

    [SerializeField] ArenaManager _arenaManager;
    public ArenaManager ArenaManager { get { return _arenaManager; } }

    [Space]

    [SerializeField] PlayerController _playerController;
    public PlayerController PlayerController { get { return _playerController; } }
}
