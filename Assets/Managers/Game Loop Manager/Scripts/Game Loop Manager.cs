using UnityEngine;
using UltimateAttributesPack;
using UnityEngine.SceneManagement;
//using UnityEditor;

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
    [SerializeField, Scene] int _mainMenuScene;
    [SerializeField, Scene] int _gameScene;

    private void Awake()
    {
        // Get game manager
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        LoadGame();

        Time.timeScale = 1;      
    }

    // ----------- SAVE ---------- //

    void LoadGame()
    {
        if (PlayerPrefs.HasKey("Wave"))
        {
            int savedWaveIndex = PlayerPrefs.GetInt("Wave");
            if (savedWaveIndex <= 0)
            {
                _currentWaveIndex =  -1;
                _gameManager.DialogueManager.StartRandomGameStartDialogue();
            }
            else
            {
                _endWaveDialogueStarted = true;
                SetWaveFinished(true);
                SetGameStarted(true);
                _currentWaveIndex = savedWaveIndex - 1;
                _gameManager.DialogueManager.StartRandomEndWaveDialogue(savedWaveIndex - 1);
            }
        }
        else
        {
            PlayerPrefs.SetInt("Wave", -1);
            _currentWaveIndex = -1;

            _gameManager.DialogueManager.StartRandomGameStartDialogue();
        }
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("Wave", _currentWaveIndex);
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteAll();
    }

    //[MenuItem("Prefs/Reset")]
    //static void ResetPrefs()
    //{
    //    PlayerPrefs.DeleteAll();
    //}

    //[MenuItem("Prefs/Next Wave")]
    //static void PrefsNextWave()
    //{
    //    PlayerPrefs.SetInt("Wave", PlayerPrefs.GetInt("Wave") + 1);
    //}

    // ---------- WAVE ---------- //

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

        SaveGame();
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(_mainMenuScene);
    }

    public void LoadGameScene()
    {  
        SceneManager.LoadScene(_gameScene);
    }
}