using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerInputs : MonoBehaviour
{
    // ----- Variables ----- //

    // Components
    PlayerController _playerController;
    PlayerControls _playerControls;

    // Inputs
    public Vector2 MoveInput { get { return _moveInput; } }
    Vector2 _moveInput;
    public bool DashInput { get { return _dashInput; } }
    bool _dashInput;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerControls = new PlayerControls();
    }

    // ----- Movement inputs ----- //

    void OnMove(InputValue inputValue)
    {
        _moveInput = inputValue.Get<Vector2>();
    }

    void OnDash(InputValue inputValue)
    {
        _dashInput = inputValue.isPressed;
    }
}
