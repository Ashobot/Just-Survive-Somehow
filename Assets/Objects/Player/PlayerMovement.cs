using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateAttributesPack;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // ----- Variables ----- //

    // Components
    PlayerController _playerController;
    PlayerInputs _playerInputs;

    Rigidbody2D _rb;

    // Values
    [Title("Movement", "white", "light blue")]

    [SerializeField, MinValue(0)] float _movementSpeed = 200f;
    [Space]
    [SerializeField, MinValue(0), MaxValue(1)] float _stickDeadZone = 0.05f;

    [Title("Dash", "white", "blue")]

    [SerializeField, MinValue(0)] float _dashCooldown = 1f;
    [Line("blue")]
    [SerializeField] AnimationCurve _dashCurve;
    [SerializeField, MinValue(0)] float _dashDuration = 0.25f;
    [SerializeField, MinValue(0)] float _dashSpeed = 400f;
    float _currentDashSpeed;
    float _dashTimer = 0f;
    bool _canDash = true;
    bool _isDashing = false;

    // ----- Functions ----- //

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        _playerInputs = _playerController.PlayerInputs;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ManageDash();
    }

    private void FixedUpdate()
    {
        Move();

        if (_isDashing)
            Dash();
    }

    // Movements functions

    private void Move()
    {
        if (_playerInputs.MoveInput.magnitude > _stickDeadZone)
            _rb.velocity = _playerInputs.MoveInput * _movementSpeed * Time.fixedDeltaTime;
        else
            _rb.velocity = Vector2.zero;
    }

    void ManageDash()
    {
        // If dash not started, start it and start cooldown
        if (_canDash && !_isDashing && _playerInputs.DashInput)
        {
            StartDash();
            Invoke(nameof(ResetCanDash), _dashCooldown);
        }
    }
   
    // Executed when dash is performed
    void Dash()
    {
        // Lerp velocity to dash speed if timer is not finished
        if(_dashTimer < _dashDuration)
        {
            _currentDashSpeed = Mathf.Lerp(_movementSpeed, _dashSpeed, _dashCurve.Evaluate(_dashTimer / _dashDuration));
            _rb.velocity = _playerInputs.MoveInput * _currentDashSpeed * Time.fixedDeltaTime;

            _dashTimer += Time.fixedDeltaTime;
        }
        // If timer is finished, reset dash
        else
        {
            ResetDash();
        }
    }

    // Executed once before dash started
    void StartDash()
    {
        _currentDashSpeed = _movementSpeed;
        _isDashing = true;
        _canDash = false;
    }

    // Executed once after the dash cooldown
    void ResetDash()
    {
        _dashTimer = 0;
        _isDashing = false;
    }

    // Executed once when dash cooldown is finished
    void ResetCanDash()
    {
        _canDash = true;
    }
}
