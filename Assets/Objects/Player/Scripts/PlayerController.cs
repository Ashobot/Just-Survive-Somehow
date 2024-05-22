using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateAttributesPack;

public class PlayerController : MonoBehaviour
{
    // ----- Variables ----- //

    // Player components
    PlayerMovement _playerMovement;
    public PlayerMovement PlayerMovement => _playerMovement;

    PlayerTriggers _playerTriggers;
    public PlayerTriggers PlayerTrigger => _playerTriggers;

    PlayerAnimations _playerAnimations;
    public PlayerAnimations PlayerAnimations => _playerAnimations;

    // Other components
    public InputsManager InputManager { get { return _inputManager; } }
    [SerializeField, Required(false)] InputsManager _inputManager;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerTriggers = GetComponent<PlayerTriggers>();
        _playerAnimations = GetComponent<PlayerAnimations>();
    }
}
