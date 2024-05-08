using UnityEngine;
using UltimateAttributesPack;
using UnityEngine.SceneManagement;

public class GameLoopManager : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField, ReadOnly] int _currentWaveIndex;
    public int CurrentWaveIndex => _currentWaveIndex;
    [SerializeField, ProgressBar("", 0, 1, true)] float _currentWavePercent;
    public float CurrentWavePercent { get { return _currentWavePercent; } }

    [SerializeField, ReadOnly] float _waveTimer;

    bool _waveFinished;
    public bool WaveFinished => _waveFinished;

    bool _endWaveDialogueStarted;

    [LineTitle("Scenes Manager")]
    [SerializeField, Scene] int _gameSceneIndex;

    private void Awake()
    {
        Time.timeScale = 1;

        // Get game manager
        _gameManager = FindObjectOfType<GameManager>();
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
                SetEndWaveDialogueStarted(true);
            }
        }
    }

    public void SetWaveFinished(bool state)
    {
        _waveFinished = state;
    }

    public void SetEndWaveDialogueStarted(bool state)
    {
        _endWaveDialogueStarted = state;
    }

    public void ReloadGame()
    {  
        SceneManager.LoadScene(_gameSceneIndex);
    }
}