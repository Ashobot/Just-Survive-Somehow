using UnityEngine;
using UnityEngine.EventSystems;
using UltimateAttributesPack;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [LineTitle("Pause menu")]
    [SerializeField] GameObject _pauseMenuObj;
    [SerializeField] GameObject _pauseMenuFirstSelectedObj;
    bool _inPauseMenu;
    public bool InPauseMenu => _inPauseMenu;

    [LineTitle("Options")]
    [SerializeField] GameObject _optionsMenuObj;
    [SerializeField] GameObject _optionsMenuFirstSelectedObj;
    [Space]
    [SerializeField] Slider _masterVolumeSlider;
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] Slider _soundFXVolumeSlider;
    bool _inOptions;
    public bool InOptions => _inOptions;

    [LineTitle("Death menu")]
    [SerializeField] GameObject _deathMenuObj;
    [SerializeField] GameObject _deathMenuFirstSelectedObj;
    bool _inDeathMenu;
    public bool InDeathMenu => _inDeathMenu;

    [LineTitle("Risking Death Image")]
    [SerializeField] Image _riskingDeathImage;
    [SerializeField] AnimationCurve _rinskingDeathCurve;
    [Space]
    [SerializeField] Color _riskingDeathMinColor;
    [SerializeField] Color _riskingDeathMaxColor;

    [LineTitle("Dialogues")]
    [SerializeField] GameObject _dialogueObject;
    [SerializeField] TextMeshProUGUI _dialogueText;
    public TextMeshProUGUI DialogueText => _dialogueText;
    [Space]
    [SerializeField] float _dialogueShowHideTime;
    [SerializeField] AnimationCurve _dialogueShowHideCurve;
    [Space]
    [SerializeField] Image _endDialogueCircleImage;

    private void Start()
    {
        InitializeOptionsValues();
    }

    // ----- General Functions ----- //

    void SetFirstSelectedObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
        EventSystem.current.firstSelectedGameObject = go;
    }

    // ----- Pause menu ----- //

    public void SetPauseMenu(bool state)
    {
        // Can't pause if in death menu
        if (_inDeathMenu)
            return;

        _pauseMenuObj.SetActive(state);
        if (state)
            SetFirstSelectedObject(_pauseMenuFirstSelectedObj);
        Time.timeScale = state ? 0 : 1; // Pause or unpause time

        // Set sounds pause
        SoundManager.instance.SetSoundsPause(state);
    }

    // ----- Options ----- //

    void InitializeOptionsValues()
    {
        // Set master volume slider
        if(PlayerPrefs.HasKey("Master Volume"))
            _masterVolumeSlider.value = PlayerPrefs.GetFloat("Master Volume");
        else
        {
            _masterVolumeSlider.value = 1f;
            PlayerPrefs.SetFloat("Master Volume", 1f);
        }

        // Set music volume slider
        if (PlayerPrefs.HasKey("Music Volume"))
            _musicVolumeSlider.value = PlayerPrefs.GetFloat("Music Volume");
        else
        {
            _musicVolumeSlider.value = 1f;
            PlayerPrefs.SetFloat("Music Volume", 1f);
        }

        // Set sound fx volume slider
        if (PlayerPrefs.HasKey("Sound FX Volume"))
            _soundFXVolumeSlider.value = PlayerPrefs.GetFloat("Sound FX Volume");
        else
        {
            _soundFXVolumeSlider.value = 1f;
            PlayerPrefs.SetFloat("Sound FX Volume", 1f);
        }
    }

    public void SetOptionsMenu(bool state)
    {
        _optionsMenuObj.SetActive(state);
        if(state)
            SetFirstSelectedObject(_optionsMenuFirstSelectedObj);
        Time.timeScale = state ? 0 : 1; // Pause or unpause time
        _inOptions = state;

        // Set sounds pause
        SoundManager.instance.SetSoundsPause(state);
    }

    // ----- Death ----- //

    public void SetDeathMenu(bool state)
    {
        _deathMenuObj.SetActive(state);
        if(state)
            SetFirstSelectedObject(_deathMenuFirstSelectedObj);
        _inDeathMenu = state;
    }

    public void SetRinskingDeathImage(float percent)
    {
        _riskingDeathImage.color = Color.Lerp(_riskingDeathMaxColor, _riskingDeathMinColor, _rinskingDeathCurve.Evaluate(percent)); // Lerp and set color
    }

    // ----- Dialogues ----- //

    public void DialogueShowAnimation()
    {
        SetDialogueState(true);

        LeanTween.scale(_dialogueObject, Vector2.one, _dialogueShowHideTime);
    }

    public void DialogueHideAnimation()
    {
        LeanTween.scale(_dialogueObject, Vector2.zero, _dialogueShowHideTime);

        SetDialogueState(false);
    }

    void SetDialogueState(bool state)
    {
        _dialogueObject.transform.localScale = state ? Vector2.one : Vector2.zero;
        _dialogueObject.SetActive(state);
    }

    public void ClearDialogueText()
    {
        _dialogueText.text = "";
    }

    public void SetDialogueText(string text)
    {
        _dialogueText.text = text;
    }

    public void AddDialogueChar(char character)
    {
        _dialogueText.text += character;
    }

    public void SetEndDialogueCirclePercent(float percent)
    {
        _endDialogueCircleImage.fillAmount = Mathf.Clamp(percent, 0f, 1f);
    }
}