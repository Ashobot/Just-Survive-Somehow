using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputsManager : MonoBehaviour
{
    // ----- Variables ----- //

    // Components
    PlayerControls _playerControls;

    // Movement inputs

    public Vector2 MoveInput { get { return _moveInput; } }
    [SerializeField] Vector2 _moveInput;

    public bool DashInput { get { return _dashInput; } }
    [SerializeField] bool _dashInput;

    private void OnEnable()
    {
        _playerControls.Enable();

        // Enable move input
        _playerControls.Movements.Move.started += OnStartMove;
        _playerControls.Movements.Move.performed += OnMove;
        _playerControls.Movements.Move.canceled += OnEndMove;

        // Enable dash input
        _playerControls.Movements.Dash.started += OnStartDash;
    }

    private void OnDisable()
    {
        _playerControls.Disable();

        _playerControls.Movements.Move.started -= OnStartMove;
        _playerControls.Movements.Move.performed -= OnMove;
        _playerControls.Movements.Move.canceled -= OnEndMove;
    }

    private void Awake()
    {
        _playerControls = new PlayerControls();
    }

    // ----- Movement input ----- //

    void OnStartMove(InputAction.CallbackContext ctx)
    {

    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>().normalized;
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
}
