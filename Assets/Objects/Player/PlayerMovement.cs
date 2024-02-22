using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateAttributesPack;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // --------------------- //
    // ----- Variables ----- //
    // --------------------- //

    // Components
    PlayerController _playerController;
    InputsManager _inputsManager;

    Rigidbody2D _rb;
    SpriteRenderer _sr;

    // Movement
    [Title("Movement", "white", "light blue")]

    [SerializeField, MinValue(0)] float _movementSpeed = 200f;
    [SerializeField] bool _smoothChangeDirection;
    [SerializeField, ShowIf(nameof(_smoothChangeDirection), true), MinValue(0)] float _moveChangeDirectionSpeed = 5f;
    bool _isMoving;

    // Dash
    [Title("Dash", "white", "blue")]

    [SerializeField] bool _canChangeDirectionDuringDash = true;
    [SerializeField, ShowIf(nameof(_canChangeDirectionDuringDash), true)] Transform _currentRotator;
    [SerializeField, ShowIf(nameof(_canChangeDirectionDuringDash), true)] Transform _targetRotator;
    [SerializeField, ShowIf(nameof(_canChangeDirectionDuringDash), true), MinValue(0)] float _dashChangeDirectionSpeed = 0.75f;
    [Space]
    [SerializeField, MinValue(0)] float _dashCooldown = 1f;
    [Line("blue")]
    [SerializeField] AnimationCurve _dashCurve;
    [SerializeField, MinValue(0)] float _dashLenght = 3f;
    [SerializeField, MinValue(0)] float _dashDuration = 0.25f;
    [Space]
    [SerializeField] Color _colorWhenDash;
    float _dashTimer = 0f;
    bool _canDash = true;
    bool _isDashing = false;
    Vector2 _startDashPos;
    Vector2 _endDashPos;
    Vector2 _lastDashTargetEndPos;
    Vector2 _dashVelocityRef;

    // Debug
    [Title("Debug", "white", "grey")]

    [SerializeField] bool _showDebug;
    [SerializeField, ShowIf(nameof(_showDebug), true)] Color _currentForwardDebugColor;
    [SerializeField, ShowIf(nameof(_showDebug), true)] Color _targetForwardDebugColor;
    [SerializeField, ShowIf(nameof(_showDebug), true)] float _forwardDebugLenght;
    Color _normalColor;

    // --------------------- //
    // ----- Functions ----- //
    // --------------------- //

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        _inputsManager = _playerController.InputManager;

        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();

        _normalColor = _sr.color;
    }

    private void Update()
    {
        ManageRotators();
        ManageDash();

        ManageColor();
    }

    private void FixedUpdate()
    {
        Move();

        if (_isDashing)
            Dash();
    }

    // ----- Rotations ----- //

    void ManageRotators()
    {
        // Set target rotator rotation with stick input
        if (_inputsManager.MoveInput != Vector2.zero)
            _targetRotator.up = _inputsManager.MoveInput;
        else
            _targetRotator.up = _currentRotator.up;

        // Smooth rotation from current to target
        if (_isDashing && _canChangeDirectionDuringDash) // If we dash and we can change direction during it
            _currentRotator.rotation = Quaternion.RotateTowards(_currentRotator.rotation, _targetRotator.rotation, _dashChangeDirectionSpeed * Time.deltaTime); 
        else if (_isMoving && _smoothChangeDirection) // If we move and smooth change direction is activated
            _currentRotator.rotation = Quaternion.RotateTowards(_currentRotator.rotation, _targetRotator.rotation, _moveChangeDirectionSpeed * Time.deltaTime);
        else // If movement and dash rotation are both not smooth
            _currentRotator.rotation = _targetRotator.rotation;
    }

    // ----- Movement ----- //

    private void Move()
    {
        // Move only if not dashing and stick input is not 0
        if(_inputsManager.MoveInput != Vector2.zero && !_isDashing)
        {
            _isMoving = true;
     
            if(_smoothChangeDirection) // Smooth change direction when moving if it's activated
                _rb.MovePosition((Vector2)transform.position + ((Vector2)_currentRotator.up * _movementSpeed * Time.deltaTime));
            else
                _rb.MovePosition((Vector2)transform.position + (_inputsManager.MoveInput * _movementSpeed * Time.deltaTime));
        }
        else
            _isMoving = false;
    }

    // ----- Dash ----- //

    void ManageDash()
    {
        // If dash not started, start it and start cooldown
        if (_isMoving && _canDash && !_isDashing && _inputsManager.DashInput)
        {
            StartDash();
            Invoke(nameof(ResetCanDash), _dashCooldown);
        }

        _inputsManager.DisableDashInput(); // Disable dash input boolean
    }
   
    // Executed once before dash started
    void StartDash()
    {
        _startDashPos = transform.position;
        _endDashPos = _startDashPos + (_inputsManager.MoveInput * _dashLenght); // Calculate end dash position
        _isDashing = true;
        _canDash = false;
    }

    // Executed when dash is performed
    void Dash()
    {
        // Lerp velocity to dash speed if timer is not finished
        if(_dashTimer < _dashDuration)
        {
            // Smooth change end dash position if it's activated
            if (_canChangeDirectionDuringDash)
            {
                _endDashPos = _startDashPos + ((Vector2)_currentRotator.up * _dashLenght);
            }

            // Calculate and set position of player
            Vector2 currentDashPosition = Vector2.Lerp(_startDashPos, _endDashPos, _dashCurve.Evaluate(_dashTimer / _dashDuration));
            _rb.MovePosition(currentDashPosition);

            _dashTimer += Time.fixedDeltaTime;
        }
        // If timer is finished, reset dash
        else
        {
            ResetDash();
        }
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

    // ----- Color ----- //

    void ManageColor()
    {
        // Manage sprite color
        if (_isDashing)
            _sr.color = _colorWhenDash;
        else
            _sr.color = _normalColor;
    }

    // ----- Debug ----- //

    private void OnDrawGizmos()
    {
        if (!_showDebug)
            return;

        // Draw current rotation line
        Gizmos.color = _currentForwardDebugColor;
        Gizmos.DrawLine(transform.position, transform.position + (_currentRotator.up * _forwardDebugLenght));

        // Draw target rotation line
        Gizmos.color = _targetForwardDebugColor;
        Gizmos.DrawLine(transform.position, transform.position + (_targetRotator.up * _forwardDebugLenght));
    }
}
