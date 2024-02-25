using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateAttributesPack;

public class PlayerController : MonoBehaviour
{
    // ----- Variables ----- //

    // Player components
    public PlayerMovement PlayerMovement { get { return _playerMovement; } }
    PlayerMovement _playerMovement;

    // Other components
    public InputsManager InputManager { get { return _inputManager; } }
    [SerializeField, Required(false)] InputsManager _inputManager;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

}
