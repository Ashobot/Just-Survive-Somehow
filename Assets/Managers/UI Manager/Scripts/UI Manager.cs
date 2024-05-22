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
    public bool InPauseMenu { get { return _inPauseMenu; } }

    [LineTitle("Death menu")]
    [SerializeField] GameObject _deathMenuObj;
    [SerializeField] GameObject _deathMenuFirstSelectedObj;
    bool _inDeathMenu;
    public bool InDeathMenu { get { return _inDeathMenu; } }

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
        SetFirstSelectedObject(_pauseMenuFirstSelectedObj);
        Time.timeScale = state ? 0 : 1; // Pause or unpause time
        _inPauseMenu = state;
    }

    // ----- Death ----- //

    public void SetDeathMenu(bool state)
    {
        _deathMenuObj.SetActive(state);
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
}