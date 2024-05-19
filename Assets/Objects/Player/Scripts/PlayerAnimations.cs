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
        }
    }

    void ManageAnimatorParameters()
    {
        _animator.SetBool("IsMoving", _controller.PlayerMovement.IsMoving);
        _animator.SetBool("IsDashing", _controller.PlayerMovement.IsDashing);

        _animator.SetFloat("MovementX", _controller.PlayerMovement.LastDirection.x);
        _animator.SetFloat("MovementY", _controller.PlayerMovement.LastDirection.y);
    }

    public void SetDeathAnimation()
    {
        _animator.SetBool("IsDead", true);
    }
}
