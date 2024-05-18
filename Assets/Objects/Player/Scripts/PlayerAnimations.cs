using UnityEngine;
using UltimateAttributesPack;

public class PlayerAnimations : MonoBehaviour
{
    PlayerController _controller;
    InputsManager _inputsManager;

    Animator _animator;
    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();  
    }

    private void Start()
    {
        _inputsManager = _controller.InputManager;
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!_controller.PlayerTrigger.IsDead)
        {
            ManageAnimatorParameters();
            ManageFlipX();      
        }
    }

    void ManageFlipX()
    {
        if (_controller.PlayerMovement.IsMoving)
        {
            if(_inputsManager.MoveInput.x > 0)
                _spriteRenderer.flipX = false;
            else if(_inputsManager.MoveInput.x < 0)
                _spriteRenderer.flipX = true;
        }
    }

    void ManageAnimatorParameters()
    {
        _animator.SetBool("IsMoving", _controller.PlayerMovement.IsMoving);
        _animator.SetBool("IsDashing", _controller.PlayerMovement.IsDashing);

        _animator.SetFloat("MovementX", Mathf.Clamp(Mathf.Abs(_inputsManager.MoveInput.x), 0.01f, 1f));

        if(_controller.PlayerMovement.IsMoving || _controller.PlayerMovement.IsDashing)
            _animator.SetFloat("MovementY", _inputsManager.MoveInput.y);
        else
            _animator.SetFloat("MovementY", (_controller.PlayerMovement.LastY > 0 ? 1 : -1));
    }

    public void SetDeathAnimation()
    {
        _animator.SetBool("IsDead", true);
    }
}
