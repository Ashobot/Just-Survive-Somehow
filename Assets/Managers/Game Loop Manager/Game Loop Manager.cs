using UnityEngine;
using UltimateAttributesPack;
using UnityEngine.SceneManagement;

public class GameLoopManager : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField, ReadOnly] int _currentWave;
    public int CurrentWave { get { return _currentWave; } }
    [SerializeField, ProgressBar("", 0, 1, true)] float _currentWavePercent;
    public float CurrentWavePercent { get { return _currentWavePercent; } }

    [SerializeField, ReadOnly] float _waveTimer;

    [LineTitle("Scenes Manager")]
    [SerializeField, Scene] int _gameSceneIndex;

    private void Awake()
    {
        Time.timeScale = 1;

        // Get game manager
        _gameManager = transform.root.GetComponent<GameManager>();
    }

    public void ResetWaveTimer()
    {
        _waveTimer = 0f;
    }

    private void Update()
    {
        ManageWave();
    }

    void ManageWave()
    {
        // If the current wave is not at the maximum difficulty
        if (_waveTimer < _gameManager.DifficultyManager.CurrentDifficultyParams.WaveDuration)
        {
            _currentWavePercent = _waveTimer / _gameManager.DifficultyManager.CurrentDifficultyParams.WaveDuration;
            _waveTimer += Time.deltaTime;
        }
        // If the current wave is at maximum difficulty (reach timer end)
        else
            _currentWavePercent = 1f;
    }

    public void ReloadGame()
    {  
        SceneManager.LoadScene(_gameSceneIndex);
    }
}