using UnityEngine;
using UltimateAttributesPack;
using UnityEngine.SceneManagement;

public class GameLoopManager : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] int _currentWaveIndex = -1;
    public int CurrentWaveIndex => _currentWaveIndex;
    [SerializeField, ProgressBar("", 0, 1, true)] float _currentWavePercent;
    public float CurrentWavePercent { get { return _currentWavePercent; } }

    [SerializeField, ReadOnly] float _waveTimer;

    bool _waveFinished;
    public bool WaveFinished => _waveFinished;
    bool _endWaveDialogueStarted;
    bool _gameStarted;
    public bool GameStarted => _gameStarted;

    [LineTitle("Scenes Manager")]
    [SerializeField, Scene] int _gameSceneIndex;

    private void Awake()
    {

        // Get game manager
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        Time.timeScale = 1;      
        //_currentWaveIndex = -1;
    }

    public void SetGameStarted(bool state)
    {
        _gameStarted = state;
    }

    public void SetWave(int index)
    {
        _currentWaveIndex = index;
    }

    public void ResetWaveTimer()
    {
        _waveTimer = 0f;
    }

    private void Update()
    {
        if (!_gameStarted)
            return;

        if(!_waveFinished)
            ManageWave();
        else
            ManageEndWave();
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

    void ManageEndWave()
    {
        if (!_endWaveDialogueStarted)
        {
            int trapsInMap = _gameManager.TrapsManager.TrapsParentObject.transform.childCount;

            if(trapsInMap == 0)
            {
                _gameManager.DialogueManager.StartRandomEndWaveDialogue(_currentWaveIndex);
                _endWaveDialogueStarted = true;
            }
        }
    }

    public void SetWaveFinished(bool state)
    {
        _waveFinished = state;
    }

    public void PassToNextWave()
    {
        SetWaveFinished(false);
        _endWaveDialogueStarted = false;

        _gameManager.DifficultyManager.SetNextWave();
    }

    public void ReloadGame()
    {  
        SceneManager.LoadScene(_gameSceneIndex);
    }
}