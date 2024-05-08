using UnityEngine;
using System;
using System.Collections;
using UltimateAttributesPack;

public class DialogueManager : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] float _timeBetweenChars;
    [SerializeField] WaveDialogues[] _dialogues;

    bool _inDialogue;
    int _currentWaveIndex = 0;
    int _currentDialogueIndex = 0;
    int _currentLineIndex = 0;


    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    // Start a random end wave dialogue of wave
    public void StartRandomEndWaveDialogue(int waveIndex)
    {
        // Calculate max chance
        float maxChance = 0;
        foreach (Dialogue dialogue in _dialogues[waveIndex].EndWaveDialogues)
            maxChance += dialogue.PercentChance;

        // Chech if total chance is less than 100, else log warning
        if (maxChance > 100)
            Debug.LogWarning($"Dialogue total percent chance is more than 100 in wave {waveIndex}");

        float rand = UnityEngine.Random.Range(0, maxChance);
        float chancesSum = 0;

        // Get random dialogue and start it
        for (int i = 0; i < _dialogues[waveIndex].EndWaveDialogues.Length; i++)
        {
            if (rand >= chancesSum && rand <= _dialogues[waveIndex].EndWaveDialogues[i].PercentChance + chancesSum)
            {
                StartDialogue(waveIndex, i); // Start new dialogue
                return;
            }
            else
                chancesSum += _dialogues[waveIndex].EndWaveDialogues[i].PercentChance;
        }
    }

    void StartDialogue(int waveIndex, int dialogueIndex)
    {
        _gameManager.UIManager.SetDialogueState(true);
        _inDialogue = true;
        _currentWaveIndex = waveIndex;
        _currentDialogueIndex = dialogueIndex;
        _currentLineIndex = 0;
        StartCoroutine(TypeLine());
    }

    // Type the current line of current dialogue
    IEnumerator TypeLine()
    {
        foreach (char c in _dialogues[_currentWaveIndex].EndWaveDialogues[_currentDialogueIndex].Lines[_currentLineIndex])
        {
            _gameManager.UIManager.AddDialogueChar(c);
            yield return new WaitForSeconds(_timeBetweenChars);
        }
    }

    // Go to the next line of current dialogue
    public void NextLine()
    {
        // Return if not in dialogue
        if (!_inDialogue)
            return;

        // If text is complete
        if(_gameManager.UIManager.DialogueText.text == _dialogues[_currentWaveIndex].EndWaveDialogues[_currentDialogueIndex].Lines[_currentLineIndex])
        {
            // If there is a line after, then type it
            if(_currentLineIndex < _dialogues[_currentWaveIndex].EndWaveDialogues[_currentDialogueIndex].Lines.Length - 1)
            {
                _currentLineIndex++;
                _gameManager.UIManager.ClearDialogueText();
                StartCoroutine(TypeLine());
            }
            else
                EndDialogue();
        }
        // Else if text is not complete, complete it
        else
        {
            StopAllCoroutines();
            _gameManager.UIManager.DialogueText.text = _dialogues[_currentWaveIndex].EndWaveDialogues[_currentDialogueIndex].Lines[_currentLineIndex];
        }
    }

    void EndDialogue()
    {
        _gameManager.UIManager.ClearDialogueText();
        _gameManager.UIManager.SetDialogueState(false);
        _inDialogue = false;

        _gameManager.GameLoopManager.SetWaveFinished(false);
        _gameManager.GameLoopManager.SetEndWaveDialogueStarted(false);
        _gameManager.DifficultyManager.SetNextWave();
    }

}

// ----- Serializable classes ----- //

[Serializable]
public class WaveDialogues
{
    public string Name;
    public Dialogue[] EndWaveDialogues;
}

[Serializable]
public class Dialogue
{
    public string Name;
    [MinValue(0)] public float PercentChance;
    [TextArea] public string[] Lines;
}