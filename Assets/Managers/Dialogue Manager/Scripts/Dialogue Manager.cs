using UnityEngine;
using System;
using System.Collections;
using UltimateAttributesPack;
using UnityEngine.Events;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] float _timeBetweenChars;
    [SerializeField] Dialogue _gameStartDialogue;
    [Space]
    [SerializeField] Dialogue[] _endWaveDialogues;

    bool _inDialogue;
    DialogueType _currentDialogueType;
    int _currentWaveIndex = 0;
    int _currentDialogueIndex = 0;
    int _currentLineIndex = 0;

    const string HTML_ALPHA = "<color=#00000000>";

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    // Start a random game start dialogue
    public void StartRandomGameStartDialogue()
    {
        // Calculate max chance
        float maxChance = 0;
        foreach (DialogueParams dialogue in _gameStartDialogue.Dialogues)
            maxChance += dialogue.PercentChance;

        // Chech if total chance is less than 100, else log warning
        if (maxChance > 100)
            Debug.LogWarning($"Dialogue total percent chance is more than 100 in game start dialogues");

        float rand = UnityEngine.Random.Range(0, maxChance);
        float chancesSum = 0;

        // Get random dialogue and start it
        for (int i = 0; i < _gameStartDialogue.Dialogues.Length; i++)
        {
            if (rand >= chancesSum && rand <= _gameStartDialogue.Dialogues[i].PercentChance + chancesSum)
            {
                StartDialogue(DialogueType.GameStart, 0, i); // Start new dialogue
                return;
            }
            else
                chancesSum += _gameStartDialogue.Dialogues[i].PercentChance;
        }
    }

    // Start a random end wave dialogue of wave
    public void StartRandomEndWaveDialogue(int waveIndex)
    {
        waveIndex = Mathf.Clamp(waveIndex, 0, _endWaveDialogues.Length - 1);

        // Calculate max chance
        float maxChance = 0;
        foreach (DialogueParams dialogue in _endWaveDialogues[waveIndex].Dialogues)
            maxChance += dialogue.PercentChance;

        // Chech if total chance is less than 100, else log warning
        if (maxChance > 100)
            Debug.LogWarning($"Dialogue total percent chance is more than 100 in wave {waveIndex}");

        float rand = UnityEngine.Random.Range(0, maxChance);
        float chancesSum = 0;

        // Get random dialogue and start it
        for (int i = 0; i < _endWaveDialogues[waveIndex].Dialogues.Length; i++)
        {
            if (rand >= chancesSum && rand <= _endWaveDialogues[waveIndex].Dialogues[i].PercentChance + chancesSum)
            {
                StartDialogue(DialogueType.EndWave ,waveIndex, i); // Start new dialogue
                return;
            }
            else
                chancesSum += _endWaveDialogues[waveIndex].Dialogues[i].PercentChance;
        }
    }

    void StartDialogue(DialogueType dialogueType, int waveIndex, int dialogueIndex)
    {
        _inDialogue = true;

        _gameManager.UIManager.ClearDialogueText();
        _gameManager.UIManager.DialogueShowAnimation();

        _currentDialogueType = dialogueType;
        _currentWaveIndex = waveIndex;
        _currentDialogueIndex = dialogueIndex;
        _currentLineIndex = 0;
        StartCoroutine(TypeLine());
    }

    // Type the current line of current dialogue
    IEnumerator TypeLine()
    {
        _gameManager.UIManager.ClearDialogueText();
        int alphaIndex = 0;
        string displayedText = "";

        // Get current line
        string currentDialogueLine = string.Empty;
        switch (_currentDialogueType)
        {
            case DialogueType.GameStart:
                currentDialogueLine = _gameStartDialogue.Dialogues[_currentDialogueIndex].DialogueObject.Lines[_currentLineIndex];
                break;
            case DialogueType.EndWave:
                currentDialogueLine = _endWaveDialogues[_currentWaveIndex].Dialogues[_currentDialogueIndex].DialogueObject.Lines[_currentLineIndex];
                break;
        }
        
        // Type letters
        foreach(char c in currentDialogueLine)
        {
            alphaIndex++;
            _gameManager.UIManager.SetDialogueText(currentDialogueLine);
            displayedText = _gameManager.UIManager.DialogueText.text.Insert(alphaIndex, HTML_ALPHA);
            _gameManager.UIManager.SetDialogueText(displayedText);

            yield return new WaitForSeconds(_timeBetweenChars);
        }
    }

    // Go to the next line of current dialogue
    public void NextLine()
    {
        // Return if not in dialogue
        if (!_inDialogue || Time.timeScale == 0)
            return;

        // Get current line
        string currentDialogueLine = string.Empty;
        int currentDialogueLineCount = 0;
        switch (_currentDialogueType)
        {
            case DialogueType.GameStart:
                currentDialogueLine = _gameStartDialogue.Dialogues[_currentDialogueIndex].DialogueObject.Lines[_currentLineIndex];
                currentDialogueLineCount = _gameStartDialogue.Dialogues[_currentDialogueIndex].DialogueObject.Lines.Length;
                break;
            case DialogueType.EndWave:
                currentDialogueLine = _endWaveDialogues[_currentWaveIndex].Dialogues[_currentDialogueIndex].DialogueObject.Lines[_currentLineIndex];
                currentDialogueLineCount = _endWaveDialogues[_currentWaveIndex].Dialogues[_currentDialogueIndex].DialogueObject.Lines.Length;
                break;
        }

        // If text is complete
        if (_gameManager.UIManager.DialogueText.text == currentDialogueLine + HTML_ALPHA)
        {
            // If there is a line after, then type it
            if(_currentLineIndex < currentDialogueLineCount - 1)
            {
                _currentLineIndex++;
                _gameManager.UIManager.ClearDialogueText();
                StartCoroutine(TypeLine());
            }
            else
            {
                EndDialogue();
            }
        }
        // Else if text is not complete, complete it
        else
        {
            StopAllCoroutines();
            _gameManager.UIManager.SetDialogueText(currentDialogueLine + HTML_ALPHA);
        }
    }

    void EndDialogue()
    {
        _gameManager.UIManager.DialogueHideAnimation();

        _inDialogue = false;

        // Play end event of the dialogue
        switch (_currentDialogueType)
        {
            case DialogueType.GameStart:
                _gameStartDialogue.Dialogues[_currentDialogueIndex].OnEndEvent.Invoke();
                break;
            case DialogueType.EndWave:
                _endWaveDialogues[_currentWaveIndex].Dialogues[_currentDialogueIndex].OnEndEvent.Invoke();
                break;
        }
    }
}

// ----- Enums ----- //

enum DialogueType
{
    GameStart,
    EndWave
}

// ----- Serializable classes ----- //

[Serializable]
public class Dialogue
{
    public string Name;
    public DialogueParams[] Dialogues;
}

[Serializable]
public class DialogueParams
{
    public string Name;
    public DialogueObject DialogueObject;
    [MinValue(0)] public float PercentChance;
    public UnityEvent OnEndEvent;
}