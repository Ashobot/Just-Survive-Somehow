using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameLoopManager _gameLoopManager;
    public GameLoopManager GameLoopManager => _gameLoopManager;

    [SerializeField] DifficultyManager _difficultyManager;
    public DifficultyManager DifficultyManager => _difficultyManager;

    [SerializeField] TrapsManager _trapsManager;
    public TrapsManager TrapsManager => _trapsManager;

    [SerializeField] ArenaManager _arenaManager;
    public ArenaManager ArenaManager => _arenaManager;

    [SerializeField] UIManager _uiManager;
    public UIManager UIManager => _uiManager;

    [Space]

    [SerializeField] PlayerController _playerController;
    public PlayerController PlayerController { get { return _playerController; } }
}
