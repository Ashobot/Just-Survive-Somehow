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
    [SerializeField] Button _btnContinue;
    [SerializeField] TextMeshProUGUI _btnContinueText;
    [Space]
    [SerializeField] Button _btnRestart;
    [SerializeField] TextMeshProUGUI _btnRestartText;

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
