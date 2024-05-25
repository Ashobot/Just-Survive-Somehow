using UnityEngine;
using UnityEngine.SceneManagement;
using UltimateAttributesPack;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [SerializeField, Scene] int _mainMenuScene;
    [SerializeField, Scene] int _gameScene;
    [Space]
    [LineTitle("Main Menu")]
    [SerializeField] GameObject _mainMenuObject;
    [Space]
    [SerializeField] Button _btnContinue;
    [SerializeField] TextMeshProUGUI _btnContinueText;
    [Space]
    [SerializeField] Button _btnRestart;
    [SerializeField] TextMeshProUGUI _btnRestartText;
    [Space]
    [LineTitle("Crédits")]
    [SerializeField] GameObject _creditsObject;
    [SerializeField] GameObject _creditsFirstSelectedObject;

    void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == _mainMenuScene)
            InitializeMainMenuButtons();
    }

    void InitializeMainMenuButtons()
    {
        if(!PlayerPrefs.HasKey("Wave"))
        {
            _btnContinue.interactable = false;
            _btnRestartText.text = "Commencer";

            SetFirstSelectedObject(_btnRestart.gameObject);
        }
    }

    public void SetMainMenu(bool state)
    {
        _mainMenuObject.SetActive(state);
        if (state)
        {
            GameObject firstSelectedButton = _btnContinue.interactable ? _btnContinue.gameObject : _btnRestart.gameObject;
            SetFirstSelectedObject(firstSelectedButton);
        }
    }

    public void SetCredits(bool state)
    {
        _creditsObject.SetActive(state);
        if (state)        
            SetFirstSelectedObject(_creditsFirstSelectedObject);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetSave()
    {
        PlayerPrefs.SetInt("Wave", -1);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(_gameScene);
    }

    void SetFirstSelectedObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
        EventSystem.current.firstSelectedGameObject = go;
    }
}
