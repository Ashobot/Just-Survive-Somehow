using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UltimateAttributesPack;

[RequireComponent(typeof(PlayerInput))]
public class InputsManager : MonoBehaviour
{
    // ----- Variables ----- //

    // Components
    GameManager _gameManager;
    PlayerControls _playerControls;

    // Movement inputs

    public Vector2 MoveInput { get { return _moveInput; } }
    [SerializeField, ReadOnly] Vector2 _moveInput;

    public bool DashInput { get { return _dashInput; } }
    [SerializeField, ReadOnly] bool _dashInput;

    private void OnEnable()
    {
        _playerControls.Enable();

        // Enable move input
        _playerControls.Movements.Move.started += OnStartMove;
        _playerControls.Movements.Move.performed += OnMove;
        _playerControls.Movements.Move.canceled += OnEndMove;

        // Enable dash input
        _playerControls.Movements.Dash.started += OnStartDash;
        _playerControls.Movements.Dash.performed += OnDash;
        _playerControls.Movements.Dash.canceled += OnEndDash;

        // Enable menu pause input
        _playerControls.Menus.Pause.started += OnPause;

        // Enable dialogues input
        _playerControls.Dialogues.NextLine.started += OnNextLine;
    }

    private void OnDisable()
    {
        _playerControls.Disable();

        // Disable move input
        _playerControls.Movements.Move.started -= OnStartMove;
        _playerControls.Movements.Move.performed -= OnMove;
        _playerControls.Movements.Move.canceled -= OnEndMove;

        // Disable dash input
        _playerControls.Movements.Dash.started -= OnStartDash;
        _playerControls.Movements.Dash.performed -= OnDash;
        _playerControls.Movements.Dash.canceled -= OnEndDash;

        // Disable menu pause input
        _playerControls.Menus.Pause.started -= OnPause;

        // Disable dialogues input
        _playerControls.Dialogues.NextLine.started -= OnNextLine;
    }

    private void Awake()
    {
        // Get game manager
        _gameManager = FindObjectOfType<GameManager>();

        _playerControls = new PlayerControls();
    }

    // ----- Movement input ----- //

    void OnStartMove(InputAction.CallbackContext ctx)
    {

    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }

    void OnEndMove(InputAction.CallbackContext ctx)
    {
        _moveInput = Vector2.zero;
    }

    // ----- Dash input ----- //

    void OnStartDash(InputAction.CallbackContext ctx)
    {
        _dashInput = true;
    }

    void OnDash(InputAction.CallbackContext ctx)
    {

    }

    void OnEndDash(InputAction.CallbackContext ctx)
    {

    }

    public void DisableDashInput()
    {
        _dashInput = false;
    }

    // ----- Menu pause input ----- //

    void OnPause(InputAction.CallbackContext ctx)
    {
        _gameManager.UIManager.SetPauseMenu(!_gameManager.UIManager.InPauseMenu);
    }

    // ----- Dialogues input ----- //

    void OnNextLine(InputAction.CallbackContext ctx)
    {
        Debug.Log("next line");
        _gameManager.DialogueManager.NextLine();
    }

}
