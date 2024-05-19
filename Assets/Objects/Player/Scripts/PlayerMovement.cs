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

    // Rotations
    [Title("Rotations", "white", "green")]

    [SerializeField] Transform _currentRotator;
    public Transform CurrentRotator { get { return _currentRotator; } }
    [SerializeField] Transform _targetRotator;
    public Transform Transform { get { return _targetRotator; } }   

    // Movement
    [Title("Movement", "white", "light blue")]

    [SerializeField, MinValue(0)] float _movementSpeed = 8f;
    public float MovementSpeed { get { return _movementSpeed; } }

    [LineTitle("Damaged", "white", "light blue")]
    [SerializeField, MinValue(0)] float _damagedMovementSpeed = 4f;
    public float DamagedMovementSpeed { get { return _damagedMovementSpeed; } }
    [SerializeField] AnimationCurve _damagedMovementSpeedRecuperationCurve;
    public AnimationCurve DamagedMovementSpeedRecuperationCurve { get { return _damagedMovementSpeedRecuperationCurve; } }
    [Space]
    [SerializeField] bool _smoothChangeDirection;
    [SerializeField, ShowIf(nameof(_smoothChangeDirection), true), MinValue(0)] float _moveChangeDirectionSpeed = 5f;
    [SerializeField] bool _normalizeMovement;
    bool _isMoving;
    public bool IsMoving => _isMoving;
    Vector2 _lastDirection;
    public Vector2 LastDirection => _lastDirection;
    float _currentMovementSpeed;

    // Dash
    [Title("Dash", "white", "blue")]

    [Space]
    [SerializeField] bool _canChangeDirectionDuringDash = true;
    [SerializeField, ShowIf(nameof(_canChangeDirectionDuringDash), true), MinValue(0)] float _dashChangeDirectionSpeed = 0.75f;
    [Space]
    [SerializeField, MinValue(0)] float _dashCooldown = 1f;
    [Line("blue")]
    [SerializeField] AnimationCurve _dashCurve;
    [Space]
    [SerializeField, MinValue(0)] float _dashLenght = 3f;
    public float DashLenght => _dashLenght;  
    [SerializeField, MinValue(0)] float _dashDuration = 0.25f;
    public float DashDuration => _dashDuration;
    
    [LineTitle("Damaged", "white", "blue")]
    [SerializeField, MinValue(0)] float _damagedDashLenght;
    public float DamagedDashLenght => _damagedDashLenght;
    [SerializeField, MinValue(0)] float _damagedDashDuration;
    public float DamagedDashDuration => _damagedDashDuration;
    float _currentDashLenght;
    float _currentDashDuration;

    [Space]
    [SerializeField] Color _colorWhenDash;
    float _dashTimer = 0f;
    bool _canDash = true;
    bool _isDashing = false;
    public bool IsDashing => _isDashing;
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
        _currentMovementSpeed = _movementSpeed;
        _currentDashLenght = _dashLenght;
        _currentDashDuration = _dashDuration;
    }

    private void Update()
    {
        ManageRotators();
        ManageDash();

        ManageColor();
    }

    private void FixedUpdate()
    {
        if (!_playerController.PlayerTrigger.IsDead)
        {
            Move();

            if (_isDashing)
                Dash();
        }
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

            _lastDirection = _inputsManager.MoveInput;

            if(_smoothChangeDirection) // Smooth change direction when moving if it's activated
                _rb.MovePosition((Vector2)transform.position + ((Vector2)_currentRotator.up * _currentMovementSpeed * Time.deltaTime * (_normalizeMovement ? _inputsManager.MoveInput.magnitude : 1)));
            else
                _rb.MovePosition((Vector2)transform.position + (_inputsManager.MoveInput * _currentMovementSpeed * Time.deltaTime * (_normalizeMovement ? _inputsManager.MoveInput.magnitude : 1)));
        }
        else
            _isMoving = false;
    }

    public void SetDamagedPercent(float percent)
    {
        _currentMovementSpeed = Mathf.Lerp(_damagedMovementSpeed, _movementSpeed, _damagedMovementSpeedRecuperationCurve.Evaluate(percent));
        _currentDashLenght = percent < 1 ? _damagedDashLenght : _dashLenght;
        _currentDashDuration = percent < 1 ? _damagedDashDuration : _dashDuration;
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
        _endDashPos = _startDashPos + (_inputsManager.MoveInput * _currentDashLenght); // Calculate end dash position
        _isDashing = true;
        _canDash = false;
    }

    // Executed when dash is performed
    void Dash()
    {
        // Lerp velocity to dash speed if timer is not finished
        if(_dashTimer < _currentDashDuration)
        {
            // Smooth change end dash position if it's activated
            if (_canChangeDirectionDuringDash)
            {
                _endDashPos = _startDashPos + ((Vector2)_currentRotator.up * _currentDashLenght);
            }

            // Calculate and set position of player
            Vector2 currentDashPosition = Vector2.Lerp(_startDashPos, _endDashPos, _dashCurve.Evaluate(_dashTimer / _currentDashDuration));
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

    // ----------------- //
    // ----- Debug ----- //
    // ----------------- //

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
