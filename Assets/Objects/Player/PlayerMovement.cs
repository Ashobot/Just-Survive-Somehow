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
    InputsManager _inputsManager;

    Rigidbody2D _rb;

    // Values
    [Title("Movement", "white", "light blue")]

    [SerializeField, MinValue(0)] float _movementSpeed = 200f;
    bool _isMoving;

    [Title("Dash", "white", "blue")]

    [SerializeField] bool _canMoveDuringDash = true;
    [SerializeField, ShowIf(nameof(_canMoveDuringDash), true)] float _dashMoveTime = 0.75f;
    [SerializeField, ShowIf(nameof(_canMoveDuringDash), true)] float _dashMoveMaxAngle = 30f;
    [Space]
    [SerializeField, MinValue(0)] float _dashCooldown = 1f;
    [Line("blue")]
    [SerializeField] AnimationCurve _dashCurve;
    [SerializeField, MinValue(0)] float _dashLenght = 3f;
    [SerializeField, MinValue(0)] float _dashDuration = 0.25f;
    float _dashTimer = 0f;
    bool _canDash = true;
    bool _isDashing = false;
    Vector2 _startDashPos;
    Vector2 _endDashPos;
    Vector2 _lastDashTargetEndPos;
    Vector2 _dashVelocityRef;

    [Title("Debug", "white", "grey")]

    [SerializeField] bool _showDebug;
    [SerializeField, ShowIf(nameof(_showDebug), true)] Color _dashLineDebugColor;
    [SerializeField, ShowIf(nameof(_showDebug), true)] Color _dashTargetPosDebugColor;

    // ----- Functions ----- //

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        _inputsManager = _playerController.InputManager;
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
        if(_inputsManager.MoveInput != Vector2.zero && !_isDashing)
        {
            _isMoving = true;
            _rb.MovePosition((Vector2)transform.position + (_inputsManager.MoveInput * _movementSpeed * Time.deltaTime));
        }
        else
        {
            _isMoving = false;
        }
    }

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
        _endDashPos = _startDashPos + (_inputsManager.MoveInput * _dashLenght);
        _isDashing = true;
        _canDash = false;
    }

    // Executed when dash is performed
    void Dash()
    {
        // Lerp velocity to dash speed if timer is not finished
        if(_dashTimer < _dashDuration)
        {
            //if (_canMoveDuringDash)
            //{
            //    Vector2 _targetEndDashPos;
            //    if(_inputsManager.MoveInput != Vector2.zero)
            //    {
            //        _targetEndDashPos = _startDashPos + (_inputsManager.MoveInput * _dashLenght);
            //        _lastDashTargetEndPos = _targetEndDashPos;
            //    }
            //    else
            //    {
            //        _targetEndDashPos = _lastDashTargetEndPos;
            //    }

            //    Debug.Log(Vector2.Angle(_endDashPos, _targetEndDashPos));
            //    float _currentAngle = Vector2.Angle(_endDashPos, _targetEndDashPos);
            //    if (_currentAngle < _dashMoveMaxAngle)
            //    {
            //        // Smooth current dash end position to target dash end position
            //        _endDashPos = Vector2.SmoothDamp(_endDashPos, _targetEndDashPos, ref _dashVelocityRef, _dashMoveTime);
            //    }
            //    else
            //    {
            //        Vector2 _angleTargetEndDashPos = (Vector2)(Quaternion.Euler(0, 0, _dashMoveMaxAngle) * Vector2.up);
            //        _endDashPos = Vector2.SmoothDamp(_endDashPos, _angleTargetEndDashPos, ref _dashVelocityRef, _dashMoveTime);
            //    }
            //}

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

    // ----- Debug ----- //

    private void OnDrawGizmos()
    {
        if (!_showDebug)
            return;

        if(_isDashing && _canMoveDuringDash)
        {
            Gizmos.color = _dashLineDebugColor;
            Gizmos.DrawLine(_startDashPos, _endDashPos);

            Gizmos.color = _dashTargetPosDebugColor;
            Gizmos.DrawSphere(_lastDashTargetEndPos, 0.3f);
        }
    }
}
